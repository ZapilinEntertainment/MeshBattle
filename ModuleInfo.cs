using UnityEngine;
using System.Collections;

public class ModuleInfo : MonoBehaviour {
	public byte type=1;//0-палубный, 1- бортовой,2-нос, 3-киль, 4 -крыло
	public byte count=1;
	public int health_plus=0;
	public Vector3 correction_vector;
	public string id;
	public bool uses_collider=false;
	public byte collider_number=0; //0-отдельный коллайдер, 1 - корпус (c1), 2 - реактор (c2), 3- сопло (с3)
	public Vector3 size;
	public Vector3 collider_correction=Vector3.zero;
	BoxCollider c;
	public string name;
	public bool symmetrical=true;
	public bool right=false;
	public Renderer myRenderer;




	void DestroyModule () {
		if (uses_collider&&c) {
			if (collider_number==0) {
				Destroy(c);
				c=null;
				switch (type) {
				case 3: transform.root.gameObject.GetComponent<NetworkShipController>().c4=null;break;
				case 4: if (transform.localPosition.x>0) transform.root.gameObject.GetComponent<NetworkShipController>().c5=null; else transform.root.gameObject.GetComponent<NetworkShipController>().c6=null;break;
				}}
			else {
				c.size-=size;
				c.center-=collider_correction;
				c=null;
			}
		}
		Destroy (gameObject);

	}

	public void SetLayer (int layer) {
		if (myRenderer)		myRenderer.gameObject.layer=layer;
	}
}
