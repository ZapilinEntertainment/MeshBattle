using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkModuleSpawner : NetworkBehaviour
{
	public GameObject module_prefab;
	public Vector3 zero_point = new Vector3 (1.4f, 3f, 13);
	Vector3 correction_vector;
	bool haveModules = false;
	public string modulesString;
	int cid;
	bool hide_after_spawn=true;
	// Use this for initialization
	public void Start ()
	{if (isLocalPlayer) {modulesString=PlayerPrefs.GetString("shipModules");SpawnModules(modulesString);CmdSetModuleString(modulesString);}
	}


	public void SpawnModulesForAll ()
	{
		if (modulesString.Length!=0) {
		RpcSpawnModules(modulesString);
		}
	}
	[Command]
	public void CmdSetModuleString (string s) {
		modulesString=s;
		SpawnModules(s);
	}

	[ClientRpc]
	public void  RpcSpawnModules (string s)
	{
		if (!haveModules) {
			SpawnModules (s);
		}
	}


	public void SpawnModules (string s)
	{
		if (haveModules) return;
		NetworkShipController nsc=GetComponent<NetworkShipController>();
		if (nsc.srl.mission&&nsc.srl.special_composition!="") {s=nsc.srl.special_composition;}
		for (var i = 0; i < 28; i++) {
			if (s.Substring (2 * i, 2) != "00") {
				module_prefab = Resources.Load (s.Substring (2 * i, 2)) as GameObject;	
				ModuleInfo mi = module_prefab.GetComponent<ModuleInfo> ();
				correction_vector = mi.correction_vector;
				if (i==27) {correction_vector.x*=-1;}	
				switch (i) {
				case 0:
					zero_point = new Vector3 (0, 0, 14);
					break;
				//left board-first
				case 1:
					zero_point = new Vector3 (-0.6f, -4, 7);
					break;
				case 2:
					zero_point = new Vector3 (-1.4f, -3, 7);
					break;
				case 3:
					zero_point = new Vector3 (-2, -1.5f, 7);
					break;
				case 4:
					zero_point = new Vector3 (-2, 1.5f, 7);
					break;
				case 5:
					zero_point = new Vector3 (-1.4f, 3, 7);
					break;
				case 6:
					zero_point = new Vector3 (-0.6f, 4, 7);
					break;
				//right board - first
				case 7:
					zero_point = new Vector3 (0.6f, 4, 7);
					break;
				case 8:
					zero_point = new Vector3 (1.4f, 3, 7);
					break;
				case 9:
					zero_point = new Vector3 (2, 1.5f, 7);
					break;
				case 10:
					zero_point = new Vector3 (2, -1.5f, 7);
					break;	
				case 11:
					zero_point = new Vector3 (1.4f, -3, 7);
					break;
				case 12:
					zero_point = new Vector3 (0.6f, -4, 7);
					break;
				//left board-second
				case 13:
					zero_point = new Vector3 (-0.7f, -4, -7);
					break;
				case 14:
					zero_point = new Vector3 (-1.9f, -3, -7);
					break;
				case 15:
					zero_point = new Vector3 (-3, -1.5f, -7);
					break;
				case 16:
					zero_point = new Vector3 (-3, 1.5f, -7);
					break;
				case 17:
					zero_point = new Vector3 (-1.9f, 3, -7);
					break;
				case 18:
					zero_point = new Vector3 (-0.7f, 4, -7);
					break;
				//right board - second
				case 19:
					zero_point = new Vector3 (0.7f, 4, -7);
					break;
				case 20:
					zero_point = new Vector3 (1.9f, 3, -7);
					break;
				case 21:
					zero_point = new Vector3 (3, 1.5f, -7);
					break;
				case 22:
					zero_point = new Vector3 (3, -1.5f, -7);
					break;	
				case 23:
					zero_point = new Vector3 (1.9f, -3, -7);
					break;
				case 24:
					zero_point = new Vector3 (0.7f, -4, -7);
					break;

				case 25:
					zero_point = new Vector3 (-4, -0.5f, -21);
					break;
				case 26:
					zero_point = new Vector3 (0, -5.5f, -20);
					break;
				case 27:
					zero_point = new Vector3 (4, -0.5f, -21);
					break;
				}
					GameObject x = Instantiate (module_prefab, transform.root.position, Quaternion.identity) as GameObject;
					x.transform.parent = transform.root;
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

					x.transform.localPosition = zero_point+ correction_vector;
					if (hide_after_spawn) {x.SetActive(false);}
					nsc.modules[i]=x;
				nsc.maxhp+=mi.health_plus;
				if (mi.uses_collider) {
					BoxCollider c=null;
					if (mi.collider_number==0) {
						c=transform.root.gameObject.AddComponent<BoxCollider>();
						Vector3 collider_correction=mi.collider_correction;
						c.size=mi.size;
						if (mi.type==4&&zero_point.x>0) collider_correction.x*=-1;
						c.center=zero_point+collider_correction;
						switch (mi.type) {
						case 3:nsc.c4=c;break;
						case 4: if (transform.localPosition.x>0) nsc.c5=c; else nsc.c6=c;break;
						}
					}
					else {
						switch (mi.collider_number) {
						case 1:c=nsc.c1;break;
						case 2:c=nsc.c2;break;
						case 3:c=nsc.c3;break;
						}
						if (c) {
							c.size+=mi.size;
							c.center+=mi.collider_correction;
						}
					}
				}
			}
		}
		if (hide_after_spawn) hide_after_spawn=false;
		haveModules=true;
	}
}
