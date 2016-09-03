using UnityEngine;
using System.Collections;

public class mission2 : MonoBehaviour {
	public GameObject crystal_pref;
	public GameObject viol_expl;
	GameObject ship;
	public int step=400;
	fleetCommand our_fc;
	public NetworkShipGUI nsg;
	Rect r;
	float carriers_distance;
	public int maxDistance=2500;
	bool fail=false;
	string reasonwhy;
	bool game_started=false;
	int lastz=0;
	public int zlimit=10000;
	bool win=false;
	Texture back_tx;
	int m_count=0;

	// Use this for initialization
	void StartGame () {
		ship=GameObject.Find("menu").GetComponent<SceneResLoader>().localShip;
		lastz=(int)ship.transform.position.z+step;
		nsg=ship.GetComponent<NetworkShipGUI>();
		our_fc=GameObject.Find("fc1").GetComponent<fleetCommand>();
		our_fc.ScanOnce();
		foreach(GameObject s in our_fc.ships) {
			if (s==null) continue;
			s.SendMessage("FollowInCurrentFormation",ship,SendMessageOptions.DontRequireReceiver);
			}
		r= new Rect(Screen.width/2-2*nsg.k,0,4*nsg.k,nsg.k/2);
		StartCoroutine(Awaiting());
		back_tx=Resources.Load<Texture>("text_back");
	}

	IEnumerator Awaiting () {
		yield return new WaitForSeconds(3);
		game_started=true;
	}

	void SpawnEnemies () {
		Vector3 pos=new Vector3(ship.transform.position.x,ship.transform.position.y,lastz+1000);
		for (byte j=0;j<9;j++) 
		{  if (Random.value<=0.5f) continue;
			switch (j) {
			case 1: pos.y+=5*step;break;
			case 2: pos+=new Vector3(-2.5f*step,2.5f*step,step/2);break;
			case 3: pos.x-=5*step;break;
			case 4: pos+=new Vector3(-2.5f*step,-2.5f*step,step/2);break;
			case 5: pos.y-=5*step;break;
			case 6: pos+=new Vector3(2.5f*step,-2.5f*step,step/2);break;
			case 7: pos.x+=5*step;break;
			case 8: pos+=new Vector3(2.5f*step,2.5f*step,step/2);break;
			}
			for (byte i=0;i<13;i++) {
			if (Random.value<=0.5f) continue;
				switch (i) {
				case 1: pos.y+=2*step; break;
				case 2: pos.y+=step;pos.x-=step;break;
				case 3: pos.y+=step;break;
				case 4: pos.y+=step;pos.x+=step;break;
				case 5: pos.x-=2*step;break;
				case 6: pos.x-=step;break;
				case 7:pos.x+=step;break;
				case 8: pos.x+=2*step;break;
				case 9: pos.x-=step;pos.y-=step;break;
				case 10: pos.y-=step;break;
				case 11: pos.y-=step;pos.x+=step;break;
				case 12: pos.y-=2*step;break;
				}
				GameObject c=Instantiate(crystal_pref,pos,Quaternion.Euler(0,180,0)) as GameObject;
				c.transform.localScale*=(int)(Random.value*10);
				BotControl_crystal bcc=c.GetComponent<BotControl_crystal>();
				bcc.maxhp=(int)(2500*c.transform.localScale.x);
				bcc.damage=(int)(50*c.transform.localScale.x);
				bcc.range=250*c.transform.localScale.x;
				bcc.crystal_pref=crystal_pref;
		}}
	}

