using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class Menu : NetworkBehaviour {
	public GUISkin menu_skin;
	public GUISkin mySkin;
	ushort g;
	byte window=0;
	int ogsw;
	int sh;
	byte campaign_mission=2;
	int quality_level;
	byte cam_rot_speed;
	byte cam_zoom_speed;
	byte looking_mission;
	public NetworkManager nm;
	public GameObject ship_prefab;

	string ip="127.0.0.1";
	int port=21025;
	string port_s;

	Texture xsun_tx;
	Texture menu_back_tx;
	public Texture logo_tx;


	void Start () {
		sh=Screen.height;
		g=(ushort)(sh/9);
		xsun_tx=Resources.Load<Texture>("xsun");
		menu_back_tx=xsun_tx;
		menu_skin.GetStyle("Button").fontSize=(int)(g/2.5);
		menu_skin.GetStyle("Label").fontSize=(int)(g/2.5);
		menu_skin.GetStyle("TextField").fontSize=(int)(g/2.5);
		ogsw=Screen.width;
		nm=GameObject.Find("networkManager").GetComponent<NetworkManager>();
		if (PlayerPrefs.HasKey("campaign_mission")) {Global.campaign_mission=(byte)PlayerPrefs.GetInt("campaign_mission");}
		else {PlayerPrefs.SetInt("campaign_mission",0);Global.campaign_mission=0;} 
		Global.campaign_mission=3;

		if (PlayerPrefs.HasKey("gui_piece")) {Global.gui_piece=PlayerPrefs.GetInt("gui_piece");}
		else {
			if (sh*(13/9)>ogsw) {
				Global.gui_piece=ogsw/32;//widescreen 16:9
			}
			else {Global.gui_piece=ogsw/24;}//quadscreen 4:3
			PlayerPrefs.SetInt("gui_piece",Global.gui_piece);}


		int x;
		if (PlayerPrefs.HasKey("drawDist1")) {Global.drawDist1=PlayerPrefs.GetInt("drawDist1");}
		else {x=Mathf.RoundToInt(Screen.height/768*300);print (x);PlayerPrefs.SetInt("drawDist1",x);Global.drawDist1=x;} 

		if (PlayerPrefs.HasKey("drawDist2")) {Global.drawDist2=PlayerPrefs.GetInt("drawDist2");}
		else {x=Mathf.RoundToInt(Screen.height/768*700);print(x);PlayerPrefs.SetInt("drawDist2",x);Global.drawDist2=x;}

		if (PlayerPrefs.HasKey("cam_rot_speed")) {Global.cam_rot_speed=(byte)PlayerPrefs.GetInt("cam_rot_speed");}
		else {PlayerPrefs.SetInt("cam_rot_speed",70);Global.cam_rot_speed=50;} 

		if (PlayerPrefs.HasKey("cam_zoom_speed")) {Global.cam_zoom_speed=(byte)PlayerPrefs.GetInt("cam_zoom_speed");}
		else {PlayerPrefs.SetInt("cam_zoom_speed",50);Global.cam_zoom_speed=70;} 

		campaign_mission=Global.campaign_mission;
		quality_level=QualitySettings.GetQualityLevel();

		nm.offlineScene="menu";
		nm.playerPrefab=ship_prefab;
		ClientScene.RegisterPrefab(ship_prefab);
	}






	void OnGUI () {
		int fs=GUI.skin.GetStyle("Button").fontSize;
		GUI.skin=menu_skin;
		float n=menu_back_tx.width;n/=ogsw; Rect rx;
		if (menu_back_tx.height/n<sh) {n=menu_back_tx.height;n/=sh;rx=new Rect(0,0,menu_back_tx.width/n,sh);}
		else rx=new Rect(0,0,ogsw,menu_back_tx.height/n);
		GUI.DrawTexture(rx,menu_back_tx,ScaleMode.ScaleToFit);
		GUI.DrawTexture(new Rect(ogsw-4*g,0,4*g,2*g),logo_tx,ScaleMode.ScaleToFit);
		switch (window) {
		case 0:
			GUI.skin.GetStyle("Button").fontSize=4*g/10;
			Rect mr=new Rect(ogsw-4*g,3*g,4*g,g);
			if (GUI.Button(mr,"Сюжет")) {window=1;} mr.y+=g;
			if (GUI.Button(mr,"Схватка")) {window=2;}mr.y+=g;
			if (GUI.Button(mr,"По сети")) 
			{
				window=3;
				if (PlayerPrefs.HasKey("last_conn_info")) 
				{
					string ci=PlayerPrefs.GetString("last_conn_info");
					if (ci.Length>8) 
					{
						int pi=ci.IndexOf("|");
						if (int.TryParse(ci.Substring(pi,ci.Length-pi),out port)) port=int.Parse(ci.Substring(pi+1,ci.Length-pi-1));
						else port=4444;
						port_s=ci.Substring(pi+1,ci.Length-pi-1);
						ip=ci.Substring(0,pi);
					}
				}}mr.y+=g;
			if (GUI.Button(mr,"Customizer")) {Application.LoadLevel("custom");}mr.y+=g;
			if (GUI.Button(mr,"Настройки")) {window=4;}mr.y+=g;
			if (GUI.Button(mr,"Выход")) {Application.Quit();}
			break;
		case 1:
			GUI.Box(new Rect(ogsw-4*g,3*g,4*g,5*g),"");
			for (byte i=1;i<=Global.campaign_mission;i++) {
					if (i<6) {
						if (GUI.Button(new Rect(ogsw-4*g,(2+i)*g,2*g,g),i.ToString())) {LookMission(i);}
					}
					else {if (GUI.Button(new Rect(ogsw-2*g,(i-3)*g,2*g,g),i.ToString())) {LookMission(i);}}
				}

			if (campaign_mission!=10) {
				if (campaign_mission!=0) {if (GUI.Button(new Rect(ogsw-4*g,2*g,4*g,g),"Продолжить")) {LookMission(campaign_mission);}}
				else {if (GUI.Button(new Rect(ogsw-4*g,2*g,4*g,g),"Начать")) {IntroMovie();}}
				if (GUI.Button(new Rect(ogsw-4*g,8*g,4*g,g),"Вернуться")) {window=0;}
			}
			break;
		case 2:
			if (GUI.Button(new Rect(ogsw-4*g,8*g,4*g,g),"Вернуться")) {window=0;}
			break;
		case 3:
				GUI.skin=menu_skin;
				GUI.skin.GetStyle("Button").fontSize=4*g/10;
				Rect nr=new Rect(ogsw/2-5*g,4*g,5*g,g/2);		  
			GUI.Label(nr,"ip"); nr.y+=g/2;
			ip=GUI.TextField(nr,ip); nr.y+=g/2;

			GUI.Label(nr,"port"); nr.y+=g/2;
			port_s=GUI.TextField(nr,port_s); nr.y-=1.5f*g; nr.x+=5*g;nr.height=g;

			if (GUI.Button(nr,"Host")) 
			{
				if (nm.networkAddress!=ip) {nm.networkAddress=ip;}          
				port=int.Parse(port_s);
				if (nm.networkPort!=port) {nm.networkPort=port;port_s=port.ToString();}
				while (port_s.Length<5) {port_s='0'+port_s;}
				if (port_s.Length>5) {port_s=port_s.Substring(port_s.Length-5,5);}
				PlayerPrefs.SetString("last_conn_info",ip+'|'+port.ToString());
				nm.onlineScene="stratophor";
				nm.StartHost();	
				Destroy(gameObject);
			} 
			nr.y+=g;
			if (GUI.Button(nr,"Connect")) {
				if (nm.networkAddress!=ip) {nm.networkAddress=ip;}          
				port=int.Parse(port_s);
				if (nm.networkPort!=port) {nm.networkPort=port;port_s=port.ToString();}
				while (port_s.Length<5) {port_s='0'+port_s;}
				if (port_s.Length>5) {port_s=port_s.Substring(port_s.Length-5,5);}
				PlayerPrefs.SetString("last_conn_info",ip+'|'+port_s);
				nm.onlineScene="stratophor";
				nm.StartClient();
				Destroy(gameObject);
			}nr.y+=g;
				if (GUI.Button(nr,"Вернуться")) {window=0;}
			break;
		case 4:
			Rect r = new Rect(ogsw-9*g,0,4*g,g);
			GUI.Label(r,"размер GUI   ["+Global.gui_piece.ToString()+"]");
			r.y+=g;r.width-=g;
			Global.gui_piece=(int)(GUI.HorizontalSlider(r,Global.gui_piece,32,Mathf.RoundToInt(Screen.height/12)));

			r.y+=g;r.width+=g;
			GUI.Label(r,"Первый порог оптимизации   ["+Global.drawDist1.ToString()+"]");
			r.y+=g;r.width-=g;
			Global.drawDist1=(int)(GUI.HorizontalSlider(r,Global.drawDist1,0,3000));

			r.y+=g;r.width+=g;
			GUI.Label(r,"Второй порог оптимизации   ["+Global.drawDist2.ToString()+"]");
			r.y+=g;r.width-=g;
			Global.drawDist2=(int)(GUI.HorizontalSlider(r,Global.drawDist2,0,7000));


			r.y+=g;r.width+=g;
			GUI.Label(r,QualitySettings.names[quality_level]+"  Графон");
			r.y+=g;
			quality_level=(int)(GUI.HorizontalSlider(r,quality_level,0,QualitySettings.names.Length-1));

			r.x=ogsw-4*g;r.y=2*g;
			GUI.Label(r,"Скорость вращения камеры  ["+Global.cam_rot_speed.ToString()+"]");
			r.y+=g;
			Global.cam_rot_speed=(byte)(GUI.HorizontalSlider(r,Global.cam_rot_speed,10,120));
			r.y+=g;
			GUI.Label(r,"Скорость приближения камеры   ["+Global.cam_zoom_speed.ToString()+"]");
			r.y+=g;
			Global.cam_zoom_speed=(byte)(GUI.HorizontalSlider(r,Global.cam_zoom_speed,10,120));



			if (GUI.Button(new Rect(ogsw-4*g,8*g,4*g,g),"Вернуться")) {
				window=0;
				PlayerPrefs.SetInt("gui_piece",Global.gui_piece);
				PlayerPrefs.SetInt("drawDist1",Global.drawDist1);
				PlayerPrefs.SetInt("drawDist2",Global.drawDist2);
				PlayerPrefs.SetInt("cam_rot_speed",Global.cam_rot_speed);
				PlayerPrefs.SetInt("cam_zoom_speed",Global.cam_zoom_speed);
				if (quality_level!=QualitySettings.GetQualityLevel()) QualitySettings.SetQualityLevel(quality_level,true);
			}
			GUI.skin=null;
			if (GUI.Button(new Rect(ogsw-6*g,g,g,g),ResLoad.default_tx)) {Global.gui_piece=sh/12;}
			if (GUI.Button(new Rect(ogsw-6*g,5*g,g,g),ResLoad.default_tx)) {Global.drawDist2=(int)(sh/768f*700);}
			if (GUI.Button(new Rect(ogsw-6*g,3*g,g,g),ResLoad.default_tx)) {Global.drawDist1=(int)(sh/768f*300);}
			break;

		//mission texts and info
		case 5:
			GUI.skin=menu_skin;
			if (GUI.Button(new Rect(ogsw-4*g,2*g,4*g,g),"Вернуться")) {menu_back_tx=xsun_tx;window=1;}
			if (GUI.Button(new Rect(ogsw-4*g,3*g,4*g,g),"Начать")) {
				if (looking_mission==0) {
					menu_back_tx=Resources.Load<Texture>("cadalees_background");
					looking_mission=1;
					if (campaign_mission<1) {campaign_mission=1;PlayerPrefs.SetInt("campaign_mission",1);Global.campaign_mission=1;}
				}
				else {
					//loading level
					nm.onlineScene="mission"+looking_mission;
					nm.networkAddress="127.0.0.1";
					nm.networkPort=4444;
					nm.StartHost();	
				}
				}
		break;
		}
	}




	void IntroMovie () {
		menu_back_tx=Resources.Load<Texture>("stratophor_background");
		window=5;
	}

	void LookMission (byte number) {
		window=5;
		looking_mission=number;
		switch (number) {
		case 0: menu_back_tx=Resources.Load<Texture>("stratophor_background");break;
		case 1:menu_back_tx=Resources.Load<Texture>("mission1_back");break;
		case 2: menu_back_tx=Resources.Load<Texture>("mission2_back");break;
		case 3: menu_back_tx=Resources.Load<Texture>("cadalees_background");break;	
			break;
		}
	}


}
