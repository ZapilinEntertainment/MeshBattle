using UnityEngine;
using System.Collections;

public class flare_explosion : MonoBehaviour {
	public float time;
	float cur_time;
	public float acceleration;
	public float start_brightness;
	LensFlare lf;
	bool a=false;
	float dist;
	int max_dist=3000;
	Light l;
	float real_brightness;
	public bool use_light=false;
	// Use this for initialization
	void Start () {
		lf=GetComponent<LensFlare>();
		cur_time=time;
		lf.brightness=start_brightness;
		if (lf.brightness==0) lf.brightness=1;
		if (use_light&&Vector3.Distance(transform.position,Global.cam.transform.position)<Global.drawDist1) {l=gameObject.AddComponent<Light>();l.intensity=4;l.range=100;}
	}
	
	// Update is called once per frame
	void Update () {
		dist=Vector3.Distance(transform.position,Global.cam.transform.position);
		if (dist>max_dist) return;
		if (cur_time>0) {cur_time-=Time.deltaTime;}
			if (cur_time<0) {Destroy(gameObject);}
		if (cur_time>time/2) {
			real_brightness+=acceleration*Time.deltaTime;
			lf.brightness=real_brightness*(1-dist/max_dist);
			if (l!=null) l.range+=acceleration*Time.deltaTime;
		}
		else {if (!a) {acceleration=lf.brightness/time/2;a=true;}
			real_brightness-=acceleration*Time.deltaTime*8;
			lf.brightness=real_brightness*(1-dist/max_dist);}

	}
	}
