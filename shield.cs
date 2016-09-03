using UnityEngine;
using System.Collections;

public class shield : MonoBehaviour {
	public int hp=0;
	public uint maxhp=10000;
	public uint consumption=600;
	public int restoration=200;
	public bool powered=false;
	float bonus=1;
	public NetworkShipController nsc;



	public void ApplyDamage (Vector4 d) {
		uint damage=(uint)d.w;       
		if (damage<hp) {hp-=(int)damage;} 
		if (damage>=hp) {hp=0;nsc.CmdShield(false);}
		
	}
	
	void Update () {
		if (!powered) {if (hp>0) {hp-=(int)(restoration*Time.deltaTime*bonus);} if (hp<=0) {hp=0;nsc.shields_on=false;gameObject.SetActive(false);}}
		else {
			if (hp<maxhp*bonus) hp+=(int)(restoration*bonus*Time.deltaTime);
		}
		if (hp>maxhp*bonus) hp=(int)(maxhp*bonus);
	}

	public void Power (bool x) {
		if (x) {powered=true;
			bonus=nsc.shield_bonus;
			nsc.constant_supply+=(int)(consumption*bonus);}
		else {powered=false;nsc.constant_supply-=(int)(consumption*bonus);}
	}
}
