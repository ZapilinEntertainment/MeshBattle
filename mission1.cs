using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class mission1 : MonoBehaviour {
	public bool passaged=false;
	public GameObject ship;
	SceneResLoader srl;
	public GameObject second_gates;
	public GameObject sphere;
	public GameObject first_space_sprites;
	public GameObject second_space_sprites;
	public Material stratophor_skybox;
	public GameObject directional_light;
	public Texture red_circle;
	Vector3 gate_point1=new Vector3(0,0,1900);
	Vector3 gate_point2=new Vector3(0,0,2100);
   Vector3 endmark=new Vector3(0,0,3000);
	Vector3 endmark2=new Vector3(4000,0,2500);
	public GameObject[] ships;// 15 cruisers+7carriers+8base_modules
	GameObject tracking_ship;
	byte i=0;
	bool can_send=true;
	public Texture text_back;
	public Texture accept_button;
	bool game_started=false;
	byte step=0;
	Rect r;
	Rect r2;
	int k=15;
	string text;
	int sw;
	int sh;
	bool gates_marking=false;
	bool win=false;
	bool fail=false;
	bool waited=false;

	void Start () {
		srl=GameObject.Find("menu").GetComponent<SceneResLoader>();
		text="Добро пожаловать в игру MeshBattle. Теперь вы - Командующий космического крейсера, и глава экспедиции в Последний сектор. Ваша задача - исследовать сектор, и , по возможности, вернуться оттуда живым." ;			
		sw=Screen.width;
		sh=Screen.height;
	}

	IEnumerator Awaiting() {
		yield return new WaitForSeconds(5);
		if (!ship) ship=srl.localShip;
		if (!ship) fail=true;
		waited=true;
	}

	public void StartGame () {
		ship=srl.localShip;
		game_started=true;
		if (sh*(13/9)>sw) {
			k=sw/32;//widescreen 16:9
		}
		else {k=sw/24;}
		r=new Rect (sw/2-4*k,sh/2-2*k,8*k,4*k);
		r2=new Rect (sw/2-1.5f*k,sh/2+2*k,3*k,k);
		GUI.skin=Global.menuSkin;
		Global.mySkin.GetStyle("Label").fontSize=k/2;
		Global.mySkin.GetStyle("Button").fontSize=k/2;
		StartCoroutine(Awaiting());
	}

	// Update is called once per frame
	void Update () {
		if (!game_started||fail) return;
		if (!ship) {if (!waited) ship=srl.localShip; else {fail=true;step=10;text="Вы погибли слишком рано!";}return;}
		byte a =0;
		foreach (GameObject s in ships) {
			if (s==null) a++;
		}
		if (a>5) {fail=true;step=10;text="Неприемлимые потери!";return;}
		if (!passaged) {
			if (ship.layer==9) {
				passaged=true;
				Global.cam.GetComponent<Camera>().cullingMask=1<<9;
				second_gates.SetActive(true);
				first_space_sprites.SetActive(false);
				second_space_sprites.SetActive(true);
				srl.environmental_camera.GetComponent<Skybox>().material=stratophor_skybox;
				directional_light.transform.Rotate(new Vector3(0,180,0));
				Destroy(sphere.GetComponent<marker>());
			}
			if (step==6) {
				if (sphere.GetComponent<marker>()==null) gates_marking=true; else gates_marking=false;
				if (sphere.GetComponent<gate_mechanism>().gates_open) {step++;r.x+=7*k;r.height-=3*k;text="Сейчас флот начнет переход. Будьте аккуратны и старайтесь никого не задеть. Миссия закончится как только вы окажетесь на той стороне.";}
			}
		}
		if (tracking_ship&&can_send) {
			if (Vector3.Angle(tracking_ship.transform.forward,gate_point1-tracking_ship.transform.position)<5||Vector3.Distance(tracking_ship.transform.position,gate_point1)<100&&Vector3.Distance(tracking_ship.transform.position,ships[i+1].transform.position)>500) {tracking_ship=null;SendShip();return;}
			if (tracking_ship.layer==9) {
				tracking_ship=null;
				SendShip();
			}
		}
	}

	IEnumerator WarpJump () {
		yield return new WaitForSeconds(2);

	}

	IEnumerator sendShipCD () {
		can_send=false;
		yield return new WaitForSeconds (3);
		can_send=true;
		SendShip();
	}

	IEnumerator WaitForPassing () {
		yield return new WaitForSeconds(2);
		if (ship.layer==9) {
				win=true;
				if (PlayerPrefs.HasKey("campaign_mission")) {
					if (PlayerPrefs.GetInt("campaign_mission")<2) {PlayerPrefs.SetInt("campaign_mission",2);Global.campaign_mission=2;}}
				else {PlayerPrefs.SetInt("campaign_mission",2);Global.campaign_mission=2;} 
			}
		else StartCoroutine(WaitForPassing());
	}

	public void SendShip() {
		if (!can_send) {StartCoroutine(sendShipCD());return;}
		if (ships[i]!=null) {
		if (i!=32) tracking_ship=ships[i];
		} else {i++;SendShip();return;}
		Vector3 w=gate_point1;
			switch (i) {
		//guarding group
		case 0: w=endmark;break;
		case 1: w=endmark+new Vector3(-100,0,-200);break;
		case 2: w=endmark+new Vector3(-100,0,100);break;
		case 3: w=endmark+new Vector3(100,0,100);break;
		case 4: w=endmark+new Vector3(-200,0,0);break;
		case 5: w=endmark+new Vector3(200,0,0);break;
		case 6: w=endmark+new Vector3(100,0,-200);break;
       //convoy cruisers
		case 7: w=endmark2+new Vector3(100,0,100);break;
		case 8: w=endmark2+new Vector3(200,0,200);break;
		case 9: w=endmark2+new Vector3(300,0,300);break;
		case 10: w=endmark2+new Vector3(100,0,-100);break;
		case 11: w=endmark2+new Vector3(200,0,-200);break;
		case 12: w=endmark2+new Vector3(300,0,-300);break;
		case 13: w=endmark2+new Vector3(600,0,250);break;
		case 14: w=endmark2+new Vector3(600,0,-250);break;
			//station
		case 15: w=endmark+new Vector3(0,0,60.355f);break;
		case 16: w=endmark+new Vector3(-42.677f,0,42.677f);break;
		case 17: w=endmark+new Vector3(42.677f,0,42.677f);break;
		case 18: w=endmark+new Vector3(-60.355f,0,0);break;
		case 19: w=endmark+new Vector3(60.355f,0,0);break;
		case 20: w=endmark+new Vector3(-42.677f,0,-42.677f);break;
		case 21: w=endmark+new Vector3(42.677f,0,-42.677f);break;
		case 22: w=endmark+new Vector3(0,0,60.355f);break; //добавить скрипт на сборку станции
			// carriers
		case 23: w=endmark2+new Vector3(300,0,150);break;
		case 24: w=endmark2+new Vector3(300,0,-150);break;
		case 25: w=endmark2+new Vector3(400,0,250);break;
		case 26: w=endmark2+new Vector3(400,0,-250);break;
		case 27: w=endmark2+new Vector3(500,0,-200);break;
		case 28: w=endmark2+new Vector3(500,0,-200);break;
		case 29: w=endmark2+new Vector3(400,0,100);break;
		case 30: w=endmark2+new Vector3(400,0,-100);break;
			}

		tracking_ship.SendMessage("MoveTo",gate_point1,SendMessageOptions.DontRequireReceiver);
		tracking_ship.SendMessage("MoveAdd",gate_point2,SendMessageOptions.DontRequireReceiver);
		tracking_ship.SendMessage("MoveAdd",w,SendMessageOptions.DontRequireReceiver);
		i++;
	}

	void OnGUI () {
		if (game_started) {
			switch (step) {
			case 0: 
				GUI.DrawTexture(r,text_back,ScaleMode.StretchToFill);
				GUI.Label(r,text);
				if (GUI.Button(r2,"далее")) {step++;text="Управлять крупным космическим кораблем достаточно просто. Первое, что нужно освоить - управление внешней камерой." +
						"Зажмите колесико мыши и двигайте саму мышь, чтобы вращать камеру, прокрутка колесиком приближает или отдаляет камеру.";}
				break;
			case 1: 
				GUI.DrawTexture(r,text_back,ScaleMode.StretchToFill);
				GUI.Label(r,text);
				if (GUI.Button(r2,"далее")) {step++;text="Вот эти кнопки отвечают за вращение корабля в плоскости эклиптики. Им соответсвуют стрелки вправо-влево.";}

				break;
			case 2: 
				GUI.DrawTexture(r,text_back,ScaleMode.StretchToFill);
				GUI.Label(r,text);
				GUI.DrawTexture(new Rect(sw/2-6*k,sh-k,k,k),red_circle,ScaleMode.StretchToFill);
				GUI.DrawTexture(new Rect(sw/2+3*k,sh-k,k,k),red_circle,ScaleMode.StretchToFill);
				if (GUI.Button(r2,"далее")) {step++;text="Это коробка передач. Сверху вниз: полный ход, полхода, стоп-машина, полхода назад.";}
				break;
				case 3:
				GUI.DrawTexture(r,text_back,ScaleMode.StretchToFill);
				GUI.Label(r,text);
				GUI.DrawTexture(new Rect(sw/2-3*k,sh-2*k,1.5f*k,2*k),red_circle,ScaleMode.StretchToFill);
				if (GUI.Button(r2,"далее")) {step++;text="Это режимы работы двигателя. Режим маневра повышает скорость вращения и немного увеличивает тягу двигателя, Форсированный режим сильно увеличивает тягу двигателя и снижает маневренность." +
						"Каждый из режимов имеет разное энергопотребление.";}
				break;
				case 4:
				GUI.DrawTexture(r,text_back,ScaleMode.StretchToFill);
				GUI.Label(r,text);
				GUI.DrawTexture(new Rect(sw/2+1.5f*k,sh-2*k,1.5f*k,1.5f*k),red_circle,ScaleMode.StretchToFill);
				if (GUI.Button(r2,"далее")) {step++;text="Количество доступной энергии. Расходуется на активацию способностей и орудийные импульсы. Текущее значение зависит от энергопотребления и режима реактора. " +
					"(Вы можете включить режим перегрузки, нажав на специальную кнопку)";}
				break;
				case 5:
				GUI.DrawTexture(r,text_back,ScaleMode.StretchToFill);
				GUI.Label(r,text);
				GUI.DrawTexture(new Rect(0,sh-k,sw/2-7*k,k/2),red_circle,ScaleMode.StretchToFill);
				GUI.DrawTexture(new Rect(sw/2-7*k,sh-k,k,k),red_circle,ScaleMode.StretchToFill);
				if (GUI.Button(r2,"далее")) {step++;text="Для наведения орудий щелкните вражеский корабль или объект левой кнопкой мыши. Используйте ctrl-ы для стрельбы из выносных орудий, alt-ы для стрельбы с бортовых орудий, и пробел для выстрела из орудий основного калибра. " +
					"Прицельтесь в светящийся шар над Вратами. Постарайтесь, чтобы ваш корабль был направлен на Врата - углы стрельбы ваших тяжелых лазеров ограничены.";
					r.x-=7*k;r.height+=3*k;r2.x=sw/2-1.5f*k;
				}
				break;
				case 6:
				GUI.DrawTexture(r,text_back,ScaleMode.StretchToFill);
				GUI.Label(r,text);
				if (gates_marking) {
					Vector2 gp=Global.cam.GetComponent<Camera>().WorldToScreenPoint(sphere.transform.position);
				    gp.y=sh-gp.y;
				    GUI.DrawTexture(new Rect(gp.x-k/2,gp.y-k/2,k,k),red_circle,ScaleMode.StretchToFill);
				}
				break;
				case 7:
				GUI.DrawTexture(r,text_back,ScaleMode.StretchToFill);
				GUI.Label(r,text);
				if (GUI.Button(r2,"далее")) {step++;StartCoroutine(WaitForPassing());}
				break;
				case 8:
				if (win) {
					GUI.DrawTexture(r,text_back,ScaleMode.StretchToFill);
					GUI.Label(r,"Задание выполнено! Продолжить вторжение?");
					if (GUI.Button(new Rect(r.x,r2.y,4*k,2*k),"Да (вести флот дальше)")) {
						NetworkManager nm=GameObject.Find("networkManager").GetComponent<NetworkManager>();
						nm.onlineScene="mission2";
						nm.StopHost();
						nm.StartHost();
						Destroy(gameObject);
						}
					if (GUI.Button(new Rect(sw/2,r2.y,4*k,2*k),"Нет (выйти в меню)")) {
						Application.LoadLevel(0);
					}
				}
				break;
				case 10:
				Rect rx=new Rect(Screen.width/2-128,Screen.height/2-32,256,64);
				GUI.Label(rx,"Поражение! "+text);rx.width/=2;
				if (GUI.Button(rx,"Перезапуск")) GameObject.Find("server_terminal").GetComponent<ServerTerminal>().Restart();
				rx.x+=128;
				if (GUI.Button(rx,"Выход")) GameObject.Find("server_terminal").GetComponent<ServerTerminal>().Exit();
				break;
			}
		}
	}

	public void MissionFailed () {
		fail=true;
		text="Вы сбежали с поля боя";
		step=10;
	}
}