	void Update() {
		if (!game_started) return;
		if (!ship) {if (game_started&&!fail) {fail=true;reasonwhy="Вы погибли слишком рано!";}return;}
		float d=ship.transform.position.z;d/=zlimit;
		int count=0;
		for (byte i=0;i<our_fc.ships.Length;i++) {
			if (our_fc.ships[i]!=null) {				
				float d2=Vector3.Distance(ship.transform.position,our_fc.ships[i].transform.position);
				if (d>maxDistance&&our_fc.ships[i].name[3]!='h') Destroy(our_fc.ships[i]);
				if (our_fc.ships[i].name[5]=='m') {
				count++;d+=d2;
				}
				if (our_fc.ships[i].transform.position.z>=zlimit) {
					Instantiate(viol_expl,our_fc.ships[i].transform.position,Quaternion.identity);
					our_fc.ships[i].BroadcastMessage("SetLayer",9,SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		if (win||fail) return;
		m_count=count;
		if (count!=0) {	carriers_distance=d/count;} else {fail=true;reasonwhy="Мы потеряли всех шахтеров!";return;}
		if (ship.transform.position.z>lastz) {lastz+=step;SpawnEnemies();}
		if (ship.transform.position.z>=zlimit) {
			Instantiate(viol_expl,ship.transform.position,Quaternion.identity);
			ship.BroadcastMessage("SetLayer",9,SendMessageOptions.DontRequireReceiver);
			win=true;
			r=new Rect(Screen.width/2-3*nsg.k,Screen.height/2-nsg.k,6*nsg.k,4*nsg.k);
			nsg.enabled=false;
			if (PlayerPrefs.HasKey("campaign_mission")) {
				if (PlayerPrefs.GetInt("campaign_mission")<3) {PlayerPrefs.SetInt("campaign_mission",3);Global.campaign_mission=3;}}
		}
	}

	void OnGUI () {
		if (!fail) {
			if (nsg&&nsg.enabled) {
				GUI.skin=Global.mySkin;
				int fs=GUI.skin.GetStyle("Label").fontSize;
				GUI.skin.GetStyle("Label").fontSize=(int)(r.height/2);
				float dole=ship.transform.position.z;dole/=zlimit;
			if (dole>1) dole=1;
			GUI.DrawTexture(new Rect(r.x,r.y,r.width*dole,r.height),nsg.h_green_line,ScaleMode.StretchToFill);
			GUI.DrawTexture(r,nsg.h_bar_frame,ScaleMode.StretchToFill);
			GUI.Label(r,"Завершено");
			dole=carriers_distance/maxDistance;
			if (dole>1) dole=1;
			Texture t;
			if (dole>0.8 ) t=nsg.h_red_line; else t=nsg.h_lblue_line;
			GUI.DrawTexture(new Rect(r.x,r.y+nsg.k/2,r.width*dole,r.height),t,ScaleMode.StretchToFill);
			GUI.DrawTexture(new Rect(r.x,r.y+nsg.k/2,r.width,r.height),nsg.h_bar_frame,ScaleMode.StretchToFill);
				GUI.Label(new Rect(r.x,r.y+nsg.k/2,r.width,r.height),"Удаленность ("+m_count+")");
				GUI.skin.GetStyle("Label").fontSize=fs;
		}
			if (win) {
				GUI.skin=Global.stSkin;
				GUI.DrawTexture(r,back_tx,ScaleMode.StretchToFill);
				GUI.Label(r,"Миссия успешно завершена!");
				if (GUI.Button(new Rect(r.x,r.y+nsg.k,3*nsg.k,nsg.k),"Продолжить")) {
					nsg.st.nm.onlineScene="mission3";
					nsg.st.nm.StopHost();
					nsg.st.nm.StartHost();
					Destroy(gameObject);}
				if (GUI.Button(new Rect(r.x+3*nsg.k,r.y+nsg.k,3*nsg.k,nsg.k),"Выйти в меню")) {
					nsg.st.nm.StopHost();
					Destroy(gameObject);
				}
			}
	}
		else {
			GUI.skin=Global.stSkin;
			Rect r2=new Rect(Screen.width/2-128,Screen.height/2-32,256,64);
			GUI.DrawTexture(r2,back_tx,ScaleMode.StretchToFill);
				GUI.Label(r2,"Поражение! "+reasonwhy);r2.width/=2;
			if (GUI.Button(new Rect(r2.x,r2.y+64,128,64),"Перезапуск")) GameObject.Find("server_terminal").GetComponent<ServerTerminal>().Restart();
			    r2.x+=128;
			if (GUI.Button(new Rect(r2.x+64,r2.y+64,128,64),"Выход")) GameObject.Find("server_terminal").GetComponent<ServerTerminal>().Exit();
		}
	}

	public void MissionFailed () {
		fail=true;
		reasonwhy="Вы сбежали с поля боя";
	}
}
