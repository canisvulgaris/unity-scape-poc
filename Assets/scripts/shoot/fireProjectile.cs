﻿using UnityEngine;
using System.Collections;

public class FireProjectile : MonoBehaviour {

    public GameObject _objProj1;
    public GameObject _objProj2;
    public GameObject _projParent;
    public float _distance = 10.0f;

	private bool buttonClicked = false;

    // Use this for initialization
    void Start () {
	
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        //TODO : need to set expiration for projectiles so they dissappear eventually
        if (Input.GetMouseButtonDown(0))
        {
			if (!buttonClicked) {
				buttonClicked = true;
				var position = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, _distance);

				position = Camera.main.ScreenToWorldPoint (position);
				GameObject objFired = Instantiate (_objProj1, Camera.main.transform.position, Quaternion.identity) as GameObject;
				objFired.transform.parent = _projParent.transform;
				objFired.transform.LookAt (position);
				objFired.GetComponent<Rigidbody> ().AddForce (objFired.transform.forward * 1000);
				StartCoroutine(ResetButtonClicked());
			}
        }

        if (Input.GetMouseButtonDown(1))
        {
			if (!buttonClicked) {
				buttonClicked = true;
				var position = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, _distance);

				position = Camera.main.ScreenToWorldPoint (position);
				GameObject objFired = Instantiate (_objProj2, Camera.main.transform.position, Quaternion.identity) as GameObject;
				objFired.transform.parent = _projParent.transform;
				objFired.transform.LookAt (position);
				objFired.GetComponent<Rigidbody> ().AddForce (objFired.transform.forward * 1000);
				StartCoroutine (ResetButtonClicked ());
			}
        }
    }

	IEnumerator ResetButtonClicked() {		
		yield return new WaitForSeconds(0.01f);
		buttonClicked = false;
	}

}
