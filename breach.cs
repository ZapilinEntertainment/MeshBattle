using UnityEngine; 
using System.Collections; 

public class breach : MonoBehaviour { 

	public float maxhp=500;
	float hp=500; 
	public float limit=1; 
	public bool further_breach=false; 
	public byte damage_degree=0; 
	public Vector3 size;
	Rigidbody rbody;

	void Start () { 
		if (damage_degree>0) further_breach=true; else further_breach=false;
		size=GetComponent<BoxCollider>().size;
		rbody=GetComponent<Rigidbody>();
		if (!rbody)  {rbody=gameObject.AddComponent<Rigidbody>();rbody.useGravity=false;}
		rbody.mass=size.x*transform.localScale.x*size.y*transform.localScale.y*size.z*transform.localScale.x;
		if (rbody.mass<10) {
			if (rbody.mass>1) {SimpleDamage sd=gameObject.AddComponent<SimpleDamage>();sd.maxhp=maxhp;gameObject.AddComponent<wreck_timer>();Destroy(this);}
			else {Destroy(gameObject);}
		}
		hp=maxhp;
	} 


	void ApplyDamage (Vector4 p) { 
		hp-=p.w; 
		if (hp>=0) return; 
		GetComponent<BoxCollider>().enabled=false;
		Vector3 point=transform.InverseTransformPoint(new Vector3(p.x,p.y,p.z)); 
		Vector3 axd=point; //дистанция до плоскостей
		if (point.x>0) {axd.x=size.x/2-point.x;} else {axd.x=size.x/2+point.x;}
		if (point.y>0) {axd.y=size.y/2-point.y;} else {axd.y=size.y/2+point.y;}
		if (point.z>0) {axd.z=size.z/2-point.z;} else {axd.z=size.z/2+point.z;}
		Vector3 pos1=Vector3.zero;
		Vector3 pos2=Vector3.zero;
		Vector3 size1=size;
		Vector3 size2=size;
		float rdist=0; 
		if (axd.x<axd.z) {
			if (axd.x<axd.y) {
				if (axd.y>axd.z) {
					//режем по z
					rdist=(size.y/2-axd.y);
					if (point.y>0) {pos1.y=rdist/2+point.y;pos2.y=(point.y+size.y/2)/2;size1.y=rdist;size2.y=point.y+size.y/2;}
					else {pos1.y=(axd.y+size.y/2)/2+point.y;pos2.y=point.y-rdist/2;size1.y=size.y-rdist;size2.y=rdist;}
				}
				else {
					//cut by oY
					rdist=(size.z/2-axd.z);
					if (point.z>0) {pos1.z=point.z+rdist/2;pos2.z=point.z-(size.z-rdist)/2;size1.z=rdist/2;size2.z=size.z-rdist;}
					else {pos1.z=point.z-rdist/2;pos2.z=point.z+(size.z-rdist)/2;size1.z=size.z-rdist;size2.z=rdist;}
				}
			}
			else {
				//в плоскости x-z
				rdist=size.x/2-axd.x;
				if (point.x>0) {pos1.x=point.x+rdist/2;pos2.x=point.x-(size.x-rdist)/2;size1.x=rdist;size2.x=size.x-rdist;}
				else {pos1.x=point.x-rdist/2;pos2.x=point.x+(size.x-rdist)/2;size1.x=rdist;size2.x=size.x-rdist;}
			}
		}
		else {
			if (axd.z<axd.y) {
				if (axd.x>axd.y) {
					//xyz
					rdist=size.y/2-axd.y;
					if (point.y>0) {pos1.y=point.y+rdist/2;pos2.y=point.y-(size.y-rdist)/2;size1.y=rdist/2;size2.y=size.y-rdist;}
					else {pos1.y=point.y+(size.y-rdist)/2;pos2.y=point.y-rdist/2;size1.y=size.y-rdist;size2.y=rdist;}
				}
				else {
					//yxz
					rdist=size.x-axd.x;
					if (point.x>0) {pos1.x=point.x+rdist/2;pos2.x=point.x-(size.x-rdist)/2;size1.x=rdist;size2.x=size.x-rdist;}
					else {pos1.x=point.x-rdist/2;pos2.x=point.x+(size.x-rdist)/2;size1.x=size.x-rdist;size2.x=rdist;}
				}
			}
			else {
				//xzy
				rdist=size.z-axd.z;
				if (point.z>0) {pos1.z=point.z+rdist/2;pos2.z=point.z-(size.z-rdist)/2;size1.z=rdist/2;size2.z=size.z-rdist;}
				else {pos1.z=point.z+(size.z-rdist)/2;pos2.z=point.z-rdist/2;size1.z=size.z-rdist;size2.z=rdist;}
			}
		}
		size1*=0.9f;size2*=0.9f;
		GameObject x;
		float kf=size1.x*size1.y*size1.z/(size.x*size.y*size.z);
		if (kf>=limit) {
			x=Instantiate(ResLoad.simple_wreck,transform.TransformPoint(pos1),transform.rotation) as GameObject;
			if (further_breach) {
				breach b=x.AddComponent<breach>(); b.damage_degree=(byte)(damage_degree-1);
				b.maxhp=kf*maxhp;
				b.limit=kf*limit;
			}
			Rigidbody rb=x.GetComponent<Rigidbody>();
			if (!rb) {rb=x.AddComponent<Rigidbody>();}
			rb.velocity=rbody.velocity*0.9f;
			x.transform.localScale=size1;
			x.SetActive(true);
		} 
		else  {GameObject l=Instantiate(ResLoad.pieces,transform.TransformPoint(pos1),Quaternion.identity)  as GameObject;l.SetActive(true);}

		kf=size2.x*size2.y*size2.z/(size.x*size.y*size.z);
		if (kf>=limit) {
			x=Instantiate(ResLoad.simple_wreck,transform.TransformPoint(pos2),transform.rotation)  as GameObject;
			if (further_breach) {
				
				breach b=x.AddComponent<breach>();
				b.damage_degree=(byte)(damage_degree-1);
				b.maxhp=kf*maxhp;
				b.limit=kf*limit;
			}
			Rigidbody rb=x.GetComponent<Rigidbody>();
			if (!rb) {rb=x.AddComponent<Rigidbody>();}
			rb.velocity=rbody.velocity*0.9f;
			x.transform.localScale=size2;
			x.SetActive(true);
		} 
		else {GameObject l=Instantiate(ResLoad.pieces,transform.TransformPoint(pos2),Quaternion.identity)  as GameObject;l.SetActive(true);}
		Destroy(gameObject);
	}


	public void OnCollisionEnter (Collision c) {
		float enemy_impulse=0;
		float damage=0;
		Rigidbody crbody=c.gameObject.GetComponent<Rigidbody>();
		BoxCollider baca=c.gameObject.GetComponent<BoxCollider>();
		if (crbody) {enemy_impulse=crbody.mass; if (crbody.velocity.magnitude>1) enemy_impulse*=crbody.velocity.magnitude;}
		else {enemy_impulse=baca.size.x*baca.size.z*baca.size.y;}
		if (rbody==null) {rbody=GetComponent<Rigidbody>();if (!rbody) gameObject.AddComponent<Rigidbody>();rbody.mass=size.x*size.y*size.z;}
		damage=rbody.mass;
		if (rbody.velocity.magnitude>1) {damage*=rbody.velocity.magnitude;}
		damage-=enemy_impulse;
		if (damage<=0) return;
		Vector3 cpoint=c.contacts[0].point;
		ApplyDamage(new Vector4(cpoint.x,cpoint.y,cpoint.z,damage));
		if (damage>1000) {GameObject l=Instantiate(ResLoad.pieces,transform.TransformPoint(cpoint),Quaternion.identity) as GameObject;l.SetActive(true);}
	}
}