using UnityEngine;
using System.Collections;

public class customizer : MonoBehaviour {
	public string shipModules;
	byte chosen_slot=255;
	Rect chosen_rect;
	bool show_properties=false;
	public string s1="";
	public string s2="";
	public GameObject ship;
	public GameObject cam;
	public ushort distance=30;

	int ck;
	int prevpos=0;
	int ogsh;

	public Texture text_data_back_tx;
	public Texture module_slot_tx;
	public Texture filled_module_slot_tx;
	GUISkin menu_skin;
	GameObject[] module_pick;
	GameObject[] modules;
	GameObject[] ship_modules;
	Vector3 camera_target;

	int maxhp=20000;
	float reactor_bonus=1;
	int maxcapacity=1000; int overload_capacity=4000;
	int constant_supply=0;
	int maxspeed1=15; int maxspeed2=40; int maxspeed3=400; float speed_bonus=1f;
	float angle_acceleration=1f; float maneuver_bonus=1f;
	float radar_bonus=1f; int radar_distance=5000;
	float armor_bonus=1f; float nose_armor=0;float leftwing_armor=0;float rightwing_armor=0; float bottom_armor=0;
	float respawn_bonus=1f;
	float weapons_bonus=1f;
	float shields_bonus=1f;
	bool taran_cover=false;
	int shields_cons=0;
	int active_armor_cons=0;
	// Use this for initialization
	void Start () {

		menu_skin=Resources.Load<GUISkin>("menu_skin");
		module_pick=new GameObject[0];
		modules=new GameObject[36];
		Vector3 aftercamera=Vector3.zero;
		ogsh=Screen.height;

		if (PlayerPrefs.HasKey("shipModules")) {shipModules=PlayerPrefs.GetString("shipModules");} 
		else {shipModules= new string('0',56);}
		cam.transform.position=ship.transform.position+ship.transform.TransformDirection(new Vector3(-distance,distance/10,distance));
		cam.transform.LookAt (camera_target);
		for (byte i=0;i<36;i++) {
			if (i==0||i==1||i==3||i==13) continue;
			if (i<10) {if (Resources.Load<GameObject>("0"+i.ToString())) modules[i]=Instantiate(Resources.Load<GameObject>("0"+i.ToString()),aftercamera,Quaternion.identity) as GameObject;} 
			else {if (Resources.Load<GameObject>(i.ToString()))  modules[i]=Instantiate(Resources.Load<GameObject>(i.ToString()),aftercamera,Quaternion.identity) as GameObject; }
			if (modules[i]) {modules[i].AddComponent<module_information>();modules[i].transform.localScale=new Vector3(0.1f,0.1f,0.1f);}
		}
		ck=Screen.width/38;
		ship_modules=new GameObject[28];
		SpawnModules(shipModules);
	}

	void RefreshModuleData(int pos) {
		if (pos<module_pick.Length) {
			if (module_pick[pos].GetComponent<module_information>()) {
				s1=module_pick[pos].GetComponent<module_information>().infostring;
				s2=module_pick[pos].GetComponent<module_information>().ps;
			}
		}
	}

	void Update () {
		shields_cons=(int)(600*shields_bonus);
		if (chosen_slot==255) {
			camera_target = ship.transform.position;
		}
		else {
			if (Input.GetMouseButtonDown (1)) {chosen_slot = 255;camera_target = ship.transform.position;module_pick=new GameObject[0];}

			if (module_pick.Length!=0){
				Vector2 mp=Input.mousePosition;
				if (mp.x>=19*ck&&mp.y<9*ck&&mp.x<=26*ck) {
					show_properties=true;
					float l=mp.y;
					l/=ck;
					int pos=8;
					pos-=(int)Mathf.Floor(l);
					if (pos>0&&pos<9) {
						if (pos!=prevpos) {RefreshModuleData(pos);prevpos=pos;}}
					else show_properties=false;
				}
				else {show_properties=false;}
			}

		}
		if (cam && cam.activeSelf) {
			float sw=Input.GetAxis("Mouse ScrollWheel");
			if (sw!=0) {cam.transform.Translate(0,0,sw*50,cam.transform);}
			if (Input.GetMouseButton(2)) {
				cam.transform.RotateAround(ship.transform.position,transform.TransformDirection(Vector3.up),50*Input.GetAxis("Mouse X"));
				float y = Input.GetAxis ("Mouse Y");
				cam.transform.RotateAround (ship.transform.position, cam.transform.TransformDirection (Vector3.left), 50 * y);
				cam.transform.LookAt(ship.transform.position);
			}
		}
	}

