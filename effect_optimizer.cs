using UnityEngine;
using System.Collections;

public class effect_optimizer : MonoBehaviour {
	public GameObject sprite;
	public ParticleEmitter emt;
	public float koefficient=2;
	byte mode=0;
	Transform cam;
	public float timer=0;

	public void Start () {
		cam=Global.cam.transform;
		if (timer!=0) StartCoroutine(Timer());
	}
	IEnumerator Timer() {
		yield return new WaitForSeconds(timer);
		gameObject.SetActive(false);
	}
	// Update is called once per frame
	void Update () {
		if (!cam) {cam=Global.cam.transform;return;}
		float dist=Vector3.Distance(transform.position,cam.position);
		byte newmode=0;
		if (dist<Global.drawDist1*koefficient) {
			newmode=1;
		}
		else {
			if (dist<Global.drawDist2*koefficient) {
				newmode=2;
			}
			else newmode=3;
		}
		if (mode!=newmode) {
			switch (newmode) {
			case 0:gameObject.SetActive(false);break;
			case 1:sprite.SetActive(false);emt.emit=true;break;
			case 2:sprite.SetActive(true);emt.emit=false;break;
			case 3:sprite.SetActive(false);if (emt.emit) emt.emit=false;break;
			}
			mode=newmode;
		}
	}
	public void Destroying() {
		mode=0;
		gameObject.SetActive(false);
		transform.parent=GameObject.Find("menu").transform;
	}
}
