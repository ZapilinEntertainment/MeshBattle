using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class camera_control : MonoBehaviour {
	public GameObject ship;
	public byte rotation_speed=70;
	public byte zoom_speed=50;
	public bool special=false;
	public Transform satellite;
	public GameObject cam;
	// Use this for initialization
	void Awake () {
		if (!satellite) satellite=transform.GetChild(0);
		if (!cam) {cam=satellite.transform.GetChild(0).gameObject;}
		Global.cam=cam;
		GameObject env_cam=GameObject.Find("menu").GetComponent<SceneResLoader>().environmental_camera;
		if (env_cam) {env_cam.GetComponent<env_camera>().c_satellite=satellite;}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (ship!=null) transform.position=ship.transform.position; 
		if (!special) {rotation_speed=Global.cam_rot_speed;zoom_speed=Global.cam_zoom_speed;}
		if (Input.GetMouseButton(2)) {
		    satellite.RotateAround(transform.position,transform.TransformDirection(Vector3.up),rotation_speed*Input.GetAxis("Mouse X"));
			satellite.RotateAround(transform.position,satellite.transform.TransformDirection(Vector3.left),rotation_speed*Input.GetAxis("Mouse Y"));
		}
		if (Input.GetKey(KeyCode.KeypadDivide)) { satellite.RotateAround(transform.position,transform.TransformDirection(Vector3.up),rotation_speed/30);}
		if (Input.GetKey(KeyCode.KeypadMultiply)) { satellite.RotateAround(transform.position,transform.TransformDirection(Vector3.up),-rotation_speed/30);}
		if (Input.GetKey(KeyCode.KeypadPlus)) { cam.transform.Translate(0,0,zoom_speed/30,cam.transform);}
		if (Input.GetKey(KeyCode.KeypadMinus)) { cam.transform.Translate(0,0,-zoom_speed/30,cam.transform);}
		if (Input.GetKey(KeyCode.Insert)) { satellite.RotateAround(transform.position,satellite.transform.TransformDirection(Vector3.left),-rotation_speed/30);}
		if (Input.GetKey(KeyCode.Delete)) { satellite.RotateAround(transform.position,satellite.transform.TransformDirection(Vector3.left),rotation_speed/30);}
		float sw=Input.GetAxis("Mouse ScrollWheel");
		if (sw!=0) {cam.transform.Translate(0,0,sw*zoom_speed,cam.transform);}
	}

	public void SetShip (GameObject a) {
		ship=a;transform.position=ship.transform.position;
	}	






}