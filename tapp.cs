using UnityEngine;
using System.Collections;
//торпедный аппарат
public class tapp : MonoBehaviour {
	public GameObject projectile;
	public ushort cooldown=60;
	public bool ready=true;
	public int range=1500;
	public int damage=2500;
	public int energy=100;
	public Transform[] guns;
	public Vector3 local_attack_vector;
	Collider mcollider;
	bool simulation=true;

	void Start () {
		if (!projectile) projectile=ResLoad.torpedo;
		if (transform.root.GetComponent<NetworkShipController>()) {transform.root.GetComponent<NetworkShipController>().tapps=true;mcollider=transform.root.GetComponent<NetworkShipController>().c1;}
		else {mcollider=transform.root.gameObject.GetComponent<Collider>();}
	}

	public void Torpedo () {
		Vector3 attack_vector=transform.TransformDirection(local_attack_vector);
		Vector3 root_pos=transform.root.transform.position;
		for (byte i=0;i<guns.Length;i++) {
			GameObject x=Network.Instantiate(projectile,guns[i].transform.position,transform.root.rotation,0) as GameObject;
			x.transform.forward=transform.root.TransformDirection(local_attack_vector);
			x.GetComponent<torpedo>().timer=range/x.GetComponent<torpedo>().speed;
			x.GetComponent<torpedo>().damage=damage;
			Physics.IgnoreCollision(mcollider,x.GetComponent<Collider>());
		}
		if (!simulation) {
		ready=false;
		StartCoroutine(Reload());
		}
	}

	public void ServerComponent(bool x) {
		if (x) simulation=false;
		transform.root.SendMessage("MyDistIs",new Vector2(range,1),SendMessageOptions.DontRequireReceiver);
	}

	IEnumerator Reload() {
		transform.root.SendMessage("GunShooted",1,SendMessageOptions.DontRequireReceiver);
		ready=false;
		yield return new WaitForSeconds(cooldown);
		ready=true;
		transform.root.SendMessage("GunReady",1,SendMessageOptions.DontRequireReceiver);
	}
}
