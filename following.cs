using UnityEngine;
using System.Collections;

public class following : MonoBehaviour {
	public GameObject target;
	// Use this for initialization

	
	// Update is called once per frame
	void Update () {
		transform.position=target.transform.position;
	}
}
