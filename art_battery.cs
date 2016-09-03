using UnityEngine;
using System.Collections;

public class art_battery : MonoBehaviour {
	public int damage=500;
	public int range=1000;
	public float cooldown=20;
	public Vector3[] gun_points;
	public GameObject light_splash;
	public int light_range=10;
	public char command='0';
	public bool right=true;
	public bool ready=true;
	public bool auto=false;
	bool game_started=false;
	bool simulation=true;
	Vector3 a_vector;

	// Use this for initialization
	void Start () {
		if (!light_splash) {light_splash=ResLoad.light_splash;}
		light_splash.GetComponent<Light>().range=light_range;
	}

	public void StartGame () {
		command=transform.root.gameObject.name[0];
		game_started=true;
	}
	// Update is called once per frame
	void Update () {
		if (!game_started) return;
		if (auto&&ready) {
			RaycastHit ht;
			if (right) a_vector=transform.root.TransformDirection(Vector3.right);
			else a_vector=transform.root.TransformDirection(Vector3.left);
			if (Physics.Raycast(transform.position,a_vector,out ht,range)&&ht.collider.transform.root.gameObject.name[0]!=command) {
					Fire();
				}

		}
	}

	public void Fire () {
		GameObject ls=Instantiate(ResLoad.light_splash,transform.position,transform.rotation) as GameObject; 
		ls.SetActive(true);
		ls.transform.parent=transform;
		bool a=false;
		for (byte i=0;i<gun_points.Length;i++) {
			RaycastHit hit;
			Vector3 point=transform.TransformPoint(gun_points[i]);
			  if (Physics.Raycast(point,a_vector,out hit,range)) {
					hit.collider.transform.root.SendMessage("ApplyDamage",new Vector4(hit.point.x,hit.point.y,hit.point.z,damage),SendMessageOptions.DontRequireReceiver);
				if (!a) {ls=Instantiate(ResLoad.small_explosion,hit.point,Quaternion.identity) as GameObject;
					ls.SetActive(true);a=true;}
				}
		}
		if (!simulation) {
		ready=false;
		StartCoroutine(Reloading());
		}
	}

	public void ServerComponent(bool x) {
		if (x) simulation=false;
		transform.root.SendMessage("MyDistIs",new Vector2(range,0),SendMessageOptions.DontRequireReceiver);
	}

	IEnumerator Reloading() {
		transform.root.SendMessage("GunShooted",0,SendMessageOptions.DontRequireReceiver);
		ready=false;
		yield return new WaitForSeconds(cooldown);
		ready=true;
		transform.root.SendMessage("GunReady",0,SendMessageOptions.DontRequireReceiver);
	}
}
