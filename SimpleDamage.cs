using UnityEngine;
using System.Collections;

public class SimpleDamage : MonoBehaviour {

	public float maxhp=100;
	float hp;
	public bool make_wreck=false;
	public GameObject pieces;
	
	void Start () {if (!pieces) pieces=ResLoad.pieces_s;hp=maxhp;}
	
	void ApplyDamage (Vector4 x) {
		hp-=(int)x.w;
		if (hp<=0) {
			if (!make_wreck) {
				Instantiate(pieces,transform.position,transform.rotation);
			}
			else {Instantiate(ResLoad.simple_wreck,transform.position,transform.rotation);
			}
			Destroy(gameObject);
		}
	}
}
