using UnityEngine;
using System.Collections;

public class spawn : MonoBehaviour {

    public GameObject childObject;
    public float spawnDelay = 3;

	// Use this for initialization
	void Start () {
        if (childObject) {
            InvokeRepeating("SpawnObject", spawnDelay, spawnDelay);
        }     
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void SpawnObject()
    {
        if (childObject)
        {
            Instantiate(childObject, transform.position, Random.rotation);
        }
    }

}
