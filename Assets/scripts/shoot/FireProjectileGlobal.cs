using UnityEngine;
using System.Collections;

public class FireProjectileGlobal : MonoBehaviour {

    public GameObject _objProj1;
    public GameObject _objProj2;
    public GameObject _projParent;
    public float _distance = 10.0f;
    public float _force = 2000.0f;

    private bool buttonClicked = false;

    // Use this for initialization
    void Start () {
	
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        //TODO : need to set expiration for projectiles so they dissappear eventually
		if (Input.GetKey(KeyCode.K))
        {
			if (!buttonClicked) {
				buttonClicked = true;
				var position = new Vector3 (Input.mousePosition.x + Random.value * 10, Input.mousePosition.y + Random.value * 10, _distance);

				position = Camera.main.ScreenToWorldPoint (position);
				GameObject objFired = Instantiate (_objProj1, Camera.main.transform.position, Quaternion.identity) as GameObject;
				objFired.transform.parent = _projParent.transform;
                
                float objScale = Random.value * 5.0f; //randomize size
                objScale = (objScale < 1.0f) ? objScale + 1.0f : objScale; //make sure size is greater pr equal to 1

                objFired.transform.localScale = new Vector3(objScale, objScale, objScale);
				objFired.transform.LookAt (position);
				objFired.GetComponent<Rigidbody> ().AddForce (objFired.transform.forward * _force);
				StartCoroutine(ResetButtonClicked());
			}
        }

		if (Input.GetKey(KeyCode.L))
        {
			if (!buttonClicked) {
				buttonClicked = true;
				var position = new Vector3 (Input.mousePosition.x + Random.value * 10, Input.mousePosition.y + Random.value * 10, _distance);

				position = Camera.main.ScreenToWorldPoint (position);
				GameObject objFired = Instantiate (_objProj2, Camera.main.transform.position, Quaternion.identity) as GameObject;
				objFired.transform.parent = _projParent.transform;
				objFired.transform.LookAt (position);
				objFired.GetComponent<Rigidbody> ().AddForce (objFired.transform.forward * _force);
				StartCoroutine (ResetButtonClicked ());
			}
        }
    }

	IEnumerator ResetButtonClicked() {		
		yield return new WaitForSeconds(0.2f);
		buttonClicked = false;
	}

}
