using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BotControl : NetworkBehaviour {

	public float acceleration=1;
	public float angle_acceleration=1;
	float speed=0;
	public int goal=0;
	public int maxspeed=100;
	public int maxhp=10000;
	public float reaction=1;
	public int radarDistance=10000;
	public bool use_standart_start_position=true;
	public bool dontShoot=false;
	public float standartHeight=0;
	public Vector3 frontpoint;
	public char special_char;

	public byte am=0; //action mode;
	public byte prev_am=0; //saved state
	int longWay=0;
	public GameObject target;
	public Vector3 target_point;
	public Vector3 target_point2;
	public Vector3 target_point3;
	public bool use_second_tp=false;
	public bool use_third_tp=false;
	public GameObject nEnemy;
	int safe_distance;
	public Vector3 size;
	Rigidbody rbody;
	Quaternion rotateTo;
	Vector3 attitude;
	float rotation_time;
	bool rotating=false;
	bool rotate_left=false;
	bool rotate_right=false;

	public int hp;
	public GameObject explosion;
	public fleetCommand fc;
	public NetworkManager nm;
	bool free_operation=true;
	byte number;

	public byte attack_type=0;
	public byte attack_step=0;
	byte sidersReady=0; int sidersRange=0;
	byte tappsReady=0; int tappsRange=0;
	byte longForwardsReady=0; int lfRange=0;
	byte longsidersReady=0; int lsRange=0;
	public int maxRange=0;public int minRange=0;
	public byte command=2;
	public float bravery=0;
	bool under_attack=false;

	bool fire1=false;
	bool fire2=false;
	bool fire3=false;
	bool fire4=false;
	bool pieces=false;

	void Start () {
		if (size==Vector3.zero) size=gameObject.GetComponent<BoxCollider>().size;
		rbody=gameObject.GetComponent<Rigidbody>();
		if (rbody) rbody.mass=size.x*size.y*size.z;
		safe_distance=(int)(size.magnitude*0.55f);
		hp=maxhp;
		if (!explosion) explosion=ResLoad.explosion;
		if (isServer) {
		number=GameObject.Find("server_terminal").GetComponent<ServerTerminal>().GetSpawnPoint(command);
			if (use_standart_start_position) {
				if (command==1) {transform.position=new Vector3(number*100-500,0,-1000);} 
				else { transform.position=new Vector3(number*100-500,0,1000);transform.Rotate(new Vector3(0,180,0));	}
				RpcSetPosition(transform.position);
			}
		gameObject.name=number.ToString();
		if (gameObject.name.Length==1) {gameObject.name='0'+gameObject.name;}
		gameObject.name=command.ToString()+gameObject.name+'c';
		gameObject.name+='l';
			if (special_char=='\0') special_char='u';
			gameObject.name+=special_char;
		nm=GameObject.Find("networkManager").GetComponent<NetworkManager>();
			if (nm.numPlayers>1) RpcSetName(gameObject.name);
		}
		while (bravery==0) {bravery=Random.value;}
		if (isServer) {
			BroadcastMessage("ServerComponent",true,SendMessageOptions.DontRequireReceiver);
		}
		else {
			BroadcastMessage("ServerComponent",false,SendMessageOptions.DontRequireReceiver);
		}
		BroadcastMessage("BotControlled",true,SendMessageOptions.DontRequireReceiver);
	}

	[ClientRpc]
	void RpcSetName(string s) {
		if (!isServer) {gameObject.name.Remove(4);gameObject.name.Insert(4,"a");}
	}
	[ClientRpc]
	void RpcSetPosition (Vector3 pos) {
		if (!isServer) transform.position=pos;
	}
	[ClientRpc]
	void RpcLeftThrusters(bool x) {
		rotate_left=x;
	}
	[ClientRpc]
	void RpcRightThrusters (bool x) {
		rotate_right=x;
	}

	void StartGame () {
		standartHeight=transform.position.y;
		if (!isServer) return;
		fc=GameObject.Find("fc"+command).GetComponent<fleetCommand>();
		StartCoroutine(Controlling());
		if (lsRange<lfRange) maxRange=lsRange; else maxRange=lfRange;
		if (maxRange==0) maxRange=radarDistance/2;
		if (tappsRange<sidersRange) minRange=tappsRange; else minRange=sidersRange;
		if (minRange==0) minRange=maxRange/2;
	}

	IEnumerator Controlling () {
		RaycastHit cc;
		float brake_trace=speed*speed/angle_acceleration*2+safe_distance;
		if (Physics.SphereCast(transform.TransformPoint(frontpoint),safe_distance,transform.forward,out cc,brake_trace)&&!cc.collider.isTrigger) {
			if (am!=1) {prev_am=am;	am=1;}
		}
		else {
			if (am==1) {am=prev_am;
				if (rotate_right) {rotate_right=false;if (nm.numPlayers>1)  RpcRightThrusters(false);}
				if (rotate_left) {rotate_left=false;if (nm.numPlayers>1)  RpcLeftThrusters(false);}
			}
		}
		DECISION:
		switch (am) {
		case 0: //await
			if (speed>0) {goal=0;}
			if (!dontShoot) {nEnemy=fc.GetNearestEnemy(new Vector4(transform.position.x,transform.position.y,transform.position.z,radarDistance));
				if (nEnemy!=null) {am=4;goto DECISION;}}
			break;
		case 1: //maneuvering
			if (cc.collider==null) {if (prev_am!=1) am=prev_am; else am=0;goto DECISION;}
			GameObject g=cc.transform.root.gameObject;
			Rigidbody rg=g.GetComponent<Rigidbody>();
			Vector3 bp=transform.InverseTransformPoint(g.transform.position);
			Vector3 bd=Vector3.zero;
			if (rg) bd=transform.InverseTransformDirection(rg.velocity);
			if (bd.magnitude>10) {
					goal=0;
			}
			else {
				if (bp.x>0) {rotate_right=true;RpcRightThrusters(true);}
				if (bp.x<0) {rotate_left=true;RpcLeftThrusters(true);}
			}
			break;
		case 2: //moving
			if (target) {target_point=target.transform.position;target_point.y=standartHeight;}
			rotateTo=Quaternion.LookRotation(target_point-transform.position,Vector3.up);  
			float tan=Vector3.Angle(transform.forward,target_point-transform.position);
			rotation_time=tan/angle_acceleration;
			if (rotateTo.eulerAngles!=Vector3.zero) rotating=true;
			longWay=(int)Vector3.Distance(transform.position,target_point);
			if (speed*speed/2/acceleration+safe_distance<longWay&&transform.TransformPoint(target_point).z>0&&rotation_time<5) {goal=maxspeed;} else {if (use_second_tp) goal=maxspeed/2; else goal=0;}
			if (longWay<=safe_distance) {
				if (use_second_tp) {target_point=target_point2;if (use_third_tp) {use_third_tp=false;target_point2=target_point3;} else use_second_tp=false;}
				else {goal=0;am=0;}}
			break;
		case 3: //following
			if (!target) am=0;
			else {if (Vector3.Angle(transform.forward,target.transform.forward)<10) goal=(int)target.GetComponent<Rigidbody>().velocity.magnitude;	}
			break;
		case 4: //attack
			if (nEnemy==null||dontShoot) {am=0; break;}
			Vector3 itp=Vector3.zero;
			Vector3 itd=Vector3.zero;
			float rota=0;
			RaycastHit lt;
			float man=Vector3.Angle(Vector3.forward,new Vector3(itp.x,0,itp.z));
			itp=transform.InverseTransformPoint(nEnemy.transform.position);itp.y=0; if (nEnemy.transform.position.y<standartHeight) standartHeight-=50; else {if (nEnemy.transform.position.y>standartHeight) {standartHeight+=50;}}
			itd=transform.InverseTransformDirection(nEnemy.transform.forward);
			if (nEnemy.GetComponent<Rigidbody>()) itd=nEnemy.GetComponent<Rigidbody>().velocity;
			else itd=transform.InverseTransformDirection(nEnemy.transform.forward);
			float d=Vector3.Distance(transform.position,nEnemy.transform.position);
			switch (attack_type) {
			case 0:
				if (d>minRange) {
					attack_step=0;
					if (itp.z>=-1) {if (Quaternion.Angle(transform.rotation,rotateTo)<10) goal=maxspeed; else goal=0;}
					else {goal=10;}
				}
				else {
					goal=15;
					attack_step=1;
				}
				break;
			}
			break;
		}
		if (transform.forward*goal!=attitude) {
			attitude=transform.position;
			RpcSetAttitude(attitude,goal);}
		RpcMyPos(transform.position);
	//end of switch
		yield return new WaitForSeconds(reaction);
		StartCoroutine(Controlling());
		}
	[ClientRpc]
	void RpcSetAttitude(Vector3 a,int g) {
		if (isServer) return;
		attitude=a;
		goal=g;
	}
	[ClientRpc]
	void RpcMyPos (Vector3 pos) {
		if (isServer) return;
		if (pos!=transform.position&&(pos-transform.position).magnitude>5) {
			attitude=(attitude+(pos-transform.position).normalized).normalized;
		}
	}

	void Update () {
		switch (am) {
		case 3: 
			if (!target) am=0;
			else {
				Vector3 pospoint;
				if (free_operation) {pospoint=target.transform.position;}
				else {pospoint=target.transform.TransformPoint(target_point);}
				float d=Vector3.Distance(transform.position,pospoint);
				if (d>2*safe_distance+speed*speed/2/acceleration) {
					rotateTo=Quaternion.LookRotation(pospoint);
					if (rotateTo!=transform.rotation) rotating=true;
					if (Vector3.Angle(transform.forward,pospoint-transform.position)<15) goal=maxspeed;
				}
				else {
					rotateTo=target.transform.rotation;
					if (rotateTo.eulerAngles!=Vector3.zero) rotating=true;
				}
			}
			break;
		case 4:
			if (nEnemy==null) return;
			switch (attack_type) {
			case 0: 
				switch (attack_step) {
				case 0: rotateTo=Quaternion.LookRotation(nEnemy.transform.position-transform.position,Vector3.up);
				           if (rotateTo!=transform.rotation) {rotating=true;}
				break;
				case 1: 
					Vector3 itp=transform.InverseTransformPoint(nEnemy.transform.position);
					float rota=Vector2.Angle(Vector2.up,new Vector2(itp.x,itp.z));
					float rotz=Vector2.Angle(Vector2.up,new Vector2(itp.x,itp.y));
					if (itp.x>0) {rota-=90;rotz=90-rotz;} else {rota=90-rota;rotz-=90;}
					rotateTo=Quaternion.Euler(transform.rotation.eulerAngles+new Vector3(0,rota,rotz));
					if (rotateTo!=transform.rotation) {rotating=true;}
				break;
			}
				break;
			}
			break;
		}
		if (!isServer) {
			if (transform.forward!=attitude) {
				rotateTo=Quaternion.FromToRotation(transform.forward,attitude);rotating=true;
			}	
		}
		float t=angle_acceleration*Time.deltaTime;
		if (rotating==true) {			
			transform.rotation = Quaternion.RotateTowards(transform.rotation,rotateTo, t);
			if (transform.rotation==rotateTo) {
				rotating=false;
				}
		}
		else {
			if (rotate_right) {transform.Rotate(new Vector3(0,-t,0));}
			else {if (rotate_left) {transform.Rotate(new Vector3(0,t,0));}}
		}

		if (goal>maxspeed) goal=maxspeed;
		if (goal<-maxspeed/2) goal=-maxspeed/2;
		if (speed!=goal) {
			float a=goal-speed;
			if (Mathf.Abs(a)<acceleration*Time.deltaTime) speed=goal;
			else {
				if (a>0) speed+=acceleration*Time.deltaTime;
				else speed-=acceleration*Time.deltaTime;
			}
			if (rbody) rbody.velocity=transform.TransformDirection(new Vector3(0,0,speed));
			else transform.position+=transform.TransformDirection(new Vector3(0,0,speed*Time.deltaTime));
		}

		//stabilization
		Vector3 av=rbody.angularVelocity;
		if (av!=Vector3.zero) {
			if (Mathf.Abs(av.x)<=t) {av.x=0;}
			if (Mathf.Abs(av.y)<=t) {av.y=0;}
			if (Mathf.Abs(av.z)<=t) {av.z=0;}

			Vector3 rav=av;
			if (rav.x>0) {rav.x-=t;}
			if (rav.x<0) {rav.x+=t;}
			if (rav.y>0) {rav.y-=t;}
			if (rav.y<0) {rav.y+=t;}
			if (rav.z>0) {rav.z-=t;}
			if (rav.z<0) {rav.z+=t;}
			rbody.angularVelocity=rav;
		}
		av=transform.position;
		if (transform.position.y!=standartHeight) {
			if (Mathf.Abs(transform.position.y-standartHeight)<=t) {av.y=standartHeight;transform.position=av;}
			else {				
				if (transform.position.y>standartHeight) {av.y-=t;transform.position=av;}
				else {av.y+=t;transform.position=av;}
			}
		}
		av=transform.rotation.eulerAngles;
		bool r=false;
		if (av.x!=0||av.z!=0) {av.x=0;av.z=0;r=true;}
		if (r) transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.Euler(av),t);
	}
		
		
	float ellipseLongway (float t) {
		float sum=0;
		int x=(int)Mathf.Floor(t);
		t-=x;
		int l=0;
		while (l<=x) {
			sum+=(speed+l*acceleration)*(speed+l*acceleration);
			l++;
		}
		sum+=(speed+(l-1+t)*acceleration)*(speed+(l-1+t)*acceleration);
		return(Mathf.Sqrt(sum));
	}

	public void MyDistIs (Vector2 d) {
		byte g=(byte)d.y;
		switch (g) {
		case 0: if (tappsRange>d.x||tappsRange==0) tappsRange=(int)d.x;break;
		case 1: if (sidersRange>d.x||sidersRange==0) sidersRange=(int)d.x;break;
		case 2:if (lfRange>d.x||lfRange==0) lfRange=(int)d.x;break;
		case 3: if (lsRange>d.x||lsRange==0) lsRange=(int)d.x;break;
		}
	}

	bool isEnemy (GameObject x) {
		if (x==null) return (false);
		if (x.transform.root.gameObject.name[0]!=gameObject.name[0]&&x.transform.root.tag=="Player"&&x.transform.root!=transform.root) 
			return (true);
		else return (false);
		}

	bool isEnemy (Transform x) {
		if (x==null) return (false);
		//only transform roots in argument
		if (x.gameObject.name[0]!=gameObject.name[0]&&x.tag=="Player"&&x!=transform.root) 
			return (true);
		else return (false);
	}

	public void MoveTo(Vector3 point) {
		target_point=point;
		am=2;
		if (nm.numPlayers>1) RpcMoveTo(point);
	}
	[ClientRpc]
	void RpcMoveTo(Vector3 point) {
		if (isServer) return;
		target_point=point;
		am=2;
	}

	public void MoveAdd (Vector3 point) {
		if (am==2) {
			if (!use_second_tp) {target_point2=point;use_second_tp=true;}
			else {target_point3=point;use_third_tp=true;}
		}
		else {target_point=point;am=2;}
		if (nm.numPlayers>1) RpcMoveAdd(point);
	}
	[ClientRpc]
	void RpcMoveAdd (Vector3 point) {
		if (isServer) return;
		if (am==2) {
			if (!use_second_tp) {target_point2=point;use_second_tp=true;}
			else {target_point3=point;use_third_tp=true;}
		}
		else {target_point=point;am=2;}
	}

	public void SetLayer (int layer) {
		gameObject.layer=layer;
		gameObject.GetComponent<BoxCollider>().enabled=true;
	}

	public void Follow (Vector4 info) {
		if (!isServer) return;
		am=3;
		target_point=new Vector3(info.x,info.y,info.z);
		string number=((int)(info.w)).ToString();
		if (number.Length==1) number='0'+number;
		foreach (GameObject s in fc.ships) {
			if (s.name.Substring(1,2)==number) {target=s;break;}
		}
		free_operation=false;
		if (nm.numPlayers>1) RpcFollowInFormation(target,target_point);
	}
	public void Follow (GameObject x) {
		if (!isServer) return;
		am=3;
		target=x;
		free_operation=true;
		if (nm.numPlayers>1) RpcFollow(x);
	}
	public void FollowInCurrentFormation (GameObject x) {
		if (!isServer) return;
		am=3;
		target=x;
		target_point=transform.InverseTransformPoint(x.transform.position)*(-1);
		free_operation=false;
		if (nm.numPlayers>1) RpcFollowInFormation(target,target_point);
	}
	[ClientRpc]
	void RpcFollowInFormation (GameObject x,Vector3 pos) {
		if (isServer) return;
		target=x;
		target_point=pos;
		am=3;
		free_operation=false;
	}
	[ClientRpc]
	void RpcFollow (GameObject x) {
		if (isServer) return;
		target=x;
		free_operation=false;
		am=3;
	}
		
	void Destruction () {
		if (!explosion) explosion=ResLoad.explosion;
		GameObject e=Instantiate(explosion,transform.position,Quaternion.identity) as GameObject;
		e.SetActive(true);
		e=Instantiate(ResLoad.simple_wreck,transform.position,transform.rotation) as GameObject;
		e.transform.localScale=size;
		e.GetComponent<Rigidbody>().velocity=rbody.velocity;
		e.SetActive(true);
		if (nm.numPlayers>1) RpcDestruction();
		BroadcastMessage("Destroying",SendMessageOptions.DontRequireReceiver);
		Destroy(gameObject);
	}
	[ClientRpc]
	void RpcDestruction () {
		if (isServer) return;
		Destruction();
	}

	public void OnCollisionEnter (Collision c) {
		if (!isServer) return;
		Vector3 cpoint=transform.InverseTransformPoint(c.contacts[0].point);
		float impulse=0;
		Rigidbody rb=c.gameObject.GetComponent<Rigidbody>();
		if (rb) {
			impulse=rb.mass;
			if (rb.velocity.magnitude>1) {impulse*=rb.velocity.magnitude*Vector3.Angle(cpoint,Vector3.back)/90;}
		}
		else {
			if (c.gameObject.GetComponent<BoxCollider>()) {
				Vector3 s=c.gameObject.GetComponent<BoxCollider>().size;
				impulse=s.x*s.y*s.z*transform.localScale.x*transform.localScale.y*transform.localScale.z*rbody.velocity.magnitude;
				if (c.collider.tag=="Environment") {impulse*=10;}
			}
		}
		float damage=impulse-rbody.mass*rbody.velocity.magnitude;
		if (damage<=0) return;
		damage/=100;
		hp-=(int)damage;
		if (damage>1000) {GameObject p=Instantiate(ResLoad.pieces,transform.TransformPoint(cpoint),Quaternion.identity) as GameObject;p.SetActive(true);}
		if (hp<=0) Destruction();
	}

	public void	GunReady (int x) {
		switch (x) {
		case 0: sidersReady++;break;
		case 1: tappsReady++;break;
		case 2: longsidersReady++;break;
		case 3: longForwardsReady++;break;
		}
	}
	public void	GunShooted (int x) {
		switch (x) {
		case 0: sidersReady--;break;
		case 1: tappsReady--;break;
		case 2: longsidersReady--;break;
		case 3: longForwardsReady--;break;
		}
	}

	public void ApplyDamage (Vector4 point) {
		if (!isServer) return;
		Vector3 p=new Vector3(point.x,point.y,point.z);
		Vector3 x=transform.InverseTransformPoint(p);
		hp-=(int)point.w;
		if (hp<0) Destruction();
		else {
			if (hp<0.6f*maxhp) {
				GameObject f=null;
				bool a=false;
				if (!fire1) {a=true;fire1=true;f=GameObject.Find("menu").GetComponent<SceneResLoader>().EffectRequest(p,1);}
				if (hp<0.5f*maxhp&&!a) {
					if (!fire2) {a=true;fire2=true;f=GameObject.Find("menu").GetComponent<SceneResLoader>().EffectRequest(p,1);}
					if (hp<0.3f*maxhp&&!a) {
						if (!fire3) {a=true;fire3=true;f=GameObject.Find("menu").GetComponent<SceneResLoader>().EffectRequest(p,1);}
						if (hp<0.16f*maxhp&&!a) {
							if (!fire4) {a=true;fire4=true;f=Instantiate(Resources.Load<GameObject>("fire")) as GameObject;}
							if (!pieces) {pieces=true;GameObject pcs=GameObject.Find("menu").GetComponent<SceneResLoader>().EffectRequest(p,2);if (pcs) pcs.transform.parent=transform;}
						}
					}
				}
				if (a&&f!=null) {f.transform.parent=transform;f.transform.forward=transform.position-f.transform.position;f.transform.position=p;}
			}
			if (point.w>maxhp/10&&!pieces) {pieces=true;GameObject pcs=GameObject.Find("menu").GetComponent<SceneResLoader>().EffectRequest(p,2);if (pcs) pcs.transform.parent=transform;}
		}
		under_attack=true;
}
	}

