using UnityEngine;
using System.Collections;

public class FireProjectileVehicle : MonoBehaviour {

    public GameObject _objProj;
    //public GameObject _objProj2;
    public GameObject _objProjOrigin;
    public GameObject _objProjTarget;
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
		if (Input.GetKey(KeyCode.M))
        {
			if (!buttonClicked) {
				buttonClicked = true;
				//var position = new Vector3 (Input.mousePosition.x + Random.value * 10, Input.mousePosition.y + Random.value * 10, _distance);
				//position = Camera.main.ScreenToWorldPoint (position);

				GameObject objFired = Instantiate(_objProj, _objProjOrigin.transform.position, Quaternion.identity) as GameObject;

				objFired.transform.parent = _projParent.transform;
				objFired.transform.LookAt (_objProjTarget.transform.position);
				objFired.GetComponent<Rigidbody> ().AddForce (objFired.transform.forward * _force);
				StartCoroutine(ResetButtonClicked());
			}
        }
    }

	IEnumerator ResetButtonClicked() {		
		yield return new WaitForSeconds(0.2f);
		buttonClicked = false;
	}
}
