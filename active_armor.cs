using UnityEngine;
using System.Collections;

public class active_armor : MonoBehaviour {

	public bool active=false;
	public bool nose=false;
	public int consumption=300;
	public float bonus_value=0.25f;
	public Renderer rr;
	public Texture firelight_tx;
	Texture normal_light_tx;
	NetworkShipController nsc;
	// Use this for initialization
	void Start () {
		if (rr) {normal_light_tx=rr.material.GetTexture("_EmissionMap");}
		nsc=transform.root.GetComponent<NetworkShipController>();
		if (nsc) {nsc.armor_consumption+=consumption;}
	}

	public void Activate() {
		active=true;
		if (nose) {nsc.nose_armor+=bonus_value;}
		else {nsc.armor_bonus*=bonus_value;}
		nsc.constant_supply+=consumption;
		if (rr) {rr.material.SetTexture("_EmissionMap",firelight_tx);}
	}

	public void Deactivate() {
		if (!active) return;
		active=false;
		if (nose) {nsc.nose_armor-=bonus_value;}
		else {nsc.armor_bonus/=bonus_value;}
		nsc.constant_supply-=consumption;
		if (rr) {rr.material.SetTexture("_EmissionMap",normal_light_tx);}
	}

}
