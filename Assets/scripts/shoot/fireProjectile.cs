using UnityEngine;
using System.Collections;

public class fireProjectile : MonoBehaviour {

    public GameObject _objProj;
    public GameObject _projParent;
    public float _distance = 10.0f;

    // Use this for initialization
    void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _distance);

            position = Camera.main.ScreenToWorldPoint(position);
            GameObject objFired = Instantiate(_objProj, Camera.main.transform.position, Quaternion.identity) as GameObject;
            objFired.transform.parent = _projParent.transform;
            objFired.transform.LookAt(position);
            objFired.GetComponent<Rigidbody>().AddForce(objFired.transform.forward * 1000);
        }
    }
}
