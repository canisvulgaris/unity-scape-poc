using UnityEngine;
using System.Collections;

public class ProjectileCollision : MonoBehaviour
{    
    public float radius = 5;

    private GameObject _explosionRadiusObj;
    private GameObject _projectileParent;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.tag == "Terrain")
        {
            //decrease terrain height within radius of _explosionRadiusObj
            _projectileParent = GameObject.Find("ProjectileParent");

            //show area of effect
            _explosionRadiusObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _explosionRadiusObj.transform.localScale = new Vector3(radius, radius, radius);
            _explosionRadiusObj.transform.position = transform.position;
            _explosionRadiusObj.transform.rotation = transform.rotation;
            //GameObject explosionRadiusInstance = Instantiate(_explosionRadiusObj, transform.position, transform.rotation) as GameObject;
            _explosionRadiusObj.transform.parent = _projectileParent.transform;            

            LayerMask terrainLayer = LayerMask.NameToLayer("Terrain");

            Collider[] hitColliders = Physics.OverlapSphere(_explosionRadiusObj.transform.position, _explosionRadiusObj.transform.localScale.x, terrainLayer);
            for (int i = 0; i < hitColliders.Length; i ++)
            {
                Debug.Log("mesh contact: " + hitColliders[i].transform.GetComponent<MeshFilter>().mesh.name);
            }

            Destroy(gameObject);
        }
    }
}