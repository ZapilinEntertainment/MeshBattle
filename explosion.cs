using UnityEngine;
using System.Collections;

public class explosion : MonoBehaviour {
	public int radius=70;
	public uint damage=5000;
	// Use this for initialization
	void Start () {
		Collider[] victims=Physics.OverlapSphere(transform.position,radius);
		Vector3 a;
		foreach (Collider victim in victims) {
			if (victim.name=="wreck1") {Destroy(victim.gameObject);continue;}
			a=victim.ClosestPointOnBounds(transform.position);
			float dmg=damage;
			dmg/=radius;
			dmg*=Vector3.Distance(transform.position,victim.transform.position);
			victim.transform.root.gameObject.SendMessage("ApplyDamage",new Vector4(a.x,a.y,a.z,dmg),SendMessageOptions.DontRequireReceiver);
			Rigidbody r=victim.GetComponent<Rigidbody>();
			if (r) r.AddExplosionForce(damage/100,transform.position,radius);
		}
	}
	

}
