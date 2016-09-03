using UnityEngine;
using System.Collections;

public class eyegun : MonoBehaviour {
	public Texture t0;
	public Texture t1;
	public Texture t2;
	public Texture t3;
	byte next_tex=0;
	float angle_acceleration=30;
	public Renderer rr;
	public GameObject target;
	GameObject splash;
	GameObject splash2;
	LineRenderer ray;
	public int damage=50;
	public int range=1000;
	// Use this for initialization
	void Start () {
		StartCoroutine(Changing());
	}

	IEnumerator Changing() {
		yield return new WaitForSeconds(1);
		switch (next_tex) {
		case 0: rr.material.mainTexture=t1;break;
		case 1: rr.material.mainTexture=t2;break;
		case 2: rr.material.mainTexture=t3;break;
		}
		next_tex++;
		if (next_tex!=3) StartCoroutine(Changing());
		else Fire();
	}

	// Update is called once per frame
	void Update () {
		Quaternion rotateTo=Quaternion.LookRotation(target.transform.position-transform.position);
		transform.rotation=Quaternion.RotateTowards(transform.rotation,rotateTo,angle_acceleration*Time.deltaTime);
	}

	void Fire() {
		StartCoroutine(fireEnd());
		splash2=Instantiate(ResLoad.laser_splash_sprite,transform.position,transform.rotation) as GameObject;
		splash2.transform.parent=transform;
		splash2.SetActive(true);
		RaycastHit hit;
		if (Physics.Raycast(transform.position,transform.forward,out hit,range)) {
			hit.collider.transform.root.SendMessage("ApplyDamage",new Vector4(hit.point.x,hit.point.y,hit.point.z,damage),SendMessageOptions.DontRequireReceiver);
			splash=Instantiate(ResLoad.laser_splash_sprite,hit.point,transform.rotation) as GameObject;
			splash.SetActive(true);
			ray=Instantiate(ResLoad.laser_ray,transform.position,transform.rotation) as LineRenderer;
			ray.SetPosition(0,transform.position);
			ray.SetPosition(1,hit.point);
			ray.transform.parent=transform;
			ray.gameObject.SetActive(true);
		}
		else {
			ray=Instantiate(ResLoad.laser_ray,transform.position,transform.rotation) as LineRenderer;
			ray.SetPosition(0,transform.position);
			ray.SetPosition(1,transform.position+transform.forward.normalized*range);
			ray.transform.parent=transform;
			ray.gameObject.SetActive(true);
		}
	}

	IEnumerator fireEnd() {
					yield return new WaitForSeconds(0.5f);
		if (splash) Destroy(splash);
		Destroy(gameObject);
	}
}
