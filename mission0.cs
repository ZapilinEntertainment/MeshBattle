using UnityEngine;
using System.Collections;
using System.IO;

public class mission0 : MonoBehaviour {
	Texture background;
	public TextAsset asset;
	GUISkin menuSkin;
	int ogsw;
	string mission_text;

	void Start () {
		background=Resources.Load<Texture>("stratophor_background");
		asset=Resources.Load<TextAsset>("mission0");
		menuSkin=Resources.Load<GUISkin>("menu_skin");
		ogsw=Screen.width;
		mission_text=asset.text;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		GUI.skin=Global.mySkin;
		GUI.DrawTexture(new Rect(0,0,ogsw,Screen.height),background);
		GUI.Label(new Rect(0,0,ogsw/2,Screen.height),mission_text);
		GUI.skin=menuSkin;
		int g=Screen.height/9;
		if (GUI.Button(new Rect(ogsw-2*g,0,2*g,g),"Вернуться")) {Application.LoadLevel("menu");}
		if (GUI.Button(new Rect(ogsw-2*g,g,2*g,g),"Продолжить")) Application.LoadLevel("mission1"); 
	}
}