	void OnGUI () {
			if (12*ck>ogsh/4) {ck=ogsh/30;}
			byte i=0;
			GUI.skin=Global.mySkin;
			Rect cr=new Rect(8*ck,ogsh-11*ck,3*ck,3*ck);
			byte prev_slot=chosen_slot;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=0) {chosen_slot=0;chosen_rect=cr;camera_target = new Vector3 (4,0,15);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=0) {chosen_slot=0;chosen_rect=cr;camera_target = new Vector3 (4,0,15);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;
			cr=new Rect(ck,ogsh-8*ck,ck,ck);
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=1) {chosen_slot=1;chosen_rect=cr;camera_target = new Vector3 (-0.6f,-4,7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=1) {chosen_slot=1;chosen_rect=cr;camera_target = new Vector3 (-0.6f,-4,7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;cr.x+=ck;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=2) {chosen_slot=2;chosen_rect=cr;camera_target = new Vector3 (-1.4f,-3,7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=2) {chosen_slot=2;chosen_rect=cr;camera_target = new Vector3 (-1.4f,-3,7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;cr.x+=ck;cr.width=1.5f*ck;cr.height=cr.width;cr.y-=ck/2;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=3) {chosen_slot=3;chosen_rect=cr;camera_target = new Vector3 (-2,-1.5f,7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=3) {chosen_slot=3;chosen_rect=cr;camera_target = new Vector3 (-2,-1.5f,7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;cr.x+=ck*1.5f;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=4) {chosen_slot=4;chosen_rect=cr;camera_target = new Vector3 (-2,1.5f,7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=4) {chosen_slot=4;chosen_rect=cr;camera_target = new Vector3 (-2,1.5f,7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;cr.x+=ck*1.5f;cr.width=ck;cr.height=ck;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=5) {chosen_slot=5;chosen_rect=cr;camera_target = new Vector3 (-1.4f,3,7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=5) {chosen_slot=5;chosen_rect=cr;camera_target = new Vector3 (-1.4f,3,7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;cr.x+=ck;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=6) {chosen_slot=6;chosen_rect=cr;camera_target = new Vector3 (-0.6f,4,7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=6) {chosen_slot=6;chosen_rect=cr;camera_target = new Vector3 (-0.6f,4,7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;cr.x+=4*ck;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=7) {chosen_slot=7;chosen_rect=cr;camera_target = new Vector3 (0.6f,4,7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=7) {chosen_slot=7;chosen_rect=cr;camera_target = new Vector3 (0.6f,4,7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;cr.x+=ck;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=8) {chosen_slot=8;chosen_rect=cr;camera_target = new Vector3 (1.4f,3,7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=8) {chosen_slot=8;chosen_rect=cr;camera_target = new Vector3 (1.4f,3,7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;cr.x+=ck;cr.width=1.5f*ck;cr.height=cr.width;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=9) {chosen_slot=9;chosen_rect=cr;camera_target = new Vector3 (2,1.5f,7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=9) {chosen_slot=9;chosen_rect=cr;camera_target = new Vector3 (2,1.5f,7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;cr.x+=ck*1.5f;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=10) {chosen_slot=10;chosen_rect=cr;camera_target = new Vector3 (2,-1.5f,7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=10) {chosen_slot=10;chosen_rect=cr;camera_target = new Vector3 (2,-1.5f,7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;cr.x+=ck*1.5f;cr.width=ck;cr.height=ck;cr.y+=ck/2;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=11) {chosen_slot=11;chosen_rect=cr;camera_target = new Vector3 (1.4f,-3,7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=11) {chosen_slot=11;chosen_rect=cr;camera_target = new Vector3 (1.4f,-3,7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;cr.x+=ck;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=12) {chosen_slot=12;chosen_rect=cr;camera_target = new Vector3 (0.6f,-4,7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=12) {chosen_slot=12;chosen_rect=cr;camera_target = new Vector3 (0.6f,-4,7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;

			cr=new Rect(ck,ogsh-6*ck,ck,ck);
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=13) {chosen_slot=13;chosen_rect=cr;camera_target = new Vector3 (-0.7f,-4,-7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=13) {chosen_slot=13;chosen_rect=cr;camera_target = new Vector3 (-0.7f,-4,-7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;cr.x+=ck;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=14) {chosen_slot=14;chosen_rect=cr;camera_target = new Vector3 (-2.3f,-3,-7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=14) {chosen_slot=14;chosen_rect=cr;camera_target = new Vector3 (-2.3f,-3,-7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;cr.x+=ck;cr.width=1.5f*ck;cr.height=cr.width;cr.y-=ck/2;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=15) {chosen_slot=15;chosen_rect=cr;camera_target = new Vector3 (-3f,-1.5f,-7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=15) {chosen_slot=15;chosen_rect=cr;camera_target = new Vector3 (-3f,-1.5f,-7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;cr.x+=ck*1.5f;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=16) {chosen_slot=16;chosen_rect=cr;camera_target = new Vector3 (-3f,1.5f,-7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=16) {chosen_slot=16;chosen_rect=cr;camera_target = new Vector3 (-3f,1.5f,-7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;cr.x+=ck*1.5f;cr.width=ck;cr.height=ck;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=17) {chosen_slot=17;chosen_rect=cr;camera_target = new Vector3 (-2.3f,3,-7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=17) {chosen_slot=17;chosen_rect=cr;camera_target = new Vector3 (-2.3f,3,-7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;cr.x+=ck;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=18) {chosen_slot=18;chosen_rect=cr;camera_target = new Vector3 (-0.7f,4,-7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=18) {chosen_slot=18;chosen_rect=cr;camera_target = new Vector3 (-0.7f,4,-7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;cr.x+=4*ck;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=19) {chosen_slot=19;chosen_rect=cr;camera_target = new Vector3 (0.7f,4,-7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=19) {chosen_slot=19;chosen_rect=cr;camera_target = new Vector3 (0.7f,4,-7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;cr.x+=ck;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=20) {chosen_slot=20;chosen_rect=cr;camera_target = new Vector3 (2.3f,3,-7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=20) {chosen_slot=20;chosen_rect=cr;camera_target = new Vector3 (2.3f,3,-7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;cr.x+=ck;cr.width=1.5f*ck;cr.height=cr.width;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=21) {chosen_slot=21;chosen_rect=cr;camera_target = new Vector3 (3,1.5f,-7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=21) {chosen_slot=21;chosen_rect=cr;camera_target = new Vector3 (3,1.5f,-7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;cr.x+=ck*1.5f;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=22) {chosen_slot=22;chosen_rect=cr;camera_target = new Vector3 (3,-1.5f,-7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=22) {chosen_slot=22;chosen_rect=cr;camera_target = new Vector3 (3,-1.5f,-7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;cr.x+=ck*1.5f;cr.width=ck;cr.height=ck;cr.y+=ck/2;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=23) {chosen_slot=23;chosen_rect=cr;camera_target = new Vector3 (2.3f,-3,-7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=23) {chosen_slot=23;chosen_rect=cr;camera_target = new Vector3 (2.3f,-3,-7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;cr.x+=ck;
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=24) {chosen_slot=24;chosen_rect=cr;camera_target = new Vector3 (0.7f,-4,-7);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=24) {chosen_slot=24;chosen_rect=cr;camera_target = new Vector3 (0.7f,-4,-7);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}
			i+=2;

			cr=new Rect(3*ck,ogsh-4*ck,3*ck,3*ck);
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=25) {chosen_slot=25;chosen_rect=cr;camera_target = new Vector3 (-4,1.5f,-21);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=25) {chosen_slot=25;chosen_rect=cr;camera_target = new Vector3 (-4,1.5f,-21);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));} i+=2;
			cr=new Rect(8*ck,ogsh-5*ck,3*ck,3*ck);
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=26) {chosen_slot=26;chosen_rect=cr;camera_target = new Vector3 (0,-7,-21);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=26) {chosen_slot=26;chosen_rect=cr;camera_target = new Vector3 (0,-7,-21);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));} i+=2;
			cr=new Rect(13*ck,ogsh-4*ck,3*ck,3*ck);
			if (shipModules.Substring(i,2)=="00") {if (GUI.Button(cr,module_slot_tx)&&chosen_slot!=27) {chosen_slot=27;chosen_rect=cr;camera_target = new Vector3 (4,1.5f,-21);FocusCamera ();}} else {if (GUI.Button(cr,filled_module_slot_tx)&&chosen_slot!=27) {chosen_slot=27;chosen_rect=cr;camera_target = new Vector3 (4,1.5f,-21);FocusCamera ();} GUI.Label(cr,(Texture)(Resources.Load("mod"+shipModules.Substring(i,2)+"_icon")));}

			int fs=GUI.skin.GetStyle("Button").fontSize;

			if (chosen_slot!=prev_slot) {ModulesPick(chosen_slot);}
			if (chosen_slot!=255) {
				GUI.Label(chosen_rect,ResLoad.selection_frame_tx);
				GUI.skin=menu_skin;
				cr=new Rect(19*ck,ogsh-10*ck,7*ck,ck);
				GUI.skin.GetStyle("Label").fontSize=ck/3;
				GUI.Label(cr,"Доступные модули");
				cr.y+=ck;

				GUI.skin.GetStyle("Button").fontSize=ck/3;
				if (GUI.Button(cr,"< >")&&ship_modules[chosen_slot]!=null) {ClearSlot(chosen_slot);} cr.y+=ck;
				if (module_pick.Length!=0) {
					for (byte a=1;a<module_pick.Length;a++) {
						string mid=module_pick[a].GetComponent<ModuleInfo>().id;
						if (mid.Length==1) mid='0'+mid;
						string curm="00";
						if (ship_modules[chosen_slot]&&ship_modules[chosen_slot].GetComponent<ModuleInfo>()) {curm=ship_modules[chosen_slot].GetComponent<ModuleInfo>().id;}
						if (GUI.Button(cr,module_pick[a].GetComponent<ModuleInfo>().name)&&mid!=curm) {	SpawnModule(chosen_slot,byte.Parse(mid));} 
						cr.y+=ck;
					}}

				GUI.skin.GetStyle("Label").fontSize=7*ck/24;
				if (show_properties) {
					int bu_font=GUI.skin.GetStyle("Label").fontSize;
					cr=new Rect(26*ck,ogsh-10*ck,7*ck,ck);

					GUI.Label(cr,s1);cr.y+=ck;
					for (byte k=0;k<s2.Length/32-1;k++) {
						GUI.Label(cr,s2.Substring(k*32,32));
						cr.y+=ck;
					}	
					GUI.skin.GetStyle("Label").fontSize=bu_font;}
			}

			int res_font=GUI.skin.GetStyle("Label").fontSize;
			GUI.skin=Global.mySkin;
			if (text_data_back_tx) GUI.DrawTexture(new Rect(0,6*ck,9*ck,11*ck),text_data_back_tx,ScaleMode.StretchToFill);
			GUI.skin.GetStyle("Label").fontSize=7*ck/15;
			cr=new Rect(0,6*ck,ck*9,ck);
			GUI.Label(cr,"выход реактора: "+(maxcapacity*reactor_bonus).ToString()+"/"+(overload_capacity*reactor_bonus).ToString()); cr.y+=ck;
			GUI.Label(cr,"потребление: "+constant_supply); cr.y+=ck;
			GUI.Label(cr,"свободная энергия: "+((int)(maxcapacity*reactor_bonus-constant_supply)).ToString()); cr.y+=ck;
			GUI.Label(cr,"скорость: "+(maxspeed1*speed_bonus).ToString()+"/"+(maxspeed2*speed_bonus).ToString()+"/"+(maxspeed3*speed_bonus).ToString()); cr.y+=ck;
			GUI.Label(cr,"маневренность: "+(angle_acceleration*maneuver_bonus)+"/"+angle_acceleration*maneuver_bonus*1.2f+"/"+angle_acceleration*maneuver_bonus*0.8f); cr.y+=ck;
			GUI.Label(cr,"дальность радара: "+radar_distance*radar_bonus); cr.y+=ck;
			GUI.Label(cr,"получаемый урон: "+(int)(armor_bonus*100)+"%/"+(1-nose_armor)*100+"%/"+(1-leftwing_armor)*100+"%/"+(1-rightwing_armor)*100+"%/"+(1-bottom_armor)*100+"%"); cr.y+=ck;
			GUI.Label(cr,"длительность респауна: "+respawn_bonus*100+"%"); cr.y+=ck;
			GUI.Label(cr,"бонус орудий: +"+(weapons_bonus*100-100).ToString()+"%");cr.y+=ck;
			GUI.Label(cr,"бонус щита: +"+((int)(shields_bonus*100-100)).ToString()+"% ("+10000*shields_bonus +")");cr.y+=ck;
			GUI.Label(cr,"потребление щита: "+shields_cons.ToString());cr.y+=ck;
			if (active_armor_cons>0) {GUI.Label(cr,"затраты активной брони: "+active_armor_cons.ToString());cr.y+=ck;}
			if (taran_cover) {GUI.Label(cr,"есть таранное покрытие");cr.y+=ck;}

			GUI.skin.GetStyle("Label").fontSize=res_font;
			GUI.skin=menu_skin;
			cr=new Rect(31*ck,ogsh-10*ck,10*ck,3*ck);
			GUI.skin.GetStyle("Button").fontSize=fs;
			GUI.skin.GetStyle("Label").fontSize=fs;
			if (GUI.Button(cr,"Сохранить и выйти")) {PlayerPrefs.SetString("shipModules",shipModules);Application.LoadLevel(0);};cr.y+=3*ck;
			if (GUI.Button(cr,"Выйти без сохранения ")) {Application.LoadLevel(0);};cr.y+=3*ck;
			if (GUI.Button(cr,"Очистить")) {chosen_slot=255;shipModules = new string ('0', 56);SpawnModules(shipModules);};
	}

	void ModulesPick (byte slot) {

		bool right=false;bool small=true;bool bottom=false;bool firstRow=false;bool special=false;bool wing=false;
		switch (slot) {
		case 0: special=true;break;
		case 1:bottom=true;break;
		case 2: firstRow=true;bottom=true;;break;
		case 3:small=false;break;
		case 4: small=false;break;
		case 5: firstRow=true;break;
		case 7:right=true;break;
		case 8: right=true;firstRow=true;break;
		case 9:right=true;small=false;break;
		case 10: right=true;small=false;break;
		case 11:right=true;firstRow=true;bottom=true;break;
		case 12:right=true;bottom=true;break;
		case 13:bottom=true;break;
		case 14: firstRow=true;bottom=true;break;
		case 15: small=false;break;
		case 16: small=false;break;
		case 17:firstRow=true;break;
		case 19: right=true;break;
		case 20:right=true;firstRow=true;break;
		case 21:right=true;small=false;break;
		case 22:right=true;small=false;break;
		case 23:right=true;firstRow=true;bottom=true;break;
		case 24:right=true;bottom=true;break;
		case 25:special=true;wing=true;break;
		case 26:special=true;bottom=true;break;
		case 27:special=true;right=true;wing=true;break;
		}
		byte a=0; 

		if (special) {
			if (wing) {
				module_pick=new GameObject[5];
				module_pick[1]=modules[33];
				if (right) {module_pick[2]=modules[32];} else {module_pick[2]=modules[31];}
				module_pick[3]=modules[34];
				module_pick[4]=modules[35];
			}
			else {
				if (bottom) {//fin
					module_pick=new GameObject[6];
					for (a=26;a<31;a++) {module_pick[a-25]=modules[a];}
				}
				else { //stem
					module_pick=new GameObject[6];
					for (a=21;a<26;a++) {module_pick[a-20]=modules[a];}
				}}
		}
		else {
			if (small) {
				module_pick=new GameObject[8];
				module_pick[1]=modules[2];
				module_pick[2]=modules[4];
				module_pick[3]=modules[6];
				module_pick[4]=modules[7];
				module_pick[5]=modules[8];
				if (right==bottom) {module_pick[6]=modules[5];} else {module_pick[6]=modules[12];}
				if (firstRow) {
					if (right==bottom) {module_pick[7]=modules[9];} else {module_pick[7]=modules[11];}
				}
				else {module_pick[7]=modules[10];}
			}
			else {
				module_pick=new GameObject[8];
				for (a=14;a<21;a++) {module_pick[a-13]=modules[a];}
			}
		}
	}

	void FocusCamera () {
		Vector3 np=cam.transform.position;
		if (cam.transform.position.x/camera_target.x<0) np.x*=-1;
		if (cam.transform.position.y/camera_target.y<0) np.y*=-1;
		cam.transform.position=np;
		cam.transform.LookAt (camera_target);
	}

	public void SpawnModule(int pos, byte id) {
		if (ship_modules[pos]!=null) ClearSlot(pos);
		if (id==0) return;
		ModuleInfo loaded_module_info;
		GameObject module_prefab;
		Vector3 zero_point=Vector3.zero;
		Vector3 correction_vector;
		bool need_doubler=false;
		module_prefab=modules[id];
		loaded_module_info=module_prefab.GetComponent<ModuleInfo>();
		correction_vector=loaded_module_info.correction_vector;
		switch (pos) {
		case 0:zero_point=new Vector3(0,0,21);break;
			//left board-first
		case 1:zero_point=new Vector3(-0.6f,-4,13);break;
		case 2:zero_point=new Vector3(-1.3f,-3,13);break;
		case 3:zero_point=new Vector3(-2,-1.5f,13);break;
		case 4:zero_point=new Vector3(-2,1.5f,13);break;
		case 5:zero_point=new Vector3(-1.3f,3,13);break;
		case 6:zero_point=new Vector3(-0.6f,4,13);break;
			//right board - first
		case 7:zero_point=new Vector3(0.6f,4,13);break;
		case 8:zero_point=new Vector3(1.3f,3,13);break;
		case 9:zero_point=new Vector3(2,1.5f,13);break;
		case 10:zero_point=new Vector3(2,-1.5f,13);break;	
		case 11:zero_point=new Vector3(1.3f,-3,13);break;
		case 12:zero_point=new Vector3(0.6f,-4,13);break;
			//left board-second
		case 13:zero_point=new Vector3(-0.7f,-4,-1);break;
		case 14:zero_point=new Vector3(-1.9f,-3,-1);break;
		case 15:zero_point=new Vector3(-3,-1.5f,-1);break;
		case 16:zero_point=new Vector3(-3,1.5f,-1);break;
		case 17:zero_point=new Vector3(-1.9f,3,-1);break;
		case 18:zero_point=new Vector3(-0.7f,4,-1);break;
			//right board - second
		case 19:zero_point=new Vector3(0.7f,4,-1);break;
		case 20:zero_point=new Vector3(1.9f,3,-1);break;
		case 21:zero_point=new Vector3(3,1.5f,-1);break;
		case 22:zero_point=new Vector3(3,-1.5f,-1);break;	
		case 23:zero_point=new Vector3(1.9f,-3,-1);break;
		case 24:zero_point=new Vector3(0.7f,-4,-1);break;

		case 25:zero_point=new Vector3(-4,0,-15);break;
		case 26:zero_point=new Vector3(0,-5.5f,-15);break;
		case 27:zero_point=new Vector3(4,0,-15);break;
		}
		GameObject x=Instantiate(module_prefab, ship.transform.position,ship.transform.rotation) as GameObject;
		x.transform.localScale=new Vector3(1,1,1);
		x.transform.parent=ship.transform;
		ModuleInfo mi=x.GetComponent<ModuleInfo>();
		switch (mi.type) {
		case 0: 
			if (mi.symmetrical) {
				if (zero_point.x>0) {
					if (zero_point.y>0) x.transform.Rotate(new Vector3(0,180,0));
					else x.transform.Rotate(new Vector3(0,0,180));
				}
				else {if (zero_point.y<0) x.transform.Rotate(new Vector3(180,0,0));}
			}
			else {if (zero_point.y<0) x.transform.Rotate(new Vector3(0,0,180));}
			break;
		case 1: if (zero_point.x>0) x.transform.Rotate(new Vector3(0,180,0)); break;
		case 4: if (zero_point.x>0&&mi.symmetrical) x.transform.Rotate(new Vector3(0,0,180));break;
		}
		x.transform.localPosition=zero_point+correction_vector;
		ship_modules[pos]=x;
		if (x.GetComponent<bonus>()) {
			bonus b= x.GetComponent<bonus>();
			switch (b.bonus_type) {
			case 0: reactor_bonus+=b.value;break;
			case 1: maneuver_bonus+=b.value;break;
			case 2: speed_bonus+=b.value;need_doubler=true;break;
			case 3: weapons_bonus+=b.value;break;
			case 4: radar_bonus+=b.value;break;
			case 5: if (respawn_bonus>0.5f) respawn_bonus*=(1-b.value); else {Destroy(x.GetComponent<bonus>());}break;
			case 6: shields_bonus+=b.value;break;
			case 7:armor_bonus*=(1-b.value);break;
			case 8: nose_armor+=b.value;break;
			case 9: taran_cover=true;break;
			case 11: bottom_armor+=b.value;break;
			case 12: leftwing_armor+=b.value;break;
			case 13: rightwing_armor+=b.value;break;
			}
			if (b.second_bonus_type!=10) {
				switch (b.second_bonus_type) {
				case 0: reactor_bonus+=b.value2;break;
				case 1: maneuver_bonus+=b.value2;break;
				case 2: speed_bonus+=b.value2;need_doubler=true;break;
				case 3: weapons_bonus+=b.value2;break;
				case 4: radar_bonus+=b.value2;break;
				case 5: if (respawn_bonus>0.5f) respawn_bonus*=(1-b.value); else {Destroy(x.GetComponent<bonus>());}break;
				case 6: shields_bonus+=b.value2;break;
				case 7:armor_bonus*=(1-b.value2);break;
				case 8: nose_armor+=b.value2;break;
				case 9: taran_cover=true;break;
				case 11: bottom_armor+=b.value2;break;
				case 12: leftwing_armor+=b.value2;break;
				case 13: rightwing_armor+=b.value2;break;
				}
			}
			constant_supply+=b.energy_consumption;
		}
		if (ship_modules[pos].GetComponent<active_armor>()) {active_armor_cons+=ship_modules[pos].GetComponent<active_armor>().consumption;}
		string mid=id.ToString();
		if (id<10) mid='0'+mid;
		shipModules=shipModules.Substring(0,2*pos)+mid+shipModules.Substring(pos*2+2,shipModules.Length-2-2*pos);
		if (need_doubler) {
			if (pos==0) return;
			if (pos==25) {if (shipModules.Substring(54,2)!="35") SpawnModule(27,35);}
			else {if (pos==27) {if (shipModules.Substring(50,2)!="35") SpawnModule(25,35);}
			else {
				if (pos<13) {pos=13-pos;}
				else {pos=37-pos;}
				if (id==12) {if (shipModules.Substring(pos*2,2)!="05") SpawnModule(pos,5);}
				if (id==5) {if (shipModules.Substring(pos*2,2)!="12")  SpawnModule(pos,12);}
			}
		}
		}
	}

	public void ClearSlot(int pos) {
		if (ship_modules[pos]==null) return;
		bonus b=ship_modules[pos].GetComponent<bonus>();
		bool remove_doubler=false;
		if (b) {
			switch (b.bonus_type) {
			case 0: reactor_bonus-=b.value;break;
			case 1: maneuver_bonus-=b.value;break;
			case 2: speed_bonus-=b.value;remove_doubler=true;break;
			case 3: weapons_bonus-=b.value;break;
			case 4: radar_bonus-=b.value;break;
			case 5: if (respawn_bonus>0.5f) respawn_bonus/=(1-b.value);break;
			case 6: shields_bonus-=b.value;break;
			case 7:armor_bonus/=(1-b.value);break;
			case 8: nose_armor-=b.value;break;
			case 9: taran_cover=false;break;
			case 11: bottom_armor-=b.value;break;
			case 12: leftwing_armor-=b.value;break;
			case 13: rightwing_armor-=b.value;break;
			}
			if (b.second_bonus_type!=10) {
				switch (b.second_bonus_type) {
				case 0: reactor_bonus-=b.value2;break;
				case 1: maneuver_bonus-=b.value2;break;
				case 2: speed_bonus-=b.value2;remove_doubler=true;break;
				case 3: weapons_bonus-=b.value2;break;
				case 4: radar_bonus-=b.value2;break;
				case 5: if (respawn_bonus>0.5f) respawn_bonus/=(1-b.value);break;
				case 6: shields_bonus-=b.value2;break;
				case 7:armor_bonus/=(1-b.value2);break;
				case 8: nose_armor-=b.value2;break;
				case 9: taran_cover=false;break;
				case 11: bottom_armor-=b.value2;break;
				case 12: leftwing_armor-=b.value2;break;
				case 13: rightwing_armor-=b.value2;break;
				}
			}
			constant_supply-=b.energy_consumption;
		}
		if (ship_modules[pos].GetComponent<active_armor>()) {active_armor_cons-=ship_modules[pos].GetComponent<active_armor>().consumption;}
		Destroy(ship_modules[pos]);
		ship_modules[pos]=null;
		shipModules=shipModules.Substring(0,2*pos)+"00"+shipModules.Substring(pos*2+2,shipModules.Length-2-2*pos);
		if (remove_doubler) {
			if(pos==25) {if (ship_modules[27]!=null) ClearSlot(27);}
			else {
				if (pos==27) {if (ship_modules[25]!=null) ClearSlot(25);}
				else {
					if (pos<13) {if (ship_modules[13-pos]!=null) ClearSlot(13-pos);}
					else {if (ship_modules[37-pos]!=null) ClearSlot(37-pos);}
				}
			}
		}
	}

	public void SpawnModules (string s) {
		if (ship_modules.Length!=0) {
			foreach (GameObject module in ship_modules) {
				if (module) {
					if (module.GetComponent<bonus>()) {
						bonus b= module.GetComponent<bonus>();
						switch (b.bonus_type) {
						case 0: reactor_bonus-=b.value;break;
						case 1: maneuver_bonus-=b.value;break;
						case 2: speed_bonus-=b.value;break;
						case 3: weapons_bonus-=b.value;break;
						case 4: radar_bonus-=b.value;break;
						case 5: respawn_bonus/=(1-b.value);break;
						case 6: shields_bonus-=b.value;break;
						case 7:armor_bonus/=(1-b.value);break;
						case 8: nose_armor-=b.value;break;
						case 9: taran_cover=false;break;
						case 11: bottom_armor-=b.value;break;
						case 12: leftwing_armor-=b.value;break;
						case 13: rightwing_armor-=b.value;break;
						}
						if (b.second_bonus_type!=10) {
							switch (b.second_bonus_type) {
							case 0: reactor_bonus-=b.value2;break;
							case 1: maneuver_bonus-=b.value2;break;
							case 2: speed_bonus-=b.value2;break;
							case 3: weapons_bonus-=b.value2;break;
							case 4: radar_bonus-=b.value2;break;
							case 6: shields_bonus-=b.value2;break;
							case 7:armor_bonus/=(1-b.value2);break;
							case 8: nose_armor-=b.value2;break;
							case 9: taran_cover=false;break;
							case 11: bottom_armor-=b.value2;break;
							case 12: leftwing_armor-=b.value2;break;
							case 13: rightwing_armor-=b.value2;break;
							}
						}
						constant_supply-=b.energy_consumption;
					}
					Destroy(module);
					maxhp=20000;
					reactor_bonus=1;
					maxcapacity=1000;overload_capacity=4000;
					constant_supply=0;
					maxspeed1=15;maxspeed2=40;maxspeed3=400;speed_bonus=1f;
					angle_acceleration=1f;maneuver_bonus=1f;
					radar_bonus=1f;radar_distance=5000;
					armor_bonus=1f;nose_armor=0;leftwing_armor=0;rightwing_armor=0;  bottom_armor=0;
					respawn_bonus=1f;
					weapons_bonus=1f;
					shields_bonus=1f;
					taran_cover=false;
				}
			} ship_modules=new GameObject[28];}
		for (var i=0;i<28;i++) {
			if (s.Substring(2*i,2)!="00") {SpawnModule(i,byte.Parse(s.Substring(2*i,2)));}
		}
	}
}
