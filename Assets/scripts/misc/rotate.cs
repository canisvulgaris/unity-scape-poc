using UnityEngine;
using System.Collections;

public class rotate : MonoBehaviour {

    private float multiplier = 20;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up * multiplier * Time.deltaTime, Space.World);
	}
}
