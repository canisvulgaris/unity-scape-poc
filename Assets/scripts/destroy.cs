using UnityEngine;
using System.Collections;

public class destroy : MonoBehaviour {

	public float ttl = 10;

	// Use this for initialization
	void Start () {
		StartCoroutine (Timer ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	IEnumerator Timer() {
		yield return new WaitForSeconds(ttl);
		Destroy (gameObject);
	}
}
