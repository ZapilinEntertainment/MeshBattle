using UnityEngine;
using System.Collections;


public class pulser : MonoBehaviour {
	NetworkShipController nsc;
	public Transform[] guns;
	public bool ready=true;
	public uint range=300;
	public uint damage=10;
	public float cooldown=10;
	public int energy=0;
	public bool right=false;
	public bool forward=false;
	float weapon_bonus=1;
	public bool game_started=false;
	public Renderer rr;
	Texture normal_light;
	public Texture firing_light;

	void Start () {
		nsc=transform.root.GetComponent<NetworkShipController>();
		if (nsc) weapon_bonus=nsc.weapons_bonus;
		if (transform.localPosition.x>0) {	right=true;}
		if (rr) normal_light=rr.material.GetTexture("_EmissionMap");
	}

	public void StartGame () {
		game_started=true;
		if (!ready) {StopCoroutine(Reloading());ready=true;StopCoroutine(Splash());}
	}



	void Update () {
		if (nsc) weapon_bonus=nsc.weapons_bonus;
	}

	void Fire() {
		RaycastHit rh;
		Vector3 attack_vector;
		nsc.capacity-=energy;
		if (right) {attack_vector=transform.root.TransformDirection(Vector3.right);} else {attack_vector=transform.root.TransformDirection(Vector3.left);}
		if (Physics.Raycast(transform.position,attack_vector,out rh,range*weapon_bonus)) {
		StartCoroutine(Splash());
		for (byte i=0;i<guns.Length;i++) {
				if (rh.collider.transform.root.gameObject.name[0]!=nsc.command) {
					float dmg=damage*weapon_bonus;
					dmg=dmg/2+dmg/2*rh.distance/range;
				rh.collider.transform.root.SendMessage("ApplyDamage",new Vector4(rh.point.x,rh.point.y,rh.point.z,dmg),SendMessageOptions.DontRequireReceiver);
					GameObject x=Instantiate(Resources.Load<GameObject>("whiteLaserLine"),guns[i].position,Quaternion.identity) as GameObject;
					pulse_laser pl=x.AddComponent<pulse_laser>();
					pl.gun=guns[i];
					pl.targetpos=rh.point;
					pl.start=damage/250;
					pl.speed=(pl.start-0.1f)/1.5f;
			}
		}
		}
		StartCoroutine(Reloading());
	}

	IEnumerator Reloading () {
		ready=false;
		yield return new WaitForSeconds(cooldown);
		ready=true;
	}

	IEnumerator Splash () {
		if (rr) rr.material.SetTexture("_EmissionMap",firing_light);
		yield return new WaitForSeconds(1.5f);
		if (rr) rr.material.SetTexture("_EmissionMap",normal_light);
	}


}
