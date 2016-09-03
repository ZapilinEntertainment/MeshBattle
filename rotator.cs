using UnityEngine;
using System.Collections;

public class rotator : MonoBehaviour {

	public Vector3 rvector;
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.FromToRotation(Vector3.up, transform.forward);
	}
}
