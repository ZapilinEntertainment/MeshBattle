using UnityEngine;
using System.Collections;

public class pr_indicator : MonoBehaviour {

	public pulser myGun;
	public Rect myRect;
	public NetworkShipGUI nsg;

	void OnGUI () {

			if (myGun.ready) {if (nsg.nsc.capacity>=myGun.energy) GUI.DrawTexture(myRect,nsg.ind_green); else GUI.DrawTexture(myRect,nsg.ind_red);}
			else {GUI.DrawTexture(myRect,nsg.ind_yellow,ScaleMode.StretchToFill);}
		GUI.DrawTexture(myRect,nsg.module_icons[1],ScaleMode.StretchToFill);
	}	
}
