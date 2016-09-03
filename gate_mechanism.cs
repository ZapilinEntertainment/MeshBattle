using UnityEngine;
using System.Collections;

public class gate_mechanism : MonoBehaviour {
	public ParticleEmitter gate_effect;
	public GameObject gate_sprite;
	public Light gate_light;
	public GameObject teleporter;
	public mission1 m;
	public bool gates_open=false;
	


	public void ApplyDamage(Vector4 v4) {
		gate_effect.emit=true;
		gate_sprite.SetActive(true);
		gate_light.enabled=true;
		teleporter.SetActive(true);
		if (!gates_open) {m.SendShip();gates_open=true;}
	}


}
