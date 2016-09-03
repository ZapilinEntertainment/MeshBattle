using UnityEngine;
using System.Collections;

public class bonus : MonoBehaviour {
	public byte bonus_type;
	//0-reactor,1-maneuver,2-speed,3-weapons,4-radar,5-respawn,6-shield,7-armor,8-nose_armor,9-taran_cover,10-nothing,11-bottom cover,12 - left reactor cover,13-right reactor cover
	public ushort energy_consumption=0;
	public float value=0;
	public byte second_bonus_type=10;
	public float value2=0;
	public int range=0; //for radar bonuses;
	NetworkShipController nsc;
	// Use this for initialization
	void Start () {
		nsc=transform.root.GetComponent<NetworkShipController>();
		if (nsc == null)
			return;
		switch (bonus_type) {
		case 0: nsc.reactor_bonus+=value;break;
		case 1: nsc.aa_bonus+=value;break;
		case 2: nsc.va_bonus+=value;break;
		case 3: nsc.weapons_bonus+=value;break;
		case 4: nsc.radar_bonus+=value;break;
		case 5: if (nsc.respawn_bonus>0.5f) nsc.respawn_bonus*=(1-value);break;
		case 6: nsc.shield_bonus+=value;break;
		case 7:nsc.armor_bonus*=(1-value);break;
		case 8: nsc.nose_armor+=value;break;
		case 9:nsc.taran_cover=true;break;
		case 11: nsc.bottom_armor+=value;break;
		case 12: nsc.leftwing_armor+=value;break;
		case 13: nsc.rightwing_armor+=value;break;
		}
		if (second_bonus_type!=10) {
			switch (second_bonus_type) {
			case 0: nsc.reactor_bonus+=value2;break;
			case 1: nsc.aa_bonus+=value2;break;
			case 2: nsc.va_bonus+=value2;break;
			case 3: nsc.weapons_bonus+=value2;break;
			case 4: nsc.radar_bonus+=value2;break;
			case 5: if (nsc.respawn_bonus>0.5f)  nsc.respawn_bonus*=(1-value2);break;
			case 6: nsc.shield_bonus+=value2;break;
			case 7:nsc.armor_bonus*=(1-value2);break;
			case 8: nsc.nose_armor+=value2;break;
			case 9:nsc.taran_cover=true;break;
			case 11: nsc.bottom_armor+=value;break;
			case 12: nsc.leftwing_armor+=value;break;
			case 13: nsc.rightwing_armor+=value;break;
			}}
		nsc.constant_supply+=energy_consumption;
	}
	
	void OnDestroy() {
		if (nsc == null)
			return;
		switch (bonus_type) {
		case 0: nsc.reactor_bonus-=value;break;
		case 1: nsc.aa_bonus-=value;break;
		case 2: nsc.va_bonus-=value;break;
		case 3: nsc.weapons_bonus-=value;break;
		case 4: nsc.radar_bonus-=value;break;
		case 5: nsc.respawn_bonus/=(1-value);break;
		case 6: nsc.shield_bonus-=value;break;
		case 7:nsc.armor_bonus/=(1-value);break;
		case 8: nsc.nose_armor-=value;break;
		case 9:nsc.taran_cover=false;break;
		case 11: nsc.bottom_armor-=value;break;
		case 12: nsc.leftwing_armor-=value;break;
		case 13: nsc.rightwing_armor-=value;break;
		}
		if (second_bonus_type!=10) {
			switch (second_bonus_type) {
			case 0: nsc.reactor_bonus-=value2;break;
			case 1: nsc.aa_bonus-=value2;break;
			case 2: nsc.va_bonus-=value2;break;
			case 3: nsc.weapons_bonus-=value2;break;
			case 4: nsc.radar_bonus-=value2;break;
			case 5: nsc.respawn_bonus/=(1-value2);break;
			case 6: nsc.shield_bonus-=value2;break;
			case 7:nsc.armor_bonus/=(1-value2);break;
			case 8: nsc.nose_armor-=value2;break;
			case 9:nsc.taran_cover=false;break;
			case 11: nsc.bottom_armor-=value;break;
			case 12: nsc.leftwing_armor-=value;break;
			case 13: nsc.rightwing_armor-=value;break;
			}
		}
		nsc.constant_supply-=energy_consumption;
	}
}
