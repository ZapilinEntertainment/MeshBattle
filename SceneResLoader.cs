using UnityEngine;
using System.Collections;

public class SceneResLoader : MonoBehaviour {
	public GameObject environmental_camera;
	public GameObject mission_spawn;
	public GameObject hyperjump_exit_effect;
	public Texture hyperjump_exit_sprite;
	public GameObject localShip;
	public int map_radius=25000;

	public LineRenderer special_line;
	public GameObject special_splash;

	public bool offline=false;
	public bool mission=true;
	public string special_composition="";
	public bool use_effects=true;
	public byte effects_limit=50;
	public GameObject[] fires;
	public GameObject[] pieces;
	// Use this for initialization
	void Awake () {
		if (special_line==null) special_line=Resources.Load<LineRenderer>("whiteLaserLine");
		if (special_splash==null) special_splash=Resources.Load<GameObject>("laser_splash");
		ResLoad.pieces=Instantiate(Resources.Load<GameObject>("pieces"),transform.position,Quaternion.identity) as GameObject;
		ResLoad.pieces.SetActive(false);

		if (use_effects) {
		ResLoad.laser_ray=Instantiate(special_line); ResLoad.laser_ray.gameObject.SetActive(false);
		ResLoad.fire=Instantiate(Resources.Load<GameObject>("fire")); ResLoad.fire.SetActive(false);
		ResLoad.lf_explosion=Instantiate(Resources.Load<GameObject>("flare_explosion"));ResLoad.lf_explosion.SetActive(false);
		ResLoad.pieces_s=Instantiate(Resources.Load<GameObject>("pieces_s")); ResLoad.pieces_s.SetActive(false);
		ResLoad.explosion=Instantiate(Resources.Load<GameObject>("explosion")); ResLoad.explosion.SetActive(false);
		ResLoad.small_explosion=Instantiate(Resources.Load<GameObject>("small_explosion")); ResLoad.small_explosion.SetActive(false);
		ResLoad.light_splash=Instantiate(Resources.Load<GameObject>("light_splash")); ResLoad.light_splash.SetActive(false);
		}
		if (mission) {
		ResLoad.laser_splash_sprite=Instantiate(special_splash); ResLoad.laser_splash_sprite.SetActive(false);
		ResLoad.warming_splash_sprite=ResLoad.laser_splash_sprite; ResLoad.warming_splash_sprite.SetActive(false);
		ResLoad.simple_wreck=Instantiate(Resources.Load<GameObject>("wreck1")); ResLoad.simple_wreck.SetActive(false);
		ResLoad.torpedo=Instantiate(Resources.Load<GameObject>("torpedo")); ResLoad.torpedo.SetActive(false);
		ResLoad.nullPref=Instantiate(Resources.Load<GameObject>("nullPref"));
		}

		Global.gui_piece=PlayerPrefs.GetInt("gui_piece");
		Global.mySkin=Resources.Load<GUISkin>("mySkin");
		Global.stSkin=Resources.Load<GUISkin>("stSkin");
		Global.menuSkin=Resources.Load<GUISkin>("menu_skin");
	}

	void Start () {
		GameObject st=GameObject.Find("server_terminal");
		if (!st||!st.activeSelf) offline=true;

		if (use_effects) {
			fires=new GameObject[effects_limit];
			byte i=0;
			GameObject ef=Resources.Load<GameObject>("fire_small"); ef.SetActive(false);
			for (i=0;i<effects_limit;i++) {
				fires[i]=Instantiate(ef) as GameObject;
				fires[i].transform.parent=transform;
			}
			pieces=new GameObject[effects_limit];
			ef=Resources.Load<GameObject>("pieces");ef.SetActive(false);
			for (i=0;i<effects_limit;i++) {
				pieces[i]=Instantiate(ef) as GameObject;
				pieces[i].transform.parent=transform;
			}
		}
		Global.stSkin.GetStyle("Button").fontSize=Global.gui_piece/2;
		Global.stSkin.GetStyle("Label").fontSize=Global.gui_piece/2;
		Global.mySkin.GetStyle("Button").fontSize=Global.gui_piece/2;
		Global.mySkin.GetStyle("Label").fontSize=Global.gui_piece/2;
	}

	public GameObject EffectRequest(Vector3 pos,byte x) {
		GameObject f=null;
		byte i=0;
			bool a=false;
			switch (x) {
			case 1:  //FIRE
				for (i=0;i<effects_limit;i++) {
					if (fires[i]==null) {fires[i]=Instantiate(Resources.Load<GameObject>("fire_small")) as GameObject;f=fires[i];a=true;break;}
					else {	if (!fires[i].activeSelf) {f=fires[i];a=true;break;}}
				}
				if (a) {f.transform.position=pos;f.SetActive(true);}
			break; 
			case 2: //WRECKED PIECES
				for (i=0;i<effects_limit;i++) {
					if (pieces[i]==null) {pieces[i]=Instantiate(Resources.Load<GameObject>("pieces_emitter")) as GameObject;f=pieces[i];a=true;break;}
					else {	if (!pieces[i].activeSelf) {f=pieces[i];a=true;break;}}
				}
				if (a) {f.transform.position=pos;f.SetActive(true);}
			break;
			}

		if (f!=null) {f.transform.position=pos;print (f.name);}
		return (f);
	}

	void OnGUI () {
		if (offline) {
			if (GUI.Button(new Rect(Screen.width-128,0,128,32),"В меню")){Application.LoadLevel(0);Destroy(gameObject);}
		}
	}
}
