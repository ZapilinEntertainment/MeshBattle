using UnityEngine;
using System.Collections;

public class env_camera : MonoBehaviour {
	public Transform c_satellite;
	public int koefficient=60;
	public GameObject nearest;


	void Start() {
		if (Global.cam!=null)
		c_satellite=Global.cam.transform.parent;
	}
	// Update is called once per frame
	void LateUpdate () {
		if (!Global.cam) return;
		if (!c_satellite) {c_satellite=Global.cam.transform.parent;}
		transform.localPosition=c_satellite.transform.position/koefficient;
		transform.forward=Global.cam.transform.forward;
	}
}
