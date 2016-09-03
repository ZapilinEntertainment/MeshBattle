using UnityEngine;
using System.Collections;

public class timer : MonoBehaviour {

	public float time=3;
	public bool effect=false;

	void Start () {
		StartCoroutine(FinalCoroutine(time));
	}

	IEnumerator FinalCoroutine (float t) {
		yield return new WaitForSeconds(t);
		if (!effect) Destroy(gameObject); else gameObject.SetActive(false);
	}

	public void Destroying () {
		if (effect) {transform.parent=null;gameObject.SetActive(false);}
	}
}
