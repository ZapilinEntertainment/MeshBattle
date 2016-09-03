using UnityEngine;
using System.Collections;

public class FlareController : MonoBehaviour {
	LensFlare lf;
	public float kfc=1;
	public int max_distance=3000;
	public float normal_bs;
	public float x;
	// Use this for initialization
	void Start () {
		lf=GetComponent<LensFlare>();
		normal_bs=lf.brightness;
	}
	
	// Update is called once per frame
	void Update () {
		if (Global.cam==null) {lf.brightness=0;}
		else {
			x=Vector3.Distance(Global.cam.transform.position,transform.position);
			lf.brightness=normal_bs*kfc*(1-x/max_distance)*(1-x/max_distance)*(1-x/max_distance);
		}
	}
}
