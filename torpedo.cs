using UnityEngine;
using System.Collections;

public class torpedo : MonoBehaviour {
	public int damage=2500;
	public int timer=30;
	public int speed=200;
	public GameObject explosion;
	Rigidbody rbody;
	Vector3 mvector;
	public GameObject target;
	public float angle_acceleration=10;
	// Use this for initialization


	void Start () {
		if (!explosion) explosion=Resources.Load<GameObject>("small_explosion");
		StartCoroutine(SelfDestruct());
		rbody=GetComponent<Rigidbody>();

	}
	
	IEnumerator SelfDestruct() {
		yield return new WaitForSeconds(timer);
		Instantiate(explosion,transform.position,transform.rotation);
		Destroy(gameObject);
	}


	void Update () {
		if (target) {
			transform.forward=Vector3.RotateTowards(transform.forward,(target.transform.position-transform.position),angle_acceleration*Time.deltaTime,0.0f);
		}
		mvector=transform.TransformDirection(new Vector3(0,0,speed));
		rbody.velocity=mvector;
	}

	void ApplyDamage (Vector4 x) {Instantiate(ResLoad.pieces,transform.position,transform.rotation);Destroy(gameObject);}

	void OnCollisionEnter (Collision c) {
		c.collider.SendMessage("ApplyDamage",new Vector4(transform.position.x,transform.position.y,transform.position.z,damage),SendMessageOptions.DontRequireReceiver);
		Instantiate(explosion,transform.position,transform.rotation);
		Destroy(gameObject);
	}
}
