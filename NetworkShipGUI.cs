using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkShipGUI : MonoBehaviour {
	public NetworkShipController nsc;
	int ogsw;
	int ogsh;
	public int k;
	Texture reactor_tx;
	public Texture ind_green;
	public Texture ind_red;
	public Texture ind_white;
	public Texture ind_yellow;
	public Texture ind_grey;
	public Texture h_bar_frame; //horizontal
	public Texture h_green_line;
	public Texture h_blue_line;
	public Texture h_lblue_line;
	public Texture h_red_line;
	bool game_started=false;
	Vector2 mp;
	float respawn_timer;
	bool dead=false;
	bool widescreen=false;
	public Texture[] module_icons;
	float fps;
	float fps_to_screen;
	byte team_number=0;
	public ServerTerminal st;
	public float exit_effect_time=0;
	float exit_time=0;
	public bool zone_leaving_alarm=false;
	public SceneResLoader srl;

	// Use this for initialization
	void Start () {
		ogsw=Screen.width;
		ogsh=Screen.height;
		if (ogsh*(13/9)>ogsw) {
			k=ogsw/32;widescreen=true;//widescreen 16:9
		}
		else {k=ogsw/24;}//quadscreen 4:3
		reactor_tx=Resources.Load<Texture>("reactor_icon");
		ind_green=Resources.Load<Texture>("indicator_on");
		ind_red=Resources.Load<Texture>("indicator_off");
		ind_white=Resources.Load<Texture>("indicator_white");
		ind_yellow=Resources.Load<Texture>("indicator_reload");
		ind_grey=Resources.Load<Texture>("indicator_grey");
		h_bar_frame=Resources.Load<Texture>("horizontal_bar_frame");
		h_green_line=Resources.Load<Texture>("horizontal_green_line");
		h_blue_line=Resources.Load<Texture>("horizontal_blue_line");
		h_lblue_line=Resources.Load<Texture>("horizontal_lightblue_line");
		h_red_line=Resources.Load<Texture>("horizontal_red_line");
		module_icons=new Texture[7];
		module_icons[0]=Resources.Load<Texture>("mod14_icon"); //longcaster
		module_icons[1]=Resources.Load<Texture>("mod15_icon"); //longpulser
		module_icons[2]=Resources.Load<Texture>("mod16_icon"); //pointcaster (laser_caster)
		module_icons[3]=Resources.Load<Texture>("mod17_icon"); //pointpulser
		module_icons[4]=Resources.Load<Texture>("mod19_icon");//active_armor
		module_icons[5]=Resources.Load<Texture>("mod07_icon");//shield
		module_icons[6]=Resources.Load<Texture>("mod25_icon");//torpedoes
		st=GameObject.Find("server_terminal").GetComponent<ServerTerminal>();
		srl=GameObject.Find("menu").GetComponent<SceneResLoader>();
		}



	public void StartGame() {
		game_started=true;
		if (dead) {dead=false;respawn_timer=0;}
		else {
			byte lb=0; byte rb=0;byte rf=0;byte lf=0;
			Rect r1=new Rect(0,ogsh/2,k/2,k/2);
			Rect r2=new Rect(ogsw-k/2,ogsh/2,k/2,k/2);
			Rect r3=new Rect (0,ogsh-k/2,k/2,k/2);
			Rect r4=new Rect (ogsw-k/2,ogsh-k/2,k/2,k/2);
			if (nsc.modules[0]!=null||nsc.modules[26]!=null) {
				lgc_indicator lgci;
				if (nsc.modules[0].GetComponent<longcaster>()||nsc.modules[0].GetComponent<tapp>()) {
					if (nsc.modules[0].GetComponent<longcaster>()) {
					lgci=nsc.modules[0].AddComponent<lgc_indicator>();
					lgci.nsg=this;
					lgci.myGun=nsc.modules[0].GetComponent<longcaster>();
					if (nsc.modules[26]!=null) {
						if (!nsc.modules[26].GetComponent<longcaster>()) {lgci.myRect=new Rect(ogsw/2-1.5f*k,ogsh-3*k,3*k,3*k);}
						else {
							lgci.myRect=new Rect(ogsw/2-k/2,ogsh-2*k,k,k);
							lgci=nsc.modules[26].AddComponent<lgc_indicator>();
							lgci.nsg=this;
							lgci.myGun=nsc.modules[26].GetComponent<longcaster>();
							lgci.myRect=new Rect(ogsw/2-k/2,ogsh-k,k,k);
						}
					}
					else {lgci.myRect=new Rect(ogsw/2-k,ogsh-2*k,2*k,2*k);}
					}}
				else {
					if (nsc.modules[26].GetComponent<longcaster>()) {
						lgci=nsc.modules[26].AddComponent<lgc_indicator>();
						lgci.nsg=this;
						lgci.myGun=nsc.modules[26].GetComponent<longcaster>();
						lgci.myRect=new Rect(ogsw/2-k,ogsh-2*k,2*k,2*k);
					}
				}
			}
		for (byte i=1;i<nsc.modules.Length;i++) {
				if (nsc.modules[i]==null||i==26) continue;
			if (nsc.modules[i].GetComponent<laser_caster>()) {
				lc_indicator lci=nsc.modules[i].AddComponent<lc_indicator>();
				lci.myGun=nsc.modules[i].GetComponent<laser_caster>();
				lci.nsg=this;
					if (nsc.modules[i].transform.localPosition.x<0) {
						if (!nsc.modules[i].GetComponent<laser_caster>().forward)	{lci.myRect=r1;r1.y+=k/2;if (lb==5)  {r1.x+=k/2;r1.y=ogsh/2;}lb++;}
						else {lci.myRect=r3;r3.x+=k/2;lf++;}
					}
					else {
						if (!nsc.modules[i].GetComponent<laser_caster>().forward)	{lci.myRect=r2;r2.y+=k/2;if (rb==5) {r2.x-=k/2;r2.y=ogsh/2;}rb++;}
						else {lci.myRect=r4;r4.x-=k/2;rf++;}
					}
			}
				else {
					if (nsc.modules[i].GetComponent<longcaster>()) {
						lgc_indicator lg=nsc.modules[i].AddComponent<lgc_indicator>();
						lg.myGun=nsc.modules[i].GetComponent<longcaster>();
						lg.nsg=this;
						if (nsc.modules[i].transform.localPosition.x<0) {
							if (!nsc.modules[i].GetComponent<longcaster>().forward)	{lg.myRect=r1;r1.y+=k/2;if (lb==5)  {r1.x+=k/2;r1.y=ogsh/2;};lb++;}
							else {lg.myRect=r3;r3.x+=k/2;lf++;}
						}
						else {
							if (!nsc.modules[i].GetComponent<longcaster>().forward)	{lg.myRect=r2;r2.y+=k/2;if (rb==5) {r2.x-=k/2;r2.y=ogsh/2;}rb++;}
							else {lg.myRect=r4;r4.x-=k/2;rf++;}
						}
					}
					else {
						if (nsc.modules[i].GetComponent<pointpulser>()) {
						pp_indicator pp=nsc.modules[i].AddComponent<pp_indicator>();
						pp.myGun=nsc.modules[i].GetComponent<pointpulser>();
						pp.nsg=this;
						if (nsc.modules[i].transform.localPosition.x<0) {
								if (!nsc.modules[i].GetComponent<pointpulser>().forward)	{pp.myRect=r1;r1.y+=k/2;if (lb==6)  {r1.x+=k/2;r1.y=ogsh/2;}lb++;}
								else {pp.myRect=r3;r3.x+=k/2;lf++;}
						}
						else {
								if (!nsc.modules[i].GetComponent<pointpulser>().forward)	{pp.myRect=r2;r2.y+=k/2;if (rb==6) {r2.x-=k/2;r2.y=ogsh/2;}rb++;}
								else {pp.myRect=r4;r4.x-=k/2;rf++;}
						}
						}
						else {
							if (nsc.modules[i].GetComponent<pulser>()) {
								pr_indicator pr=nsc.modules[i].AddComponent<pr_indicator>();
								pr.myGun=nsc.modules[i].GetComponent<pulser>();
								pr.nsg=this;
								if (nsc.modules[i].transform.localPosition.x<0) {
									if (!nsc.modules[i].GetComponent<pulser>().forward)	{pr.myRect=r1;r1.y+=k/2;if (lb==6)  {r1.x+=k/2;r1.y=ogsh/2;}lb++;}
									else {pr.myRect=r3;r3.x+=k/2;lf++;}
								}
								else {
									if (!nsc.modules[i].GetComponent<pulser>().forward)	{pr.myRect=r2;r2.y+=k/2;if (rb==6) {r2.x-=k/2;r2.y=ogsh/2;}rb++;}
									else {pr.myRect=r4;r4.x-=k/2;rf++;}
								}
							}
						}
					}
				}
		}
			if (team_number==0) {
				if (srl.mission) {nsc.CmdSetCommand(1);}
				else {
				if (st.inCommand1>st.inCommand2) {nsc.CmdSetCommand(2);}
					else {nsc.CmdSetCommand(1);}}
			}
		}

		StartCoroutine(FPSToScreen());
	}

	IEnumerator FPSToScreen () {
		yield return new WaitForSeconds(1);
		fps_to_screen=fps;
		StartCoroutine(FPSToScreen());
	}

	public void MakeMeDead(float l) {
		game_started=false;
		dead=true;
		respawn_timer=l;
		exit_effect_time=1;
	}

	// Update is called once per frame
	void Update () {
		if (exit_effect_time>0) {exit_effect_time-=Time.deltaTime;if (exit_effect_time<0) {exit_effect_time=0;}}
		fps=1/Time.deltaTime;
		if (dead) {respawn_timer-=Time.deltaTime;if (respawn_timer<=0) {dead=false;}return;}
		if (!game_started) return;
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {nsc.CmdTnLeft();}
		if (Input.GetKeyDown (KeyCode.RightArrow)) {nsc.CmdTnRight();}
		if (Input.GetKeyDown(KeyCode.UpArrow)&nsc.speed_state<3) {nsc.CmdSpeedChange(true);}
		if (Input.GetKeyDown(KeyCode.DownArrow)&&nsc.speed_state>0) {nsc.CmdSpeedChange(false);}
		if (Input.GetKeyDown(KeyCode.LeftAlt)) {nsc.CmdFireLeft();	}
		if (Input.GetKeyDown(KeyCode.RightAlt)) {nsc.CmdFireRight();	}
		if (Input.GetKeyDown(KeyCode.RightControl)) {nsc.CmdFireRightFwd();	}
		if (Input.GetKeyDown(KeyCode.LeftControl)) {nsc.CmdFireLeftFwd();	}
		if (Input.GetKeyDown(KeyCode.Space)) {nsc.CmdFireForwards();}

		if (Input.GetKeyDown ("q")) {if (nsc.engine_mode!=0) nsc.CmdChangeEngineMode(0);} 
		if (Input.GetKeyDown ("w")) {if (nsc.engine_mode!=1) nsc.CmdChangeEngineMode(1);} 
		if (Input.GetKeyDown ("e")) {if (nsc.engine_mode!=2) nsc.CmdChangeEngineMode(2);} 
		if (Input.GetKeyDown ("r")) {if (nsc.reactor_overloaded) nsc.CmdReactorState(false); else nsc.CmdReactorState(true);}
		if (nsc.tapps) {if (Input.GetKeyDown("t")) {if (nsc.modules[0].GetComponent<tapp>().ready) {nsc.CmdTorpedoesLaunch();}}}

		if (Input.GetKeyDown ("a")) {if (nsc.armor_on) nsc.CmdActiveArmor(false); else {if (nsc.capacity>nsc.armor_consumption) nsc.CmdActiveArmor(true);}} 
		if (Input.GetKeyDown ("s")) {if (nsc.shields_on) nsc.CmdShield(false); else {if (nsc.capacity>=nsc.myShield.consumption*nsc.shield_bonus)  nsc.CmdShield(true);}} 

		if (Input.GetMouseButtonDown(0)) {
			mp=Input.mousePosition;//y - обратные координаты
			if (mp.y>2*k&&mp.x>k&&mp.x<ogsw-k) {
				Ray sr= Global.cam.GetComponent<Camera>().ScreenPointToRay(new Vector3(mp.x,mp.y,0));
				RaycastHit st;
				if (!Physics.Raycast(sr.origin,sr.direction,out st,nsc.radar_distance*nsc.radar_bonus)) return;
				if (st.collider.transform.root.gameObject.name[0]!=nsc.command&&st.collider.transform.root.tag=="Player"&&st.transform.root!=transform.root) {
					bool a=false; byte x=0;
					GameObject targ=null;
					targ=st.collider.transform.root.gameObject;
					if (nsc.marked_targets.Length!=0) {
						for (byte i=0;i<nsc.marked_targets.Length;i++) {
						if (nsc.marked_targets[i]==targ) {a=true;x=i;break;}
					}
					} else {a=false;x=0;}
					if (!a) {nsc.CmdAddTargetToList(targ);targ.AddComponent<marker>();}
					else {if (nsc.marked_targets[x]!=null) {if (nsc.marked_targets[x].GetComponent<marker>()) nsc.marked_targets[x].GetComponent<marker>().RemoveSelection();}nsc.CmdRemoveTargetFromList(x);}
					
				}
			}
		}
		exit_time=(srl.map_radius-transform.position.magnitude)/nsc.speed;

	}

	void OnGUI () {
		Rect r;
		int fs=GUI.skin.GetStyle("Label").fontSize;
		if (exit_effect_time>0) {
			r=new Rect (ogsw/2-exit_effect_time*960,ogsh/2-exit_effect_time*540,exit_effect_time*1920,exit_effect_time*1080);
			GUI.DrawTexture(r,nsc.srl.hyperjump_exit_sprite,ScaleMode.StretchToFill);}		
		
		if (dead) {GUI.Label(new Rect(ogsw/2-128,ogsh/2-96,256,128),((int)respawn_timer).ToString());return;}
		if (game_started) {
			GUI.skin=Global.mySkin;
			float d=0;
			float lv=ogsw/2-7*k;
			r=new Rect(0,ogsh-k,lv,k/2);
			if (nsc.capacity>=0) {
			 d=nsc.capacity/nsc.goal_capacity;
			if (d<0) d=0;
			if (d>1) d=1;
				r.width*=d;
			if (!nsc.reactor_overloaded) GUI.DrawTexture(r,h_blue_line,ScaleMode.StretchToFill);
			else GUI.DrawTexture(r,h_red_line,ScaleMode.StretchToFill);
			}
			r.width=lv;
			GUI.DrawTexture(r,h_bar_frame,ScaleMode.StretchToFill);
			GUI.skin.GetStyle("Label").fontSize=(int)(k/2*0.8f);
			GUI.Label(r,((int)(nsc.capacity)).ToString()+"/"+nsc.goal_capacity.ToString());

			GUI.skin=Global.stSkin;
			GUI.skin.GetStyle("Label").fontSize=(int)(k/4*0.8f);
		r=new Rect(ogsw/2-7*k,ogsh-2*k,k,k);
		if (nsc.armor_consumption>0) {
			if (nsc.armor_on) {if (GUI.Button(r,ind_yellow)) {nsc.CmdActiveArmor(false);}}
				else {if (GUI.Button(r,ind_grey)&&nsc.capacity>nsc.armor_consumption) {nsc.CmdActiveArmor(true);}}
			GUI.DrawTexture(r,module_icons[4],ScaleMode.StretchToFill);
			GUI.Label(new Rect(ogsw/2-7*k,ogsh-5*k/4,k,k/4),nsc.armor_consumption.ToString());
			} r.y+=k;
				
			if (nsc.reactor_overloaded) {
				GUI.DrawTexture(r,ind_red,ScaleMode.StretchToFill);
				if (GUI.Button(r,reactor_tx)) {nsc.CmdReactorState(false);}
				GUI.skin.GetStyle("Label").fontSize=(int)(r.height/6);
				r.height/=4;
				GUI.Label(r,((int)(nsc.reactor_ol_timeleft)).ToString());
				r.height*=4;
			}
			else {
				if (nsc.reactor_cd_timeleft==0) {
					GUI.DrawTexture(r,ind_green,ScaleMode.StretchToFill);
					if (GUI.Button(r,reactor_tx)) {nsc.CmdReactorState(true);}
				}
				else {
					GUI.DrawTexture(r,ind_yellow,ScaleMode.StretchToFill);
					GUI.skin.GetStyle("Label").fontSize=(int)(r.height/6);
					r.height/=4;
					GUI.Label(r,((int)(nsc.reactor_cd_timeleft)).ToString());
					r.height*=4;
				}
			}

		r.x+=3*k;

		if (nsc.tn_left) {if (GUI.Button(r,ResLoad.toLeft_a_tx)) {nsc.CmdTnLeft();}}
		else {if (GUI.Button(r,ResLoad.toLeft_tx)) {nsc.CmdTnLeft();}}
		r.x+=k;
		
		GUI.skin.GetStyle("Button").fontSize=(int)(k/5);
			r.y-=k;r.width=1.5f*k;r.height=k/2;
		if (GUI.Button(r,"вперед") ) {nsc.CmdChangeSpeedState(3);} r.y+=k/2;
			if (GUI.Button(r,"1/2")) {nsc.CmdChangeSpeedState(2);} r.y+=k/2;
			if (GUI.Button(r,"стоп")) {nsc.CmdChangeSpeedState(1);} r.y+=k/2;
			if (GUI.Button(r,"назад")) {nsc.CmdChangeSpeedState(0);} r.y+=k/2;
		GUI.DrawTexture(new Rect(ogsw/2-3*k,ogsh-nsc.speed_state*k/2-k/2,1.5f*k,k/2),ResLoad.selection_frame_tx,ScaleMode.StretchToFill);

			r=new Rect(ogsw/2+1.5f*k,ogsh-k/2,0.3f*k,k/2);
			if (GUI.Button(r,"A")) nsc.standartHeight=100;r.x+=0.3f*k;
			if (GUI.Button(r,"B")) nsc.standartHeight=50;r.x+=0.3f*k;
			if (GUI.Button(r,"C")) nsc.standartHeight=0;r.x+=0.3f*k;
			if (GUI.Button(r,"D")) nsc.standartHeight=-50;r.x+=0.3f*k;
			if (GUI.Button(r,"E")) nsc.standartHeight=-100;r.x+=0.3f*k;
			r=new Rect(ogsw/2+1.5f*k,ogsh-k,1.5f*k,k/2);
			if (GUI.Button(r,"Норма")) {if (nsc.engine_mode!=0) nsc.CmdChangeEngineMode(0);} r.y-=k/2;
			if (GUI.Button(r,"Маневр")) {if (nsc.engine_mode!=1) nsc.CmdChangeEngineMode(1);} r.y-=k/2;
			if (GUI.Button(r,"Форсаж")) {if (nsc.engine_mode!=2) nsc.CmdChangeEngineMode(2);} r.y-=k/2;
			GUI.DrawTexture(new Rect(ogsw/2+1.5f*k,ogsh-k-k/2*nsc.engine_mode,1.5f*k,k/2),ResLoad.selection_frame_tx,ScaleMode.StretchToFill);

			r=new Rect(ogsw/2+3*k,ogsh-k,k,k);
		if (nsc.tn_right) {if (GUI.Button(r,ResLoad.toRight_a_tx)) {nsc.CmdTnRight();}}
		else {if (GUI.Button(r,ResLoad.toRight_tx)) {nsc.CmdTnRight();}}
		r.x+=k;

		
		r=new Rect(ogsw/2+6*k,ogsh-k,k,k);
			if (nsc.myShield.powered) {if (GUI.Button(r,ind_yellow)) {nsc.CmdShield(false);}}
			else {if (GUI.Button(r,ind_grey)) {nsc.CmdShield(true);}}
			GUI.DrawTexture(r,module_icons[5],ScaleMode.StretchToFill);
			GUI.Label(new Rect(ogsw/2+6*k,ogsh-k/4,k,k/4),(nsc.myShield.consumption*nsc.shield_bonus).ToString());

			GUI.skin=Global.mySkin;
			r.x+=k;r.height=k/2;
			lv=ogsw-r.x;
			d=nsc.hp/nsc.maxhp; if (d<0) d=0;
			r.width=d*lv;
			if (d>0.2f) GUI.DrawTexture(r,h_green_line,ScaleMode.StretchToFill); else GUI.DrawTexture(r,h_red_line,ScaleMode.StretchToFill);
			r.width=lv;
			GUI.DrawTexture(r,h_bar_frame,ScaleMode.StretchToFill);
		    GUI.Label(r,nsc.hp.ToString());
			if (nsc.shields_on) {
				r.y-=k/2;
				r.width=(float)(nsc.myShield.hp)/nsc.myShield.maxhp*lv;
				GUI.DrawTexture (r,h_blue_line,ScaleMode.StretchToFill);
				r.width=lv;
				GUI.DrawTexture(r,h_bar_frame,ScaleMode.StretchToFill);
				GUI.Label(r,nsc.myShield.hp.ToString());
			}
	
		GUI.Label(new Rect(0,k,6*k,k),"FPS = " + ((byte)fps_to_screen).ToString());
			if (exit_time<=20) {GUI.Label(new Rect(ogsw/2-3*k,ogsh/2-k,6*k,2*k),"Вы покидаете зону боя!");}
		}
		else {
			//match not started yet
			if (team_number==0&&!nsc.srl.mission) {
			GUI.Label(new Rect(ogsw/2-5*k,0,10*k,2*k),"Выберите команду");
				if (GUI.Button(new Rect(ogsw/2-5*k,2*k,5*k,2*k),"Команда 1")) {team_number=1;nsc.CmdSetCommand(1);}
				GUI.Label(new Rect(ogsw/2-5*k,4*k,5*k,k),st.inCommand1.ToString()+" игроков");
				if (GUI.Button(new Rect(ogsw/2,2*k,5*k,2*k),"Команда 2")) {team_number=2;nsc.CmdSetCommand(2);}
				GUI.Label(new Rect(ogsw/2,4*k,5*k,k),st.inCommand2.ToString()+" игроков");
			}
		}
		GUI.skin.GetStyle("Label").fontSize=fs;
		GUI.skin.GetStyle("Button").fontSize=fs;
		}



}
