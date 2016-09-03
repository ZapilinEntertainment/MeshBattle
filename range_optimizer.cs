using UnityEngine;
using System.Collections;

public class range_optimizer : MonoBehaviour {

	public byte size=1;
	public GameObject near_zone_mesh;//zone 0
	public GameObject range_zone_mesh;//zone 1
	public GameObject far_zone_mesh;//zone 2
	public GameObject current_mesh;
	public Vector3 correction_vector;
	int dist1;
	int dist2;
	byte zone=0;
	byte prevzone=0;
	byte prevzone2=0;
	
	int l;
	
	void Start () {
		dist1=Global.drawDist1*size;
		dist2=Global.drawDist2*size;
		SetMeshes();
	}
	
	void Update () {
		if (Global.cam==null||Global.cam.transform.InverseTransformPoint(transform.position).z<-30*size) return;
		l=(int)Vector3.Distance(transform.position,Global.cam.transform.position);
		prevzone=zone;
		if (l>=dist1) {if (l>=dist2) {zone=2;} else {zone=1;}} else {zone=0;}
		if (prevzone!=zone) SetMeshes(); 
	}
	
	
	
	void SetMeshes () {
		Destroy(current_mesh);
		switch (zone) {
		case 0:
			current_mesh=Instantiate(near_zone_mesh,transform.position+transform.TransformDirection(correction_vector),transform.rotation) as GameObject;
			break;
		case 1:
			current_mesh=Instantiate(range_zone_mesh,transform.position+transform.TransformDirection(correction_vector),transform.rotation) as GameObject;
			break;
		case 2:
			current_mesh=Instantiate(far_zone_mesh,transform.position+transform.TransformDirection(correction_vector),transform.rotation) as GameObject;
			break;
		}
		current_mesh.transform.parent=transform;
	}
}
