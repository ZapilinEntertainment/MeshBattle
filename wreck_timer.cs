using UnityEngine;
using System.Collections;

public class wreck_timer : MonoBehaviour {
	bool small=false;
	public Vector3 s;

	// Use this for initialization
	void Start () {
	s=GetComponent<BoxCollider>().size;
		float mass=s.x*s.y*s.z;
		if (mass<100) {small=true;}
		GetComponent<Rigidbody>().mass=mass;
		StartCoroutine(Disappearance(mass));
	}

	void Update() {
		if (small) {
			if (Vector3.Distance(transform.position,Global.cam.transform.position)>Global.drawDist1) {Destroy(gameObject);}
		}
	}
	
	IEnumerator Disappearance (float t) {
		yield return new WaitForSeconds(t);
		Destroy(gameObject);
	}
}
