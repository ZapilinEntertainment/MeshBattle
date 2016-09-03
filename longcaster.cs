using UnityEngine;
using System.Collections;

public class longcaster : MonoBehaviour {
	public Transform[] guns;
	cast_laser[] rays;
	public int range=1000;
	public short dps=100;
	public float time=15;
	public float cooldown=60;
	public int eps=200;
	public Vector3 attackVector;
	NetworkShipController nsc;
	public Renderer rr;
	bool bot=false;
	bool simulation=true;
	Texture normal_light_tx;
	public Texture firelight_tx;
	public bool forward=false;
	public bool right=false;
	public bool ready=true;
	public bool firing=false;
	float weapon_bonus=1;
	char command;
	bool game_started=false;
	// Use this for initialization
	void Start () {
		if (rr) normal_light_tx=rr.material.GetTexture("_EmissionMap");
		nsc=transform.root.GetComponent<NetworkShipController>();
		if (nsc) weapon_bonus=nsc.weapons_bonus;
		rays=new cast_laser[guns.Length];
		LineRenderer x;
		for (byte i=0;i<guns.Length;i++) {
			x=Instantiate(ResLoad.laser_ray,guns[i].transform.position,Quaternion.identity) as LineRenderer;
			rays[i]=x.gameObject.AddComponent<cast_laser>();
			rays[i].gun=guns[i];
			x.transform.parent=transform;
		}
		if (transform.localPosition.x>0) {right=true;}
		if (attackVector==Vector3.zero) {if (!forward) {if (right) attackVector=Vector3.right; else attackVector=Vector3.left;} else attackVector=Vector3.forward;}
	}

	public void StartGame () {
		if (firing) {firing=false;StopCoroutine(FiringTime());foreach (cast_laser element in rays) {element.gameObject.SetActive(false);}}
		if (!ready) {StopCoroutine(Reload());ready=true;}
		command=transform.root.name[0];
		game_started=true;
	}

	// Update is called once per frame
	void Update () {
		Vector3 a_vector=transform.root.TransformDirection(attackVector);
		if (firing) {
			float range2=range;
			if (nsc) {if (nsc.capacity<=0) {StopCoroutine(FiringTime());StartCoroutine(Reload());} weapon_bonus=nsc.weapons_bonus;range2=range*weapon_bonus;}
			RaycastHit rh;
			for (byte i=0;i<guns.Length;i++) {
				if (Physics.Raycast(guns[i].position,a_vector,out rh,range2)) {
					if (bot&&rh.collider.transform.root.gameObject.name[0]==transform.root.gameObject.name[0]) {Reload();return;}
					float dmg=dps*Time.deltaTime*weapon_bonus;
					dmg=dmg/2+dmg/2*rh.distance/range2;
					rh.collider.transform.root.SendMessage("ApplyDamage",new Vector4(rh.point.x,rh.point.y,rh.point.z,dmg),SendMessageOptions.DontRequireReceiver);
					rays[i].targetpos=rh.point;
				}
				else {rays[i].targetpos=guns[i].position+a_vector*range2;}
			}
		}
		else {
			if (game_started&&!simulation&&bot&&ready) {
				RaycastHit ht;
				if (Physics.Raycast(transform.position,a_vector,out ht,range)&&ht.collider.transform.root.gameObject.name[0]!=command) {Fire();	}
			}
		}
	}

	public void Fire () {
		for (byte i=0;i<guns.Length;i++) {
			rays[i].gameObject.SetActive(true);
		}
		firing=true;
		if (rr) rr.material.SetTexture("_EmissionMap",firelight_tx);
		ready=false;
		if (nsc) nsc.constant_supply+=eps;
		StartCoroutine(FiringTime());if (bot) transform.root.SendMessage("GunShooted",0,SendMessageOptions.DontRequireReceiver);
	}

	IEnumerator FiringTime () {
		yield return new WaitForSeconds(time);
		firing=false;
		StartCoroutine(Reload());
	}

	IEnumerator Reload () {
		firing=false;
		if (nsc) nsc.constant_supply-=eps;
		if (rr) rr.material.SetTexture("_EmissionMap",normal_light_tx);
		for (byte i=0;i<guns.Length;i++) {
			rays[i].gameObject.SetActive(false);
		}
		yield return new WaitForSeconds(cooldown);
		ready=true;
		if (bot&&!simulation) transform.root.SendMessage("GunReady",0,SendMessageOptions.DontRequireReceiver);
	}

	public void ServerComponent(bool x) {
		simulation=!x;
	}
	public void BotControlled (bool x) {
		bot =x;
	}
}
