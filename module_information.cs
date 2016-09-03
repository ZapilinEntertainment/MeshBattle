using UnityEngine;
using System.Collections;

public class module_information : MonoBehaviour {
	public string infostring="";
	public string ps=""; //properties/ 8x32

	// Use this for initialization
	void Start () {
		int a=0;
		if (GetComponent<laser_caster>()) {
			laser_caster lc=GetComponent<laser_caster>();
			infostring="лучевой точечный излучатель";
			ps=new string(' ',224);
			ps=ps.Insert(a,"орудий: "+lc.guns.Length);a+=32;
			ps=ps.Insert(a,"дальность: "+lc.range);a+=32;
			ps=ps.Insert(a,"урон/сек: "+lc.dps);a+=32;
			ps=ps.Insert(a,"длится "+lc.time+" сек");a+=32;
			ps=ps.Insert(a,"охлаждение: "+lc.cooldown+"секунд");a+=32;
			ps=ps.Insert(a,lc.eps+" энергии/сек");a+=32;
			ps=ps.Insert (a,"отклонение: до "+lc.max_angle+" градусов");a+=32;
		}
		else {
			if (GetComponent<longcaster>()) {
			longcaster lg=GetComponent<longcaster>();
			infostring="лучевой параллельный излучатель";
			ps=new string(' ',192);
				ps=ps.Insert(a,"орудий: "+lg.guns.Length);a+=32;
				ps=ps.Insert(a,"дальность: "+lg.range);a+=32;
				ps=ps.Insert(a,"урон/сек: "+lg.dps);a+=32;
				ps=ps.Insert(a,"длится "+lg.time+" сек");a+=32;
				ps=ps.Insert(a,"охлаждение: "+lg.cooldown)+"секунд";a+=32;
				ps=ps.Insert(a,lg.eps+" энергии/сек");a+=32;
			}
			else {
				if (GetComponent<pointpulser>()) {
					pointpulser pp=GetComponent<pointpulser>();
					infostring="импульсный точечный излучатель";
					ps=new string(' ',192);
					ps=ps.Insert(a,"орудий: "+pp.guns.Length);a+=32;
					ps=ps.Insert(a,"дальность: "+pp.range);a+=32;
					ps=ps.Insert(a,"урон: "+pp.damage);a+=32;
					ps=ps.Insert(a,"охлаждение: "+pp.cooldown+"секунд");a+=32;
					ps=ps.Insert(a,"затраты: "+pp.energy);a+=32;
					ps=ps.Insert (a,"отклонение: до "+pp.max_angle+" градусов");a+=32;

				}
				else {
					if (GetComponent<pulser>()) {
						pulser p=GetComponent<pulser>();
						infostring="импульсный параллельный излучатель";
						ps=new string(' ',160);
						ps=ps.Insert(a,"орудий: "+p.guns.Length);a+=32;
						ps=ps.Insert(a,"дальность: "+p.range);a+=32;
						ps=ps.Insert(a,"урон: "+p.damage);a+=32;
						ps=ps.Insert(a,"охлаждение: "+p.cooldown+"секунд");a+=32;
						ps=ps.Insert(a,"затраты: "+p.energy);a+=32;
					}
					else {
						if (GetComponent<tapp>()) {
							tapp t=GetComponent<tapp>();
							infostring="торпедный аппарат";
							ps=new string(' ',160);
							ps=ps.Insert(a,"шахт: "+t.guns.Length);a+=32;
							ps=ps.Insert(a,"дальность: "+t.range);a+=32;
							ps=ps.Insert(a,"урон от торпеды:"+t.damage);a+=32;
							ps=ps.Insert (a,"перезарядка: "+t.cooldown+" секунд");a+=32;
							ps=ps.Insert(a,"затраты энергии: "+t.energy);a+=32;
						}
						else {
							if (GetComponent<active_armor>()) {
								active_armor aa=GetComponent<active_armor>();
								infostring="активная броня";
								ps=new string(' ',64);
								ps=ps.Insert(a,"бонус броне: "+aa.bonus_value);a+=32;
								ps=ps.Insert(a,"энергопотребление:"+aa.consumption);a+=32;
							}
						}
					}
				}
			}
			if (GetComponent<bonus>()) {
				bonus b=GetComponent<bonus>();
				if (ps.Length==0) {if (b.second_bonus_type!=10) {ps=new string(' ',96);} else {ps=new string(' ',64);}}
				if (infostring=="") {
					infostring=GetComponent<ModuleInfo>().name;a=0;
				}
				else {
					if (b.bonus_type==7||b.bonus_type==8||b.bonus_type==9||b.bonus_type==11||b.bonus_type==12||b.bonus_type==13) {infostring="бронированный "+infostring;}
				}
				if (a!=256) {
				switch (b.bonus_type) {
					case 0: ps=ps.Insert(a,"выход реактора:  +"+ b.value*100+"%");break;
					case 1: ps=ps.Insert(a,"маневренность:  +"+ b.value*100+"%");break;
					case 2: ps=ps.Insert(a,"скорость:  +"+ b.value*100+"%");break;
					case 3: ps=ps.Insert(a,"мощность орудий:  +"+ b.value*100+"%");break;
					case 4: ps=ps.Insert(a,"дальность радара:  +"+ b.value*100+"%");break;
					case 5: ps=ps.Insert(a,"время возрождения:  -"+ b.value*100+"%");break;
					case 6: ps=ps.Insert(a,"мощность щитов:  +"+ b.value*100+"%");break;
					case 7: ps=ps.Insert(a,"бронирование:  *"+ b.value*100+"%");break;
					case 8: ps=ps.Insert(a,"носовая броня:  +"+ b.value*100+"%");break;
					case 9: ps=ps.Insert(a,"таран:  +"+ b.value*100+"%");break;
					case 11: ps=ps.Insert(a,"бронирование днища:  +"+ b.value*100+"%");break;
					case 12: ps=ps.Insert(a,"броня на левом крыле:  +"+ b.value*100+"%");break;
					case 13: ps=ps.Insert(a,"броня на правом крыле:  +"+ b.value*100+"%");break;
				}
					if (a!=256) {
					if (b.second_bonus_type!=10) {
							a+=32;
						switch (b.second_bonus_type) {
						case 0: ps=ps.Insert(a,"выход реактора:  +"+ b.value*100+"%");break;
						case 1: ps=ps.Insert(a,"маневренность:  +"+ b.value*100+"%");break;
						case 2: ps=ps.Insert(a,"скорость:  +"+ b.value*100+"%");break;
						case 3: ps=ps.Insert(a,"мощность орудий:  +"+ b.value*100+"%");break;
						case 4: ps=ps.Insert(a,"дальность радара:  +"+ b.value*100+"%");break;
						case 5: ps=ps.Insert(a,"время возрождения:  -"+ b.value*100+"%");break;
						case 6: ps=ps.Insert(a,"мощность щитов:  +"+ b.value*100+"%");break;
						case 7: ps=ps.Insert(a,"бронирование:  +*"+ b.value*100+"%");break;
						case 8: ps=ps.Insert(a,"носовая броня:  +"+ b.value*100+"%");break;
						case 9: ps=ps.Insert(a,"таран:  +"+ b.value*100+"%");break;
						case 11: ps=ps.Insert(a,"бронирование днища:  +"+ b.value*100+"%");break;
						case 12: ps=ps.Insert(a,"броня на левом крыле:  +"+ b.value*100+"%");break;
						case 13: ps=ps.Insert(a,"броня на правом крыле:  +"+ b.value*100+"%");break;
						}
					}
					}
					if (a!=256) {
					a+=32;
					ps=ps.Insert(a,"энергозатраты: "+ b.energy_consumption);
					}
				}
			}
		}


	}
}
