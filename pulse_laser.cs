using UnityEngine;
using System.Collections;

public class pulse_laser : MonoBehaviour {
	public float speed=0.05f;
	public float start=0.3f;
	public float end=0.1f;
	public Transform gun;
	public Vector3 targetpos;
	public Transform target;
	LineRenderer lr;
	GameObject lighting_splash;


	void Start () {
		lr=GetComponent<LineRenderer>();
		lighting_splash=Instantiate(Resources.Load<GameObject>("flare_explosion"),transform.position,Quaternion.identity) as GameObject;
		lighting_splash.GetComponent<flare_explosion>().time=1;
		lighting_splash.GetComponent<flare_explosion>().start_brightness=3;
	}

	// Update is called once per frame
	void Update () {
		lr.SetPosition(0,gun.transform.position);
		lighting_splash.transform.position=gun.transform.position;
		if (target==null) {lr.SetPosition(1,targetpos);
}
		else {lr.SetPosition(1,target.transform.position);
}
		if (start>speed) {start-=speed*Time.deltaTime;if (start<speed) start=speed;}
		else {start=speed;}
		if (end>0.01f) {end-=speed*Time.deltaTime;if (end<0.01f) end=0.01f;}
		GetComponent<LineRenderer>().SetWidth(start,end);
		if (start==speed) Destroy(gameObject);
	}
}
