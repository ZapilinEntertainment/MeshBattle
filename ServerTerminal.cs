using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ServerTerminal : NetworkBehaviour {
	public int sum=0;
	public string[] chat;
	byte ch_pos=0;
	int k;
	float chat_time;
	public byte inCommand1=0;
	public byte inCommand2=0;

	[SyncVar]
	bool real_game_started=false;
	bool game_started=false;
	byte players_count=0;
	public Transform main_spawner;
	int sw;
	int sh;
	public NetworkManager nm;
	string system_message;
	bool show_sys_msg=false;
	float sys_msg_timer=0;
	Rect crect1; //central rects
	Rect crect2;

	void Start() {
		k=Global.gui_piece;
		chat=new string[10];
		sw=Screen.width;
		sh=Screen.height;
		crect1=new Rect(sw/2-2*k,sh/2-k,6*k,2*k);
		crect2=crect1;crect2.y+=2*k;
		nm=GameObject.Find("networkManager").GetComponent<NetworkManager>();
		GameObject.Find("menu").GetComponent<SceneResLoader>().offline=false;
	}

	public void Restart() {
		if (isServer) {nm.StopHost();nm.StartHost();}
		else {nm.StopClient();
			system_message="Вы проголосовали за рестарт миссии.";
			show_sys_msg=true;
			sys_msg_timer=7;
			StopAllCoroutines();
		}
	}
	public void Exit() {
		if (isServer) nm.StopHost();
		else nm.StopClient();
		StopAllCoroutines();
		Destroy(this);
	}

	public void Update () {
		if (chat_time>0) {chat_time-=Time.deltaTime;if (chat_time<=0) {
				for (byte a=0;a<ch_pos;a++) {
					chat[a]=chat[a+1];
				}
				chat[ch_pos]=null;ch_pos--;
			}}
		if (sys_msg_timer>0) {
			sys_msg_timer-=Time.deltaTime;
			if (sys_msg_timer<=0) {sys_msg_timer=0;show_sys_msg=false;system_message="";}
		}
	}


	public short PlayerAdded () {
		GameObject[] nms=GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject element in nms) {
			NetworkModuleSpawner ne=element.GetComponent<NetworkModuleSpawner>();
			if (ne!=null) ne.SpawnModulesForAll();
		}
		players_count++;
		if (real_game_started) return((short)(-1*(players_count-1)));
		else {return ((short)(players_count-1));}
	}

	public void AddMessage(string x) {
		if (ch_pos<9) {chat[ch_pos]=x;ch_pos++;}
		else {
			ch_pos=9;
		for (byte a=0;a<8;a++) {
				chat[a]=chat[a+1];
		}
			chat[9]=x;
		}
		chat_time=5;
	}

	public byte GetSpawnPoint (byte x) {
		if (x==1) {inCommand1++;RpcPlayerPlus(1);return ((byte)(inCommand1-1));}
		else {inCommand2++;RpcPlayerPlus(2);return ((byte)(inCommand2-1));}
	}

	[ClientRpc]
	public void RpcPlayerPlus (byte x) {
		if (isServer) return;
		if (x==1) inCommand1++;
		else inCommand2++;
	}


	[ClientRpc]
	public void RpcStartGame() {
		if (isServer) return;
		game_started=true;
		GameObject[] ships=GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject element in ships) {
			element.BroadcastMessage("StartGame",SendMessageOptions.DontRequireReceiver);
			GameObject.Find("menu").BroadcastMessage("StartGame",SendMessageOptions.DontRequireReceiver);
			BroadcastMessage("StartGame",SendMessageOptions.DontRequireReceiver);
		}
	}


	void OnGUI() {
		GUI.skin.GetStyle("Button").fontSize=k/3;
		if (!game_started){
			if (isServer) {if (GUI.Button(crect1,"Начать бой!")) {
					game_started=true;
					GameObject[] ships=GameObject.FindGameObjectsWithTag("Player");
					foreach (GameObject element in ships) {
						element.BroadcastMessage("StartGame",SendMessageOptions.DontRequireReceiver);
						GameObject.Find("menu").BroadcastMessage("StartGame",SendMessageOptions.DontRequireReceiver);
					}
					BroadcastMessage("StartGame",SendMessageOptions.DontRequireReceiver);
					RpcStartGame();
					real_game_started=true;}}
			else {
				if (!real_game_started) GUI.Label(crect1,"Ждем сигнала");
			}}

		if (ch_pos>0) {
			GUI.skin.GetStyle("Label").fontSize=k/6;
			for (byte a=0;a<ch_pos;a++) {
				GUI.Label(new Rect(0,sh-2*k-a*k/3,6*k,k),chat[a]);
			}
		}
		if (GUI.Button(new Rect(sw-2*k,0,2*k,k),"Отключиться")) {
			if (isServer) {
				nm.StopHost();
			}
			else {nm.StopClient();}
		}
		if (show_sys_msg) {
			GUI.Label(crect1,system_message);
		}
	}
}
