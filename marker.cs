using UnityEngine;
using System.Collections;

public class marker : MonoBehaviour {
	public Texture marking_texture;
	public int radarDistance=10000;
	// Use this for initialization
	void Start () {
		if (!marking_texture) {marking_texture=Resources.Load<Texture>("weapons_icon");}

	}
	


	void OnGUI () {
		if (Global.cam.transform.InverseTransformPoint(transform.position).z>0) {
			Vector2 tc=Global.cam.GetComponent<Camera>().WorldToScreenPoint(transform.position);
			float d=radarDistance-Vector3.Distance(transform.position,Global.cam.transform.position)-500;
			float dole=0;
			if (d>0) {dole=64.0f*d/radarDistance;if (dole<2) dole=2;}
			GUI.DrawTexture(new Rect(tc.x-dole/2,Screen.height-tc.y-dole/2,dole,dole),marking_texture,ScaleMode.StretchToFill);
		}
	}

	public void RemoveSelection() {
		Destroy(this);
	}
}
