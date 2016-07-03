using UnityEngine;
using System.Collections;

public class EngineController : MonoBehaviour {

	public float power = 5.0F;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetButtonDown ("Jump")) {
		    GetComponent<Rigidbody> ().AddRelativeForce (transform.forward * power, ForceMode.Impulse);
		}
	}
}
