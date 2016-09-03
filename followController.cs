using UnityEngine;
using System.Collections;

public class followController : MonoBehaviour {
	public int safeDist=30;
	public float conusAngle=30;
	public float angle_step=20;


	public void GetFollowPoint(GameObject[] ships) {
		int number=int.Parse(gameObject.name.Substring(1,2));
		int step=1; 
		int step_pos_count=3; 
		int current_ins_pos=0;
		int zpos=safeDist;
		float insAngle=120;
		float current_ins_angle_pos=0;
		float radius=zpos/Mathf.Cos(conusAngle);
		float current_null_angle=0;
		Vector3 nullPoint=new Vector3(0,radius,zpos);

		for (byte i=0;i<ships.Length;i++) {
			Vector3 pos=Vector3.zero;
			if (current_ins_pos==0) {
				pos=nullPoint;
			}
			else {
				if (current_ins_pos==1&&step_pos_count%2==0) {
					pos=new Vector3(-nullPoint.x,-nullPoint.y,zpos);
				}
				else {
					if (current_ins_angle_pos>insAngle) {current_ins_angle_pos=insAngle-(current_ins_angle_pos-insAngle);}
					else {current_ins_angle_pos+=insAngle;}
					pos=new Vector3(radius*Mathf.Sin(current_ins_angle_pos),radius*Mathf.Cos(current_ins_angle_pos),zpos);
				}

			}
			current_ins_pos++;
 //проверка на заполнение кольца
			if (current_ins_pos==step_pos_count) {step++;
				zpos+=safeDist;
				radius=zpos/Mathf.Cos(conusAngle);
				insAngle=360/(step+2);
				if (current_ins_angle_pos>360) current_ins_angle_pos-=360;
				current_ins_pos=0;
				current_null_angle+=angle_step;
				nullPoint=new Vector3(radius*Mathf.Sin(current_null_angle),radius*Mathf.Cos(current_null_angle),zpos);
				step_pos_count++;}

			print (pos);
			ships[i].SendMessage("Follow",new Vector4(pos.x,pos.y,pos.z,number),SendMessageOptions.DontRequireReceiver);
		}
	}
}
