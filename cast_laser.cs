using UnityEngine;
using System.Collections;

public class cast_laser : MonoBehaviour {

	public float might=2f;
	public Transform gun;
	public Vector3 targetpos;
	 GameObject lighting_splash;
	GameObject warming_splash;
	LineRenderer lr;

	void Start () {
		lr=GetComponent<LineRenderer>();
		lighting_splash=Instantiate(ResLoad.laser_splash_sprite,gun.position,Quaternion.identity) as GameObject;
		lighting_splash.transform.parent=transform;
		lighting_splash.SetActive(true);
		warming_splash=Instantiate(ResLoad.warming_splash_sprite,targetpos,Quaternion.identity) as GameObject;
		warming_splash.transform.parent=transform;
		warming_splash.SetActive(true);
	}

	// Update is called once per frame
	void Update () {
		lr.SetPosition(0,gun.position);
		lighting_splash.transform.position=gun.position;
		lr.SetPosition(1,targetpos);
		warming_splash.transform.position=targetpos;
		float a=0;
		a=Mathf.PingPong(25*Time.time,might);
		lr.SetWidth(a+1,a);
	}
		
}
