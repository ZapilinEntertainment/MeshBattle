using UnityEngine;
using System.Collections;

public class BotControl_crystal : MonoBehaviour {

	public float acceleration=1;
	public float angle_acceleration=1;
	public int speed=10;
	public int maxspeed=100;

	public GameObject[] guns;
	//0-fwd,1-right,2-left,3-up,4-down,5-back
	public bool[] ready;

	public int maxhp=10000;
	public float range=1000;
	public int damage=150;
	public float hp;
	public GameObject explosion;
	public GameObject small_explosion;
	public GameObject eyegun_pref;
	fleetCommand fc;
	char command='2';
	bool aggressive=false;
	public GameObject crystal_pref;
	GameObject[] hits;

	void Start () {
		hp=maxhp;
		ready=new bool[6];
		for (byte i=0;i<6;i++) {ready[i]=true;}
		hits=new GameObject[10];
	}

	void StartGame() {
		fc=GameObject.Find("fc"+command).GetComponent<fleetCommand>();
	}

	void Update () {
		transform.Translate(new Vector3(0,0,speed*Time.deltaTime));
		if (Vector3.Distance(transform.position,Global.cam.transform.position)>10000*transform.localScale.x) Destroy(gameObject);
	}

	void Targeting() {
		bool a=false;
		for (byte i=0; i<6;i++){
			if (!ready[i]) continue;
		Vector3 dir=Vector3.forward;
		switch (i) {
		case 1:dir=Vector3.right;break;
		case 2:dir=Vector3.left;break;
		case 3: dir=Vector3.up;break;
		case 4: dir=Vector3.down;break;
		case 5: dir=Vector3.back;break;
		}
		dir=transform.TransformDirection(dir);
			shootingSector s=new shootingSector(guns[i].transform.position,dir,range,90f);
		GameObject nEnemy=fc.GetNearestEnemy(s);
		if (nEnemy!=null) {
				GameObject g=Instantiate(eyegun_pref,guns[i].transform.position,Quaternion.LookRotation(dir)) as GameObject;
			g.GetComponent<eyegun>().target=nEnemy;
				g.GetComponent<eyegun>().damage=damage;
				g.transform.parent=transform;
				StartCoroutine(Awaiting(4));
				StartCoroutine(Reloading(i));
				a=true;
				break;
		}
		}
		if (!a) {	if (fc.GetNearestEnemy(new Vector4(transform.position.x,transform.position.y,transform.position.z,range*1.1f))==null) {aggressive=false;return;} else StartCoroutine(Awaiting(2));}
	}

	IEnumerator Awaiting(float x) {
		yield return new WaitForSeconds(x);
		Targeting();
	}
		
	IEnumerator Reloading (byte x) {
		ready[x]=false;
		yield return new WaitForSeconds(10);
		ready[x]=true;
	}

		
	void Destruction () {
		GameObject e=Instantiate(explosion,transform.position,Quaternion.identity) as GameObject;
		int step=(int)(transform.localScale.x/2);
		if (step>=1) {
			for (byte i=0;i<6;i++) {
				Vector3 pos=Vector3.zero;
				switch (i) {
				case 0: pos=new Vector3(0,0,8*step);break;
				case 1:pos=new Vector3 (0,0,-8*step);break;
				case 2:pos=new Vector3(-4*step,0,0);break;
				case 3:pos=new Vector3(4*step,0,0);break;
				case 4: pos = new Vector3(0,4*step,0);break;
				case 5: pos=new Vector3(0,-4*step,0);break;
				}
			GameObject c=Instantiate(crystal_pref,transform.TransformPoint(new Vector3(0,0,15)),transform.rotation) as GameObject;
			BotControl_crystal bcc=c.GetComponent<BotControl_crystal>();
			c.transform.localScale*=step*0.8f;
			bcc.damage=damage/2;
			bcc.maxhp=maxhp/2;
			bcc.range=range/2;
				bcc.crystal_pref=crystal_pref;
			}
		}
		Destroy(gameObject);
	}

	public void ApplyDamage (Vector4 x) {
		if (hp<0) return;
		hp-=x.w;
		if (hp<0) Destruction();
		else {
			Vector3 pos=new Vector3(x.x,x.y,x.z);
			byte ps=255;
			bool can=true;
			for (byte i=0;i<10;i++)
			{if (hits[i]==null) ps=i;
			else {if (Vector3.Distance(hits[i].transform.position,pos)<2) {can=false;break;}}
			}
			if (can&&ps!=255) {hits[ps]=Instantiate(small_explosion,new Vector3(x.x,x.y,x.z),Quaternion.identity) as GameObject;}
		}
	}

	public void OnCollisionEnter (Collision c) { 
		Vector3 cpoint=transform.InverseTransformPoint(new Vector3(c.contacts[0].point.x,c.contacts[0].point.y,c.contacts[0].point.z));
		float impulse=0;
		Rigidbody rb=c.gameObject.GetComponent<Rigidbody>();
		if (rb) {
			impulse=rb.mass;
			if (rb.velocity.magnitude>1) {impulse*=rb.velocity.magnitude*Vector3.Angle(cpoint,Vector3.back)/90;}
		}
		else {
			if (c.gameObject.GetComponent<BoxCollider>()) {
				Vector3 s=c.gameObject.GetComponent<BoxCollider>().size;
				impulse=s.x*s.y*s.z*transform.localScale.x*transform.localScale.y*transform.localScale.z*speed;
				if (c.collider.tag=="Environment") {impulse*=10;}
			}
		}
		float damage=impulse-1000*transform.localScale.x*speed*Time.deltaTime;
		if (damage<=0) return;
		damage/=100;
		ApplyDamage(new Vector4(c.contacts[0].point.x,c.contacts[0].point.y,c.contacts[0].point.z,damage));
	}
}
