using UnityEngine;
using System.Collections;

public class engineController : MonoBehaviour {

	public float power = 5.0F;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetButtonDown ("Jump")) {
			float rand = Random.value;
			if (rand > 0.75) {
				GetComponent<Rigidbody> ().AddRelativeForce (Vector3.forward * power, ForceMode.Impulse);
			} else if (rand > 0.5) {
				GetComponent<Rigidbody> ().AddRelativeForce (Vector3.back * power, ForceMode.Impulse);				
			} else if (rand > 0.25) {
				GetComponent<Rigidbody> ().AddRelativeForce (Vector3.left * power, ForceMode.Impulse);								
			} else {
				GetComponent<Rigidbody> ().AddRelativeForce (Vector3.right * power, ForceMode.Impulse);			
			}
		}	
	}
}
