using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkShipController : NetworkBehaviour {

	//engine
	public byte engine_mode=0;
	byte prev_emode=0;
	int maxspeed;
	[SyncVar]
	public float speed=0;
	public int goal = 0;
	public byte speed_state=1;
	public float va; //main engine acceleration
	public float aa; //rotation speed;
	int engine_consumption=100;
	Vector3 mvector=Vector3.zero;
	public bool tn_right = false;
	public bool tn_left=false;
	public bool rt_left=false;
	public bool rt_right=false;
	public bool upto=false;
	public bool downto=false;
	public float standartHeight=0;
	public bool engine_working=true;

	//reactor
	[SyncVar]
	public float capacity;
	public int constant_supply;
	public int goal_capacity;
	public bool reactor_overloaded=false;
	float restore_speed=10;
	public float reactor_bonus=1;
	float time_left;
	float reactor_ol_time=30; //OverLoading
	public float reactor_ol_timeleft=0;
	float reactor_cd=300;
	public float reactor_cd_timeleft=0;

	//setting values
	public float reactor_restoring_value=10;
	public int reactor_normal_capacity=1000;
	public int reactor_overloaded_capacity=4000;
	public int engine_consumption_value=100;
	public int engine_maxspeed1=15;
	public int engine_maxspeed2=40;
	public int engine_maxspeed3=400;
	public float march_acceleration=0.6f;
	public float maneuver_acceleration=1f;
	public int maxhp=20000;
	public int radar_distance=5000;
	public float hp;
	int max_dps;
	float lost_hp=0;
	bool underfire=false;
	bool dead=false;
	float respawn_time=30.0f;
	public shield myShield;
	//components
	Rigidbody rbody;
ServerTerminal st;
	public GameObject cameraPrefab;
	GameObject cam;
	public GameObject[] modules;

	public char command='0';
	public char special_char;
	public GameObject[] enemies;
	public byte[] zonal;
	public bool[] presence;
	public GameObject[] marked_targets;
	public ushort close_distance=300;
	public uint far_distance=1000;
	public SceneResLoader srl;

	public float va_bonus=1;
	public float aa_bonus=1;
	public float weapons_bonus=1f;
	public float radar_bonus=1f;
	public float respawn_bonus=1f;
	public float shield_bonus=1f;
	public float armor_bonus=1f;
	public float nose_armor=0;
	public float bottom_armor=0;
	public float leftwing_armor=0;
	public float rightwing_armor=0;

	public bool shields_on=false; 
	public bool taran_cover=false;
	public bool tapps=false;
	bool second_mass_calculation=false;

	public bool armor_on;
	public int armor_consumption=0;
	bool fire1=false;
	bool fire2=false;
	bool fire3=false;
	bool fire4=false;

	public BoxCollider c1;
	public BoxCollider c2;
	public BoxCollider c3;
	public BoxCollider c4; //4-fin,6-leftwing,5-rightwing
	public BoxCollider c5;
	public BoxCollider c6;
	public GameObject ship;

	Texture reactor_tx;
	Texture shield_tx;
	Texture ind_green;
	Texture ind_red;


	void Awake () {modules= new GameObject[28];max_dps=maxhp/10;}

	void Start () {
		switch (engine_mode) {
		case 0: maxspeed=(int)(engine_maxspeed1*va_bonus);va=march_acceleration;aa=maneuver_acceleration*aa_bonus;engine_consumption=engine_consumption_value;prev_emode=0;break;
		case 1:maxspeed=(int)(engine_maxspeed2*va_bonus);va=march_acceleration*4;aa=maneuver_acceleration*1.2f*aa_bonus;engine_consumption=engine_consumption_value*3;prev_emode=1;break;
		case 2:maxspeed=(int)(engine_maxspeed3*va_bonus);va=march_acceleration*16;aa=maneuver_acceleration*0.8f*aa_bonus;engine_consumption=engine_consumption_value*9;prev_emode=2;break;
		}
		reactor_overloaded=false;
		rbody = gameObject.GetComponent<Rigidbody> ();
		if (GetComponent<NetworkIdentity>().isLocalPlayer) {
			NetworkShipGUI nsg=gameObject.AddComponent<NetworkShipGUI>();
			nsg.nsc=this;
			st=GameObject.Find("server_terminal").GetComponent<ServerTerminal>();
			cam=Instantiate(cameraPrefab,transform.position,transform.rotation) as GameObject;
			cam.GetComponent<camera_control>().ship=gameObject;
			Global.cam=cam.GetComponent<camera_control>().cam;
		}
		ship.SetActive(false);
		GameObject s=Instantiate(Resources.Load<GameObject>("nms_shield"),transform.position,transform.rotation) as GameObject;
		s.transform.parent=transform;
		myShield=s.GetComponent<shield>();
		myShield.nsc=this;
		s.SetActive(false);
		srl=GameObject.Find("menu").GetComponent<SceneResLoader>();
		if (isLocalPlayer) srl.localShip=gameObject;
	}
		


	public void StartGame () {
		c1.enabled=true;
		c2.enabled=true;
		c3.enabled=true;
		hp=maxhp;
		gameObject.tag="Player";
		if (dead) {
			foreach (GameObject module in modules) {
				if (module!=null) module.SetActive(true);
			}
			ship.SetActive(true);
			dead=false;
			reactor_overloaded=false;
			capacity=0;
			shields_on=false;
			speed_state=1;
			engine_mode=0;maxspeed=engine_maxspeed1;prev_emode=0;speed=0;goal=0;
			engine_working=true;
			Scan();
			if (c4) c4.enabled=true;
			if (c5) c5.enabled=true;
			if (c6) c6.enabled=true;
		}
		else {
			rbody.mass=c1.size.x*c1.size.y*c1.size.z+c2.size.x*c2.size.y*c2.size.z+c3.size.x*c3.size.y*c3.size.z;
			if (c4) {rbody.mass+=c4.size.x*c4.size.y*c4.size.z;}
			if (c5) {rbody.mass+=c5.size.x*c5.size.y*c5.size.z;}
			if (c6) {rbody.mass+=c6.size.x*c6.size.y*c6.size.z;}
		}
		myShield.name=command+"shield";
		Scan();
	}
		

	void Update () {
		if (dead) {if (speed>0) {float f=speed-10*Time.deltaTime;transform.Translate(new Vector3(0,0,f));speed-=f;} return;}
		if (reactor_overloaded) {if (reactor_ol_timeleft>0) {reactor_ol_timeleft-=Time.deltaTime;if (reactor_ol_timeleft<=0) {if (isServer) {RpcReactorState(false);} reactor_ol_timeleft=0;}}}
		else {if (reactor_cd_timeleft>0) {reactor_cd_timeleft-=Time.deltaTime;if (reactor_cd_timeleft<=0) {reactor_cd_timeleft=0;}}}
		float t = Time.deltaTime;
		float a=0;
		if (!reactor_overloaded) {goal_capacity=(int)(reactor_normal_capacity*reactor_bonus);restore_speed=reactor_restoring_value*reactor_bonus;}
		else {goal_capacity=(int)(reactor_overloaded_capacity*reactor_bonus);restore_speed=reactor_restoring_value*reactor_restoring_value*reactor_bonus;}
		goal_capacity-=constant_supply+engine_consumption;
		if (goal_capacity!=capacity) {
			a=goal_capacity-capacity;
			if (Mathf.Abs(a)<=t*restore_speed) {capacity=goal_capacity;}
			else {if (a>0) {capacity+=t*restore_speed;}
			else {capacity-=t*reactor_restoring_value;}}
		}
		if (engine_working) {
			if (prev_emode!=engine_mode) {
				switch (engine_mode) {
				case 0: maxspeed=(int)(engine_maxspeed1*va_bonus);va=march_acceleration;aa=maneuver_acceleration*aa_bonus;engine_consumption=engine_consumption_value;prev_emode=0;break;
				case 1:maxspeed=(int)(engine_maxspeed2*va_bonus);va=march_acceleration*4;aa=maneuver_acceleration*1.2f*aa_bonus;engine_consumption=engine_consumption_value*3;prev_emode=1;break;
				case 2:maxspeed=(int)(engine_maxspeed3*va_bonus);va=march_acceleration*16;aa=maneuver_acceleration*0.8f*aa_bonus;engine_consumption=engine_consumption_value*9;prev_emode=2;break;
				}
			}
			switch (speed_state) {
			case 0: goal=-maxspeed/4;break;
			case 1:goal=0;break;
			case 2:goal=maxspeed/2;break;
			case 3:goal=maxspeed;break;
			}
			if (goal > maxspeed)	goal =maxspeed;
			if (goal < -1*maxspeed/2) 	goal = -1*maxspeed/2;
			a = goal - speed;

			if (a != 0) {
				if (Mathf.Abs (a)<= va) {speed=goal;}
				else {if (a>0) {speed+=va*Time.deltaTime;} else {speed-=va*t;}}
			}
				
			t = Time.deltaTime*aa; //now this is the rotation speed
			if (tn_left) {transform.Rotate(0,-t,0);}
			if (tn_right) {transform.Rotate(0,t,0);}

			mvector = transform.TransformDirection(new Vector3(0,0,speed));

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
		if (rbody.velocity != mvector) rbody.velocity=mvector;

		if (isServer) {
			if (transform.position.magnitude>srl.map_radius) {
				RpcMakeMeDead(7);
				Instantiate(srl.hyperjump_exit_effect,transform.position,transform.rotation);
			}
			if (shields_on&&capacity<=0) {CmdShield(false);}
		}
	}

	public void Scan() {
		GameObject[] ships=GameObject.FindGameObjectsWithTag("Player");
		if (ships.Length!=0) {
			int x=0;
			foreach (GameObject pred_enemy in ships) {
				if (pred_enemy.name[0]!=command&&Vector3.Distance(pred_enemy.transform.position,transform.position)<radar_distance*radar_bonus) x++;
			}
			GameObject[] real_enemies=new GameObject[x];
			byte f=0;
			for (byte z=0;z<x;z++) {
				if (ships[z].name[0]!=command&&Vector3.Distance(ships[z].transform.position,transform.position)<radar_distance*radar_bonus&&ships[z]!=null) {real_enemies[f]=ships[z];f++;}
			}
			enemies=real_enemies;
		zonal=new byte[enemies.Length];
		presence=new bool[12];
		for (byte i=0;i<enemies.Length;i++) {
				if (enemies[i]==null) continue;
			Vector3 epos=transform.InverseTransformPoint(enemies[i].transform.position);
			byte a=0;
			if (epos.z>1) {
				if (epos.x<0) {
					if (epos.y>-3) {
						if (epos.y>3) {a=1;presence[0]=true;}
						else {a=2;presence[1]=true;}
					}
					else {a=3;presence[2]=true;}
				}
				else {
					if (epos.y>-3) {
						if (epos.y>3) {a=4;presence[3]=true;}
						else {a=5;presence[4]=true;}
					}
					else {a=6;presence[5]=true;}
				}
			}
			else {
				if (epos.z>-13) {
					if (epos.x<0) {
						if (epos.y>-3) {
							if (epos.y>3) {a=7;presence[6]=true;}
							else {a=8;presence[7]=true;}
						}
						else {a=9;presence[8]=true;}
					}
					else {
						if (epos.y>-3) {
							if (epos.y>3) {a=10;presence[9]=true;}
							else {a=11;presence[10]=true;}
						}
						else {a=12;presence[11]=true;}
					}
				}
			}
				
			if (Vector3.Magnitude(epos)<far_distance) {
				if (Vector3.Magnitude(epos)>close_distance) {a+=200;} else {a+=100;}
			}
			zonal[i]=a;
		}
		if (isServer) {
			for (byte i=0;i<marked_targets.Length;i++) {
				if (!marked_targets[i]) {CmdRemoveTargetFromList(i);}
				else {if (Vector3.Distance(marked_targets[i].transform.position,transform.position)>radar_distance*radar_bonus) {marked_targets[i].GetComponent<marker>().RemoveSelection();CmdRemoveTargetFromList(i);}}
			}
		}
		}
		else {if (marked_targets.Length!=0) {CmdClearMarkedTargetsList();}}
		StartCoroutine(WaitForScan());
	}

	 int EnemyCountScan() {
		GameObject[] e=GameObject.FindGameObjectsWithTag("Player");
		byte c=0;
		foreach (GameObject ship in e) {
			if (ship.name[0]!=command) c++;
		}
		return(c);
	}

	IEnumerator WaitForScan () {
		yield return new WaitForSeconds(3);
		if (!dead) {Scan();}
	}

	[Command]
	void CmdClearMarkedTargetsList() {
		if (EnemyCountScan()==0) RpcClearMarkedTargetsList();
	}
	[ClientRpc]
	void RpcClearMarkedTargetsList() {
		marked_targets=new GameObject[0];
	}

	[ClientRpc]
	public void RpcSetPosition (Vector3 a) {
		transform.position=a;
	}

	[Command]
	public void CmdAddTargetToList(GameObject target) {
		if (marked_targets.Length!=0) {
			GameObject[] bt=new GameObject[marked_targets.Length+1];
			for (byte j=0;j<marked_targets.Length;j++) {
				bt[j]=marked_targets[j];
			}
			bt[marked_targets.Length]=target;
			marked_targets=new GameObject[marked_targets.Length+1];
			marked_targets=bt;
		}
		else {marked_targets=new GameObject[1];
			marked_targets[0]=target;
		}
		RpcAddTargetToList(target);
	}

	[ClientRpc]
	public void RpcAddTargetToList (GameObject target) {
		if (isServer) return;
		if (marked_targets.Length!=0) {
			GameObject[] bt=new GameObject[marked_targets.Length+1];
			for (byte j=0;j<marked_targets.Length;j++) {
				bt[j]=marked_targets[j];
			}
			bt[marked_targets.Length]=target;
			marked_targets=new GameObject[marked_targets.Length+1];
			marked_targets=bt;
		}
		else {marked_targets=new GameObject[1];
			marked_targets[0]=target;
		}
	}

	[Command]
	public void CmdRemoveTargetFromList (byte x) {
		if (marked_targets.Length!=1) {
			GameObject[] bt=new GameObject[marked_targets.Length-1];
			byte j=0;
			for (byte i=0;i<marked_targets.Length-1;i++) {
				if (i!=x) {bt[i]=marked_targets[j];j++;}
				else {bt[i]=marked_targets[j+1];j+=2;}

			}
			marked_targets=new GameObject[marked_targets.Length-1];
			marked_targets=bt;
		}
		else {marked_targets=new GameObject[0];}
		RpcRemoveTargetFromList(x);
	}

	[ClientRpc]
	public void RpcRemoveTargetFromList (byte x) {
		if (isServer) return;
		if (marked_targets.Length!=1) {
			GameObject[] bt=new GameObject[marked_targets.Length-1];
			byte j=0;
			for (byte i=0;i<marked_targets.Length-1;i++) {
				if (i!=x) {bt[i]=marked_targets[j];j++;}
				else {bt[i]=marked_targets[j+1];j+=2;}
			}
			marked_targets=new GameObject[marked_targets.Length-1];
			marked_targets=bt;
		}
		else {marked_targets=new GameObject[0];}
	}

	[Command]
	public void CmdReactorState(bool x) {
		RpcReactorState(x);
	}
	[ClientRpc]
	public void RpcReactorState (bool x) {
		reactor_overloaded=x;
		if (x) {goal_capacity=(int)(4000*reactor_bonus);reactor_ol_timeleft=reactor_ol_time*reactor_bonus;} 
		else {goal_capacity=(int)(1000*reactor_bonus);reactor_cd_timeleft=(reactor_ol_time*reactor_bonus-reactor_ol_timeleft)*3;}
	}

	[Command]
	public void CmdChangeSpeedState(byte a) {
		if (a!=speed_state) {speed_state=a;RpcChangeSpeedState(a);}
	}
	[ClientRpc]
	void RpcChangeSpeedState(byte a) {
		if (GetComponent<NetworkIdentity>().isServer) return;
		speed_state=a;
	}
	[Command]
	public void CmdSpeedChange (bool up) {
		if (up) {speed_state++;} else {speed_state--;}
		RpcSpeedChange(up);
	}
	[ClientRpc]
	public void RpcSpeedChange (bool up) {
		if (GetComponent<NetworkIdentity>().isServer) return;
		if (up) {speed_state++;} else {speed_state--;}
	}
	[Command]
	public void CmdChangeEngineMode (byte a) {
		if (a!=engine_mode) {engine_mode=a;RpcChangeEngineMode(a);}
	}
	[ClientRpc]
	void RpcChangeEngineMode (byte a) {
		engine_mode=a;
	}

	[Command]
	public void CmdLiftDown () {
		downto=!downto;
		RpcLiftDown();
		if (downto) upto=false;
	}
	[ClientRpc]
	void RpcLiftDown () {
		if (GetComponent<NetworkIdentity>().isServer) return;
		downto=!downto;
		if (downto) upto=false;
	}
	[Command]
	public void CmdLiftUp () {
		upto=!upto;
		RpcLiftUp();
		if (upto) downto=false;
	}
	[ClientRpc]
	void RpcLiftUp () {
		if (GetComponent<NetworkIdentity>().isServer) return;
		upto=!upto;
		if (upto) downto=false;
	}
	[Command]
	public void CmdRtLeft () {
		rt_left=!rt_left;
		RpcRtLeft();
		if (rt_left) rt_right=false;
	}
	[ClientRpc]
	void RpcRtLeft () {
		if (GetComponent<NetworkIdentity>().isServer) return;
		rt_left=!rt_left;
		if (rt_left) rt_right=false;
	}
	[Command]
	public void CmdRtRight () {
		rt_right=!rt_right;
		RpcRtRight();
		if (rt_right) rt_left=false;
	}
	[ClientRpc]
	void RpcRtRight () {
		if (GetComponent<NetworkIdentity>().isServer) return;
		rt_right=!rt_right;
		if (rt_right) rt_left=false;
	}
	[Command]
	public void CmdTnRight () {
		tn_right=!tn_right;
		RpcTnRight();
		if (tn_right) tn_left=false;
	}
	[ClientRpc]
	void RpcTnRight () {
		if (GetComponent<NetworkIdentity>().isServer) return;
		tn_right=!tn_right;
		if (tn_right) tn_left=false;
	}
	[Command]
	public void CmdTnLeft () {
		tn_left=!tn_left;
		RpcTnLeft();
		if (tn_left) tn_right=false;
	}
	[ClientRpc]
	void RpcTnLeft () {
		if (GetComponent<NetworkIdentity>().isServer) return;
		tn_left=!tn_left;
		if (tn_left) tn_right=false;
	}

	[Command]
	public void CmdFireLeft() {
		laser_caster lc;
		longcaster lgc;pulser pc;pointpulser ppc;
		byte i=0;
		short x=-1;
		for (i=1;i<7;i++) {
			if (modules[i]==null) continue;
			lc=modules[i].GetComponent<laser_caster>();
			lgc=modules[i].GetComponent<longcaster>();
			pc=modules[i].GetComponent<pulser>();
			ppc=modules[i].GetComponent<pointpulser>();
			if (lc) {	x=lc.Focus(); if (x>=0&&!lc.forward) {RpcFire(i,x);}	}
			if (lgc) {if (lgc.ready&&!lgc.forward) {RpcFire(i,-2);}	}
			if (pc) {if (pc.ready&&capacity>=pc.energy&&!pc.forward) {RpcFire(i,-2);}	}
			if (ppc) {	if (ppc.ready&&capacity>=ppc.energy&&!ppc.forward) {RpcFire(i,ppc.Focus());}	}
		}
		for (i=13;i<19;i++) {
			if (modules[i]==null) continue;
			lc=modules[i].GetComponent<laser_caster>();
			lgc=modules[i].GetComponent<longcaster>();
			pc=modules[i].GetComponent<pulser>();
			ppc=modules[i].GetComponent<pointpulser>();
			if (lc) {	x=lc.Focus(); if (x>=0&&!lc.forward) {RpcFire(i,x);}	}
			if (lgc) {if (lgc.ready&&!lgc.forward) {RpcFire(i,-2);}	}
			if (pc) {if (pc.ready&&capacity>=pc.energy&&!pc.forward) {RpcFire(i,-2);}	}
			if (ppc) {	if (ppc.ready&&capacity>=ppc.energy&&!ppc.forward) {RpcFire(i,ppc.Focus());}	}
		}
	}
	[Command]
	public void CmdFireRight () {
		byte i=7;
		short x=-1;
		laser_caster lc;
		longcaster lgc;pulser pc;pointpulser ppc;
		for (i=7;i<13;i++) {
			if (modules[i]==null) continue;
			lc=modules[i].GetComponent<laser_caster>();
			lgc=modules[i].GetComponent<longcaster>();
			pc=modules[i].GetComponent<pulser>();
			ppc=modules[i].GetComponent<pointpulser>();
			if (lc) {	x=lc.Focus(); if (x>=0&&!lc.forward) {RpcFire(i,x);}	}
			if (lgc) {if (lgc.ready&&!lgc.forward) {RpcFire(i,-2);}	}
			if (pc) {if (pc.ready&&capacity>=pc.energy&&!pc.forward) {RpcFire(i,-2);}	}
			if (ppc) {	if (ppc.ready&&capacity>=ppc.energy&&!ppc.forward) {RpcFire(i,ppc.Focus());}	}
		}
		for (i=19;i<25;i++) {
			if (modules[i]==null) continue;
			lc=modules[i].GetComponent<laser_caster>();
			lgc=modules[i].GetComponent<longcaster>();
			pc=modules[i].GetComponent<pulser>();
			ppc=modules[i].GetComponent<pointpulser>();
			if (lc) {	x=lc.Focus(); if (x>=0&&!lc.forward) {RpcFire(i,x);}	}
			if (lgc) {if (lgc.ready&&!lgc.forward) {RpcFire(i,-2);}	}
			if (pc) {if (pc.ready&&capacity>=pc.energy&&!pc.forward) {RpcFire(i,-2);}	}
			if (ppc) {	if (ppc.ready&&capacity>=ppc.energy&&!ppc.forward) {RpcFire(i,ppc.Focus());}	}
		}
	}
	[Command]
	public void CmdFireRightFwd() {
		laser_caster lc;
		longcaster lgc;pulser pc;pointpulser ppc;
		byte i=7;
		short x=-1;
		for (i=7;i<13;i++) {
			if (modules[i]==null) continue;
			lc=modules[i].GetComponent<laser_caster>();
			lgc=modules[i].GetComponent<longcaster>();
			pc=modules[i].GetComponent<pulser>();
			ppc=modules[i].GetComponent<pointpulser>();
			if (lc) {	x=lc.Focus(); if (x>=0&&lc.forward) {RpcFire(i,x);}	}
			if (lgc) {if (lgc.ready&&lgc.forward&&lgc.eps<capacity) {RpcFire(i,-2);}	}
			if (pc) {if (pc.ready&&capacity>=pc.energy&&pc.forward&&pc.energy<capacity) {RpcFire(i,-2);}	}
			if (ppc) {	if (ppc.ready&&capacity>=ppc.energy&&ppc.forward&&ppc.energy<capacity) {RpcFire(i,ppc.Focus());}	}
		}
		for (i=19;i<25;i++) {
			if (modules[i]==null) continue;
			lc=modules[i].GetComponent<laser_caster>();
			lgc=modules[i].GetComponent<longcaster>();
			pc=modules[i].GetComponent<pulser>();
			ppc=modules[i].GetComponent<pointpulser>();
			if (lc) {	x=lc.Focus(); if (x>=0&&lc.forward) {RpcFire(i,x);}	}
			if (lgc) {if (lgc.ready&&lgc.forward&&lgc.eps<capacity) {RpcFire(i,-2);}	}
			if (pc) {if (pc.ready&&capacity>=pc.energy&&pc.forward&&pc.energy<capacity) {RpcFire(i,-2);}	}
			if (ppc) {	if (ppc.ready&&capacity>=ppc.energy&&ppc.forward&&ppc.energy<capacity) {RpcFire(i,ppc.Focus());}	}
		}
		if (modules[27]) {
			lc=modules[27].GetComponent<laser_caster>();
			lgc=modules[27].GetComponent<longcaster>();
			pc=modules[27].GetComponent<pulser>();
			ppc=modules[27].GetComponent<pointpulser>();
			if (lc) {	x=lc.Focus(); if (x>=0&&lc.forward) {RpcFire(27,x);}	}
			if (lgc) {if (lgc.ready&&lgc.forward&&lgc.eps<capacity) {RpcFire(27,-2);}	}
			if (pc) {if (pc.ready&&capacity>=pc.energy&&pc.forward&&pc.energy<capacity) {RpcFire(27,-2);}	}
			if (ppc) {	if (ppc.ready&&capacity>=ppc.energy&&ppc.forward&&ppc.energy<capacity) {RpcFire(27,ppc.Focus());}	}
		}
	}

	[Command]
	public void CmdFireLeftFwd() {
		laser_caster lc;
		longcaster lgc;pulser pc;pointpulser ppc;
		byte i=1;
		short x=-1;
		for (i=1;i<7;i++) {
			if (modules[i]==null) continue;
			lc=modules[i].GetComponent<laser_caster>();
			lgc=modules[i].GetComponent<longcaster>();
			pc=modules[i].GetComponent<pulser>();
			ppc=modules[i].GetComponent<pointpulser>();
			if (lc) {	x=lc.Focus(); if (x>=0&&lc.forward) {RpcFire(i,x);}	}
			if (lgc) {if (lgc.ready&&lgc.forward&&lgc.eps<capacity) {RpcFire(i,-2);}	}
			if (pc) {if (pc.ready&&capacity>=pc.energy&&pc.forward&&pc.energy<capacity) {RpcFire(i,-2);}	}
			if (ppc) {	if (ppc.ready&&capacity>=ppc.energy&&ppc.forward&&ppc.energy<capacity) {RpcFire(i,ppc.Focus());}	}
		}
		for (i=13;i<19;i++) {
			if (modules[i]==null) continue;
			lc=modules[i].GetComponent<laser_caster>();
			lgc=modules[i].GetComponent<longcaster>();
			pc=modules[i].GetComponent<pulser>();
			ppc=modules[i].GetComponent<pointpulser>();
			if (lc) {	x=lc.Focus(); if (x>=0&&lc.forward) {RpcFire(i,x);}	}
			if (lgc) {if (lgc.ready&&lgc.forward&&lgc.eps<capacity) {RpcFire(i,-2);}	}
			if (pc) {if (pc.ready&&capacity>=pc.energy&&pc.forward&&pc.energy<capacity) {RpcFire(i,-2);}	}
			if (ppc) {	if (ppc.ready&&capacity>=ppc.energy&&ppc.forward&&ppc.energy<capacity) {RpcFire(i,ppc.Focus());}	}
		}
		if (modules[25]) {
			lc=modules[25].GetComponent<laser_caster>();
			lgc=modules[25].GetComponent<longcaster>();
			pc=modules[25].GetComponent<pulser>();
			ppc=modules[25].GetComponent<pointpulser>();
			if (lc) {	x=lc.Focus(); if (x>=0&&lc.forward) {RpcFire(25,x);}	}
			if (lgc) {if (lgc.ready&&lgc.forward&&lgc.eps<capacity) {RpcFire(25,-2);}	}
			if (pc) {if (pc.ready&&capacity>=pc.energy&&pc.forward&&pc.energy<capacity) {RpcFire(25,-2);}	}
			if (ppc) {	if (ppc.ready&&capacity>=ppc.energy&&ppc.forward&&ppc.energy<capacity) {RpcFire(25,ppc.Focus());}	}
		}
	}


	[ClientRpc]
	public void RpcFire (byte index,short target_index) {
		// -1 - no target, -2 - gun need not a target
		if (target_index>=0) {
			if (modules[index].GetComponent<laser_caster>()) modules[index].GetComponent<laser_caster>().Fire((byte)(target_index));
			if (modules[index].GetComponent<pointpulser>()) modules[index].GetComponent<pointpulser>().Fire((byte)(target_index));
		}
		else {
		if (target_index==-2) modules[index].SendMessage("Fire",SendMessageOptions.DontRequireReceiver);
			if (target_index==-3) modules[index].SendMessage("Torpedo",SendMessageOptions.DontRequireReceiver);
		}
	}



	[Command]
	public void CmdFireForwards () {
		GameObject x=modules[0];
		short y=-1;
		if (x!=null) {
			if (x.GetComponent<longcaster>()) {if (x.GetComponent<longcaster>().eps<capacity&&x.GetComponent<longcaster>().ready) {RpcFire(0,-2);}}
			else {
				if (x.GetComponent<tapp>()) {if (x.GetComponent<tapp>().ready) {RpcFire(0,-3);}}
				else {
					if (x.GetComponent<laser_caster>()) {y=x.GetComponent<laser_caster>().Focus();if (x.GetComponent<laser_caster>().eps<capacity&&y>=0) {RpcFire(0,y);}}
					else {
						if (x.GetComponent<pulser>()) {if (x.GetComponent<pulser>().energy<capacity&&x.GetComponent<pulser>().ready) {RpcFire(0,-2);}}
						else {
							if (x.GetComponent<pointpulser>()) {y=x.GetComponent<pointpulser>().Focus();if (x.GetComponent<pointpulser>().energy<capacity&&y>=0) {RpcFire(0,y);}}
						}
					}
				}
			}
		}
		x=modules[26]; y=-1;
		if (x!=null) {
			if (x.GetComponent<longcaster>()) {if (x.GetComponent<longcaster>().eps<capacity&&x.GetComponent<longcaster>().ready) {RpcFire(26,-2);}}
			else {
				if (x.GetComponent<tapp>()) {if (x.GetComponent<tapp>().ready) {RpcFire(26,-3);}}
				else {
					if (x.GetComponent<laser_caster>()) {y=x.GetComponent<laser_caster>().Focus();if (x.GetComponent<laser_caster>().eps<capacity&&y>=0) {RpcFire(26,y);}}
					else {
						if (x.GetComponent<pulser>()) {if (x.GetComponent<pulser>().energy<capacity&&x.GetComponent<pulser>().ready) {RpcFire(26,-2);}}
						else {
							if (x.GetComponent<pointpulser>()) {y=x.GetComponent<pointpulser>().Focus();if (x.GetComponent<pointpulser>().energy<capacity&&y>=0) {RpcFire(26,y);}}
						}
					}
				}
			}
		}
	}

	public void ApplyDamage (Vector4 point) {
		if (shields_on) {myShield.ApplyDamage(point);return;}
		if (!isServer||dead) return;
		Vector3 p= new Vector3(point.x,point.y,point.z);
		if (lost_hp>=max_dps) return;
		else {
				if (hp<2/3*maxhp) {
					GameObject f=null;
					bool a=false;
					if (!fire1) {a=true;fire1=true;f=GameObject.Find("menu").GetComponent<SceneResLoader>().EffectRequest(p,1);}
					if (hp<1/2*maxhp&&!a) {
						if (!fire2) {a=true;fire2=true;f=GameObject.Find("menu").GetComponent<SceneResLoader>().EffectRequest(p,1);}
						if (hp<1/3*maxhp&&!a) {
							if (!fire3) {a=true;fire3=true;f=GameObject.Find("menu").GetComponent<SceneResLoader>().EffectRequest(p,1);}
							if (hp<1/6*maxhp&&!a) {
							if (!fire4) {a=true;fire4=true;f=Instantiate(Resources.Load<GameObject>("fire")) as GameObject;}
							}
						}
					}
					if (a&&f!=null) {f.transform.parent=transform;f.transform.forward=transform.position-f.transform.position;}
				}
			}
		Vector3 x=transform.InverseTransformPoint(p);
		float damage=point.w;
		if (x.z>=14) {
			damage*=1-nose_armor;
		}
		else {
			if (x.y<=-5&&x.z<-13) {damage*=1-bottom_armor;}
			else {
				if (x.x<=-5&&x.z<-13) {damage*=1-leftwing_armor;}
				else {
					if (x.x>=5&&x.z<-13) {damage*=1-rightwing_armor;}
					else {
						damage*=armor_bonus;
					}
				}
			}
		}
		if (damage+lost_hp>max_dps) {lost_hp=max_dps;}
		else {lost_hp+=damage;}
		if (!underfire) {StartCoroutine(DamagePass());}
		underfire=true;
	}

	IEnumerator DamagePass () {
		yield return new WaitForSeconds(1);
		if (hp-lost_hp<0) {
			RpcMakeMeDead(2);
		}
		else hp-=lost_hp;
		RpcSetHP (hp);
		lost_hp=0;
		underfire=false;
	}

	[ClientRpc]
	void RpcSetHP (float x) {
		hp=x;
	}

	[ClientRpc]
	public void RpcMakeMeDead (byte cpos) {
		BroadcastMessage("Destroying",SendMessageOptions.DontRequireReceiver); //сброс всех визуальных эффектов
		reactor_ol_timeleft=0;
		reactor_cd_timeleft=0;
		reactor_overloaded=false;
		hp=0;
		dead=true;
		Vector3 a=c1.size;Vector3 b=c2.size;Vector3 c=c3.size;Vector3 d=Vector3.zero;Vector3 e=Vector3.zero;Vector3 f=Vector3.zero;
		if (c4) d=c4.size;if (c5) e=c5.size; if (c6) f=c6.size;
		c1.enabled=false;
		c2.enabled=false;
		c3.enabled=false;
		if (c4) c4.enabled=false;
		if (c5) c5.enabled=false;
		if (c6) c6.enabled=false;
		myShield.powered=false;
		myShield.hp=0;
		myShield.gameObject.SetActive(false);
		foreach (GameObject module in modules) {
			if (module!=null)		module.SetActive(false);
		}
		gameObject.tag="Respawn";
		ship.SetActive(false);
		if (GetComponent<marker>()) {Destroy(GetComponent<marker>());}
		if (!srl.mission) {
		if (isServer) {RpcActiveArmor(false);StartCoroutine(Respawn());}
		if (isLocalPlayer) gameObject.GetComponent<NetworkShipGUI>().MakeMeDead(respawn_time*(2-respawn_bonus));
		}
		GameObject.Find("menu").SendMessage("MissionFailed",SendMessageOptions.DontRequireReceiver);
	}

	IEnumerator Respawn () {
		yield return new WaitForSeconds(respawn_time*(2-respawn_bonus));
		float a=Random.value;
		Vector3 point=new Vector3(a*5000,0,5000*Mathf.Sqrt(1-a)*Mathf.Sign(Random.value));
		transform.position=point;
		transform.forward=Vector3.zero-point;
		gameObject.BroadcastMessage("StartGame",SendMessageOptions.DontRequireReceiver);
		RpcStartAgain(point);
	}

	[ClientRpc]
	public void RpcStartAgain(Vector3 point) {
		if (isServer) return;
		transform.position=point;
		transform.forward=Vector3.zero-point;
		gameObject.BroadcastMessage("StartGame",SendMessageOptions.DontRequireReceiver);
	}
		
	[Command]
	public void CmdActiveArmor(bool x) {
		if (x) {if (capacity>armor_consumption)	RpcActiveArmor(true);}
		else {RpcActiveArmor(false);}
	}
	[ClientRpc]
	void RpcActiveArmor (bool x) {
		if (x) {
			for (byte i=0;i<modules.Length;i++) {
				if (i==25||i==26||i==27||!modules[i]) continue;
				if (modules[i].GetComponent<active_armor>()) {modules[i].SendMessage("Activate",SendMessageOptions.DontRequireReceiver);}
			}
			armor_on=true;
		}
		else {
			for (byte i=0;i<modules.Length;i++) {
				if (i==25||i==26||i==27||!modules[i]) continue;
				if (modules[i].GetComponent<active_armor>()) {modules[i].SendMessage("Deactivate",SendMessageOptions.DontRequireReceiver);}
			}
			armor_on=false;
		}
	}

	[Command]
	public void CmdShield (bool x) {
		if (x) {if (capacity>=myShield.consumption*shield_bonus) {RpcShield(true);}}
		else {RpcShield(false);}
	}
	[ClientRpc]
	public void RpcShield(bool x) {
		if (x) {myShield.gameObject.SetActive(true);
			myShield.Power(true);
			shields_on=true;
			c1.enabled=false;c2.enabled=false;c3.enabled=false; if (c4) c4.enabled=false; if (c5) c5.enabled=false; if (c6) c6.enabled=false;
		}
		else {myShield.Power(false);c1.enabled=true;c2.enabled=true;c3.enabled=true; if (c4) c4.enabled=true; if (c5) c5.enabled=true; if (c6) c6.enabled=true;}
	}
	[Command]
	public void CmdSetCommand(byte x) {
		byte pointx=0;
		pointx=GameObject.Find("server_terminal").GetComponent<ServerTerminal>().GetSpawnPoint(x);
		if (!srl.mission) {
		if (x==1) {transform.position=new Vector3(pointx*100-500,0,-1000);command='1';} 
		else { transform.position=new Vector3(pointx*100-500,0,1000);transform.Rotate(new Vector3(0,180,0));command='2';
			if (isLocalPlayer) {Global.cam.transform.parent.RotateAround(cam.transform.position,Vector3.up,180);}
			}		
		}
		else {
			transform.position=srl.mission_spawn.transform.position;
		}
		foreach (GameObject module in modules) {
			if (module!=null) module.SetActive(true);
		}
		ship.SetActive(true);
		if (pointx<10) gameObject.name=x.ToString()+"0"+pointx.ToString(); else {gameObject.name=x.ToString()+pointx.ToString();}
		bool controlledByHuman=false;
		if (gameObject.GetComponent<NetworkShipGUI>()) {controlledByHuman=true;gameObject.name+="hl";} else gameObject.name+="ca";
		if (special_char=='\0') special_char='u';
		gameObject.name+=special_char;
		RpcSetCommand(x,pointx,controlledByHuman);
		Instantiate(srl.hyperjump_exit_effect,transform.position,transform.rotation);
		if (isLocalPlayer) gameObject.GetComponent<NetworkShipGUI>().exit_effect_time=1;
	}
	[ClientRpc]
	public void RpcSetCommand (byte x,byte n,bool cbh) {
		if (isServer) return;
		if (!srl.mission) {
		if (x==1) {transform.position=new Vector3(n*100-500,0,-1000);command='1';} 
		else { transform.position=new Vector3(n*100-500,0,1000);transform.Rotate(new Vector3(0,180,0));command='2';
			}}
		else {
			transform.position=srl.mission_spawn.transform.position;
		}
		foreach (GameObject module in modules) {
			if (module!=null) module.SetActive(true);
		}
		ship.SetActive(true);
		Instantiate(srl.hyperjump_exit_effect,transform.position,transform.rotation);
		if (isLocalPlayer) gameObject.GetComponent<NetworkShipGUI>().exit_effect_time=1;
		if (n<10) gameObject.name=x.ToString()+"0"+n.ToString(); else {gameObject.name=x.ToString()+n.ToString();}
		if (cbh) gameObject.name+='h'; else gameObject.name+='c';
		if (isLocalPlayer) gameObject.name+='l'; else gameObject.name+='a';
		if (special_char=='\0') special_char='u';
		gameObject.name+=special_char;
	}

	[Command]
	public void CmdTorpedoesLaunch () {
		tapp t=modules[0].GetComponent<tapp>();
		if (!t.ready) return;
		t.ready=false;
		RpcTappState(false);
		for (byte i=0;i<t.guns.Length;i++) {
			GameObject x=Network.Instantiate(t.projectile,t.guns[i].transform.position,transform.rotation,0) as GameObject;
			if (t.local_attack_vector!=Vector3.forward) x.transform.forward=transform.TransformDirection(t.local_attack_vector);
			x.GetComponent<torpedo>().timer=t.range/x.GetComponent<torpedo>().speed;
			x.GetComponent<torpedo>().damage=t.damage;
			Physics.IgnoreCollision(c1,x.GetComponent<Collider>());
		}
		StartCoroutine(TappReload(t.cooldown));
	}

	void RpcTappState(bool x) {
		modules[0].GetComponent<tapp>().ready=x;
	}

	IEnumerator TappReload (float t) {
		yield return new WaitForSeconds(t);
		modules[0].GetComponent<tapp>().ready=true;
		RpcTappState(true);
	}

	public void OnCollisionEnter (Collision c) {
		if (dead||shields_on) return;
		if (!second_mass_calculation) {
			rbody.mass=c1.size.x*c1.size.y*c1.size.z+c2.size.x*c2.size.y*c2.size.z+c3.size.x*c3.size.y*c3.size.z;
			if (c4) {rbody.mass+=c4.size.x*c4.size.y*c4.size.z;}
			if (c5) {rbody.mass+=c5.size.x*c5.size.y*c5.size.z;}
			if (c6) {rbody.mass+=c6.size.x*c6.size.y*c6.size.z;}
			second_mass_calculation=true;
		}
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
				impulse=s.x*s.y*s.z*transform.localScale.x*transform.localScale.y*transform.localScale.z*rbody.velocity.magnitude;
				if (c.collider.tag=="Environment") {impulse*=10;}
			}
		}
		float damage=impulse-rbody.mass*rbody.velocity.magnitude;
		if (damage<=0) return;
		if (cpoint.z>14&&taran_cover) {damage/=2;}
		damage/=100;
		hp-=damage;
	
		if (damage>1000) {GameObject p=Instantiate(ResLoad.pieces,transform.TransformPoint(cpoint),Quaternion.identity) as GameObject;p.SetActive(true);}
		if (hp<=0&&isServer) {
			byte cn=0;
			if (cpoint.z>14) cn=4;
			else {
				if (cpoint.z>-13) cn=1;
				else {
					if (cpoint.z<-29) cn=3;
					else {
						if (cpoint.x>0) {if (c5) cn=5; else cn=2;}
						else {if (c6) cn=6; else cn=2;}

					}
				}
			}
			dead=true;
			RpcMakeMeDead(cn);
		}
	}

	public void SetLayer (int layer) {
		gameObject.layer=layer;
		ship.layer=layer;
		foreach (GameObject m in modules) {
			m.layer=layer;
		}
		c1.enabled=true;
		c2.enabled=true;
		c3.enabled=true;
		if (c4) c4.enabled=true;
		if (c5) c5.enabled=true;
		if (c6) c6.enabled=true;
	}
}
