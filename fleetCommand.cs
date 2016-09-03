using UnityEngine;
using System.Collections;

public class fleetCommand : MonoBehaviour {
	public GameObject[] ships;
	public GameObject[] enemies;
	public char command='0';

	// Use this for initialization
	void Start () {
		gameObject.name="fc"+command;
		StartCoroutine(Scan());
	}

	IEnumerator Scan() {
		GameObject[] allships=GameObject.FindGameObjectsWithTag("Player");
		int ships_count=0; int enemy_count=0;
		foreach (GameObject s in allships) {
			if (s.name[0]!=command) enemy_count++; else ships_count++;
		}
		ships=new GameObject[ships_count];ships_count=0;
		enemies=new GameObject[enemy_count];enemy_count=0;
		foreach (GameObject s in allships) {
			if (s.name[0]!=command) {enemies[enemy_count]=s;enemy_count++;}
			else {ships[ships_count]=s;ships_count++;}
		}
		yield return new WaitForSeconds(2);
		StartCoroutine(Scan());}

	public void ScanOnce () {
		GameObject[] allships=GameObject.FindGameObjectsWithTag("Player");
		int ships_count=0; int enemy_count=0;
		foreach (GameObject s in allships) {
			if (s.name[0]!=command) enemy_count++; else ships_count++;
		}
		ships=new GameObject[ships_count];ships_count=0;
		enemies=new GameObject[enemy_count];enemy_count=0;
		foreach (GameObject s in allships) {
			if (s.name[0]!=command) {enemies[enemy_count]=s;enemy_count++;}
			else {ships[ships_count]=s;ships_count++;}
		}
	}

	public GameObject GetNearestEnemy(shootingSector s) {
		float minr=s.range;
		float mina=s.angle;
		GameObject nEnemy=null;
		if (enemies.Length>0) {
		foreach (GameObject ship in enemies) {
			float d=Vector3.Distance(ship.transform.position,s.position);
			if (d<=minr) {
				float an=Vector3.Angle(ship.transform.position-s.position,s.current_direction);
				if (an<=mina) {
					mina=an;
					minr=d;
					nEnemy=ship;
				}
			}
		}
		}
		return(nEnemy);
	} 

	public GameObject GetNearestEnemy (Vector4 info) {
		float minr=info.w;
		Vector3 pos=new Vector3(info.x,info.y,info.z);
		GameObject nEnemy=null;
		if (enemies.Length>0) {
		foreach (GameObject ship in enemies) {
				if (ship==null) continue;
			float d=Vector3.Distance(pos,ship.transform.position);
			if (d<=minr) {minr=d;nEnemy=ship;}
			}}
		return(nEnemy);
	}

}

public class shootingSector:MonoBehaviour {
	public Vector3 position;
	public Vector3 current_direction;
	public float range;
	public float angle;

	public shootingSector(Vector3 pos,Vector3 dir,float r, float a) {
		position=pos;
		current_direction=dir;
		range=r;
		angle=a;
	}
}
