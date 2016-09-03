using UnityEngine;
using System.Collections;

public class lgc_indicator : MonoBehaviour {
	public longcaster myGun;
	public Rect myRect;
	public NetworkShipGUI nsg;

	void OnGUI () {
		if (myGun.firing) {GUI.DrawTexture(myRect,nsg.ind_white,ScaleMode.StretchToFill);}
		else {
			if (myGun.ready) {if (nsg.nsc.capacity>=myGun.eps) GUI.DrawTexture(myRect,nsg.ind_green); else GUI.DrawTexture(myRect,nsg.ind_red);}
			else {GUI.DrawTexture(myRect,nsg.ind_yellow,ScaleMode.StretchToFill);}
		}
		GUI.DrawTexture(myRect,nsg.module_icons[0],ScaleMode.StretchToFill);
	}	
}
