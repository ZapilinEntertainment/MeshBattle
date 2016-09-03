using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class cooperative_mission : NetworkBehaviour {
	public fleetCommand our_fc;
	bool win=false;
	bool game_started=false;
	// Update is called once per frame
	public void StartGame() {StartCoroutine(Awaiting());}

	IEnumerator Awaiting() {
		yield return new WaitForSeconds(5);
		game_started=true;
	}

	void Update () {
		if (game_started&&!win) {
			if (our_fc.enemies.Length==0) win=true;
		}
	}

	void OnGUI () {
		if (!game_started) return;
		if (!win) {GUI.Label(new Rect(0,0,128,64),"врагов:"+our_fc.enemies.Length);}
		else {
		int sw=Screen.width;
		int sh=Screen.height;
		GUI.Label(new Rect(sw/2-128,sh/2-32,256,64),"Победа!");
		if (GUI.Button(new Rect(sw/2-128,sh/2+32,128,32),"Продолжить")) {Destroy(this);	}
		if (GUI.Button(new Rect(sw/2,sh/2+32,128,32),"Закончить")) {
			NetworkManager nm=GameObject.Find("networkManager").GetComponent<NetworkManager>();
			if (isServer) nm.StopHost();
			else nm.StopClient();
		}
		}}
}
