using UnityEngine;
using System.Collections;

public class lc_indicator : MonoBehaviour {
	public laser_caster myGun;
	public Rect myRect;
	public NetworkShipGUI nsg;

	void OnGUI () {
		if (myGun.firing) {GUI.DrawTexture(myRect,nsg.ind_white,ScaleMode.StretchToFill);}
		else {
			if (myGun.ready) {if (nsg.nsc.capacity>=myGun.eps) GUI.DrawTexture(myRect,nsg.ind_green); else GUI.DrawTexture(myRect,nsg.ind_red);}
			else {GUI.DrawTexture(myRect,nsg.ind_yellow,ScaleMode.StretchToFill);}
		}
		GUI.DrawTexture(myRect,nsg.module_icons[2],ScaleMode.StretchToFill);
	}	
}
