using UnityEngine;
using System.Collections;

public class CreateDebris : MonoBehaviour {

    public GameObject _debrisParticleSystemObj;
    public GameObject _smokeParticleSystemObj;

    private GameObject _projectileParent;
	private GameObject _debrisObj;
    private GameObject _smokeObj;

    // Use this for initialization
    void Start () {
		_projectileParent = GameObject.Find("ProjectileParent");		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void createDebrisParticleSystem () {
		_debrisObj = Instantiate (_debrisParticleSystemObj, transform.position + new Vector3(0, -1, 0), Quaternion.Euler(-90, 0, 0)) as GameObject;
		_debrisObj.transform.parent = _projectileParent.transform;
        Destroy(_debrisObj.gameObject, 10.0f);

        _smokeObj = Instantiate(_smokeParticleSystemObj, transform.position + new Vector3(0, -1, 0), Quaternion.Euler(-90, 0, 0)) as GameObject;
        _smokeObj.transform.parent = _projectileParent.transform;
        Destroy(_smokeObj.gameObject, 10.0f);
    }
}
