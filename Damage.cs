using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour {

	public int hp=1000;
	public int maxhp=1000;
	public byte armor=0;
	public GameObject explosion;
	public GameObject wreck;
	float situation=0;
	public bool destructing=false;
	public int length=1;
	
	void Start () {if (!wreck) wreck=ResLoad.simple_wreck;}
	
	void ApplyDamage (Vector4 d) {
		if (destructing) return;
		situation=Random.value;
		float damage=d.w*(100-armor)/100;
		float f=situation*(100-armor)/100;       
		damage+=f*damage;
		if (f>0.5f) {
			GameObject fr=Instantiate(ResLoad.fire,new Vector3(d.x,d.y,d.z),Quaternion.identity) as GameObject;
			fr.transform.parent=transform;}
		
		if (situation<0.001) {
			GameObject firex=Instantiate(ResLoad.fire,transform.position,transform.rotation) as GameObject;
			firex.transform.parent=transform;
			Vector3 itp=transform.InverseTransformPoint(new Vector3(d.x,d.y,d.z));
			firex.transform.localPosition=itp;
			firex.GetComponent<timer>().time=damage;
		}
		
		if (damage<hp) {hp-=(int)damage;} 
		if (damage==hp) {hp=0;Destruction();destructing=true;}
		if (damage>hp) {if (damage/hp>=10) ImmDestruction(); else Destruction();}
	}
	
	void Destruction () {
		destructing=true;
		byte g=(byte)(situation*10);        
		for (byte i=0;i<g;i++) {
			float l=Random.value;
			if (l>=0.5) l*=-1;   
			Instantiate(explosion,transform.position+transform.TransformDirection(new Vector3(0,0,l*length/2)),transform.rotation);
			StartCoroutine(FinalCoroutine(Random.value));
		}
	}

	IEnumerator FinalCoroutine(float k) {
		yield return new WaitForSeconds(k);
		Instantiate (wreck,transform.position,transform.rotation);
		Destroy(gameObject);
	}
	
	void ImmDestruction () {
		Instantiate(ResLoad.pieces,transform.position,transform.rotation);
		Destroy(gameObject);
	}
}
