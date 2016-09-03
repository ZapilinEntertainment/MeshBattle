using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class laser_caster : MonoBehaviour {
	public float max_angle=5;
	public int range=1000;
	public float cooldown=30;
	public int dps=100;
	public float time=3;
	float time_left;
	public float weapon_bonus=1;
	public Transform[] guns;
	public Vector3 gun_point;
	cast_laser[] rays;
	public bool ready=true;
	bool game_started=false;
	public bool right=false;
	public bool firing=false;
	public bool forward=false;
	NetworkShipController nsc;
	public GameObject target;
	public float might=2;
	public short eps=0;
	public Renderer rr;
	Texture normal_light;
	public Texture firing_light;


	void Start () {
		if (rr) normal_light=rr.material.GetTexture("_EmissionMap");
		nsc=transform.root.GetComponent<NetworkShipController>();
		if (transform.localPosition.x>0) right=true;
		if (guns.Length>0) {
			rays=new cast_laser[guns.Length];
			for (byte i=0;i<guns.Length;i++) {
				LineRenderer x=Instantiate(ResLoad.laser_ray,guns[i].position,Quaternion.identity) as LineRenderer;
				x.gameObject.transform.parent=transform;
				cast_laser lr=x.gameObject.AddComponent<cast_laser>();
				lr.gun=guns[i];
				rays[i]=lr;
			}}
		gun_point=Vector3.zero;
		foreach (Transform t in guns) {
			Vector3 p=transform.InverseTransformPoint(t.position);
			gun_point+=p;
		}
		gun_point/=guns.Length;
	}

	void Update () {
		if (firing) {
			if (target==null||nsc.capacity<=0) {if (firing) {firing=false;StartCoroutine(Reload());}}
			else {
				time_left-=Time.deltaTime;
				if (time_left<=0) {firing=false;time_left=0; StartCoroutine(Reload());}
				else {
				float a;
				Vector3 dir=transform.root.InverseTransformPoint(target.transform.position)-gun_point;
				if (forward) {
					a=Vector3.Angle(dir,Vector3.forward);
				}
				else {if (right) {a=Vector3.Angle(dir,Vector3.right);} else {a=Vector3.Angle(dir,Vector3.left);}}
				if (a>max_angle||dir.magnitude>range*weapon_bonus) {target=null;firing=false;StartCoroutine(Reload());}
					else {
						RaycastHit rh;
						for (byte k=0;k<rays.Length;k++) {
							if (Physics.Raycast(guns[k].position,target.transform.position-guns[k].position,out rh,range*weapon_bonus)) {
								rays[k].targetpos=rh.point;
								float damage=dps*Time.deltaTime*weapon_bonus;
								damage=damage/2+damage/2*rh.distance/range;
								rh.collider.SendMessage("ApplyDamage",new Vector4 (rh.point.x,rh.point.y,rh.point.z,damage),SendMessageOptions.DontRequireReceiver);
								};
						}
					}
				}
			}
		}
		if (nsc) {weapon_bonus=nsc.weapons_bonus;}
	}

	public void StartGame () {game_started=true;target=null;if (firing) {firing=false;foreach (cast_laser element in rays) {element.gameObject.SetActive(false);}};if (!ready) {StopCoroutine(Reload());ready=true;}}

	public short Focus () {
		if (!ready||firing||nsc.marked_targets.Length==0||nsc.capacity<eps) return(-1);
		GameObject pred_target=null;
		short index=-1;
		Vector3 dir;
		float pred_dist=range*weapon_bonus;
		float radius=range*Mathf.Tan(max_angle/180*Mathf.PI);
		RaycastHit rh;
		if (forward) {
			for (byte i=0;i<nsc.marked_targets.Length;i++) {
				if (nsc.marked_targets[i]==null) continue;
				dir=transform.root.InverseTransformPoint(nsc.marked_targets[i].transform.position)-gun_point;
				if (dir.z>0&&dir.magnitude<range*weapon_bonus) {
					if (Vector3.Angle(dir,Vector3.forward)<=max_angle&&(dir.x>0&&right||dir.x<0&&!right)) {
						if (dir.magnitude<pred_dist) {pred_target=nsc.marked_targets[i];index=i;pred_dist=dir.magnitude;}
					}
					else {
						Vector3 a=dir-new Vector3(0,0,range*weapon_bonus);
						a=a.normalized*radius;
						if (Physics.Raycast(gun_point,a,out rh,range*weapon_bonus)) {
							short idx=isMarkedTarget(rh.collider.transform.root.gameObject);
							if (idx!=-1) {pred_target=nsc.marked_targets[idx];index=idx;pred_dist=rh.distance;}
						}
					}
				}
			}
		}
		else {
			for (byte i=0;i<nsc.marked_targets.Length;i++) {
				dir=transform.root.InverseTransformPoint(nsc.marked_targets[i].transform.position);
				RaycastHit hit;
				if (Physics.Raycast(transform.position,nsc.marked_targets[i].transform.position-transform.position,out hit,range*weapon_bonus)&&hit.collider.transform.root.gameObject.name[0]!=nsc.command) {
					if (right) {
						if (dir.x>0&&Vector3.Angle(dir,Vector3.right)<=max_angle) {if (dir.magnitude<pred_dist) {pred_target=nsc.marked_targets[i];index=i;pred_dist=dir.magnitude;}}
					}
					else {
						if (dir.x<0&&Vector3.Angle(dir,Vector3.left)<=max_angle) {if (dir.magnitude<pred_dist) {pred_target=nsc.marked_targets[i];index=i;pred_dist=dir.magnitude;}}
					}
				} 
			}
	}
		return(index);
	}

	short isMarkedTarget (GameObject x) {
		for (byte i=0;i<nsc.marked_targets.Length;i++) {
			if (x==nsc.marked_targets[i]) {return (i);}
		}
		return (-1);
	}

	public void Fire (byte index) {
		if (rr) {rr.material.SetTexture("_EmissionMap",firing_light);}
		target=nsc.marked_targets[index];
			firing=true;
			ready=false;
			time_left=time;
			for (byte j=0;j<guns.Length;j++) {
				rays[j].gameObject.SetActive(true);
			}
		nsc.constant_supply+=eps;
		}
			
		
	IEnumerator Reload () {
		nsc.constant_supply-=eps;
		if (rr) {rr.material.SetTexture("_EmissionMap",normal_light);}
		for (byte j=0;j<guns.Length;j++) {
			rays[j].gameObject.SetActive(false);
		}
		firing=false;ready=false;target=null;
		yield return new WaitForSeconds(cooldown);
		ready=true;
	}


}
