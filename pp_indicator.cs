using UnityEngine;
using System.Collections;

public class pp_indicator : MonoBehaviour {

	public pointpulser myGun;
	public Rect myRect;
	public NetworkShipGUI nsg;

	void OnGUI () {
			if (myGun.ready) {if (nsg.nsc.capacity>=myGun.energy) GUI.DrawTexture(myRect,nsg.ind_green); else GUI.DrawTexture(myRect,nsg.ind_red);}
			else {GUI.DrawTexture(myRect,nsg.ind_yellow,ScaleMode.StretchToFill);}

		GUI.DrawTexture(myRect,nsg.module_icons[3],ScaleMode.StretchToFill);
	}}
