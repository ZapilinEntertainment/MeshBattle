using UnityEngine;
using System.Collections;


public class pointpulser : MonoBehaviour {
	NetworkShipController nsc;
	public Transform[] guns;
	public bool ready=true;
	public byte max_angle=30;
	public uint range=800;
	public uint damage=500;
	public float cooldown=40;
	public int energy=220;
	public bool right=false;
	public bool forward=false;
	float weapon_bonus=1;
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
		if (!ready) {StopCoroutine(Reloading());ready=true;StopCoroutine(Splash());}}

	// Update is called once per frame
	void Update () {
		if (nsc) weapon_bonus=nsc.weapons_bonus;
	}

	public void Fire(byte index) {
		RaycastHit rh;
		Transform target=nsc.marked_targets[index].transform;
		if (Physics.Raycast(transform.position,target.position-transform.position,out rh,range*weapon_bonus)) {
			if (rh.collider.transform.root.gameObject.name[0]!=nsc.command) {
				float dmg=damage*weapon_bonus;
				dmg=dmg/2+dmg/2*rh.distance/range;
				rh.collider.transform.root.SendMessage("ApplyDamage",new Vector4(rh.point.x,rh.point.y,rh.point.z,dmg),SendMessageOptions.DontRequireReceiver);
				StartCoroutine(Splash());
				for (byte i=0;i<guns.Length;i++) {
					GameObject x=Instantiate(Resources.Load<GameObject>("whiteLaserLine"),transform.position,Quaternion.identity) as GameObject;
					pulse_laser pl=x.AddComponent<pulse_laser>();
					pl.gun=guns[i];
					pl.target=target;
					pl.start=damage/200;
					pl.speed=(pl.start-0.1f)/1.5f;
				}
			}
		}
		for (byte i=0;i<guns.Length;i++) {
	
		}
		ready=false;
		StartCoroutine(Reloading());
	}

	IEnumerator Reloading () {
		yield return new WaitForSeconds(cooldown);
		ready=true;
	}

	IEnumerator Splash () {
		rr.material.SetTexture("_EmissionMap",firing_light);
		yield return new WaitForSeconds(1.5f);
		rr.material.SetTexture("_EmissionMap",normal_light);
	}

	public short Focus () {
		if (!ready||nsc.marked_targets.Length==0) return(-1);
		GameObject pred_target=null;
		short index=-1;
		Vector3 dir;
		float pred_dist=range*weapon_bonus;
			for (byte i=0;i<nsc.marked_targets.Length;i++) {
				dir=transform.root.InverseTransformPoint(nsc.marked_targets[i].transform.position);
				RaycastHit hit;
			if (Physics.Raycast(transform.position,nsc.marked_targets[i].transform.position-transform.position,out hit,range)&&hit.collider.transform.root.gameObject.name[0]!=nsc.command) {
					if (right) {
						if (dir.x>0&&Vector3.Angle(dir,Vector3.right)<=max_angle) {if (dir.magnitude<pred_dist) {pred_target=nsc.marked_targets[i];index=i;pred_dist=dir.magnitude;}}
					}
					else {
						if (dir.x<0&&Vector3.Angle(dir,Vector3.left)<=max_angle) {if (dir.magnitude<pred_dist) {pred_target=nsc.marked_targets[i];index=i;pred_dist=dir.magnitude;}}
					}
				} 
			}
		return(index);
	}
}

