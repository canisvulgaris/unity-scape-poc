using UnityEngine;
using System.Collections;

public class CreateDebris : MonoBehaviour {

    public GameObject _debrisParticleSystemObj;

    private GameObject _projectileParent;
	private GameObject _debrisObj;

    // Use this for initialization
    void Start () {
		_projectileParent = GameObject.Find("ProjectileParent");		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void createDebrisParticleSystem () {
		_debrisObj = Instantiate (_debrisParticleSystemObj, transform.position + new Vector3(0, 1, 0), transform.rotation) as GameObject;
		_debrisObj.transform.parent = _projectileParent.transform;
		StartCoroutine (CleanUpDebris ());
	}

	IEnumerator CleanUpDebris() {
		yield return new WaitForSeconds(10.0f);
		Destroy (_debrisObj);
	}
}
