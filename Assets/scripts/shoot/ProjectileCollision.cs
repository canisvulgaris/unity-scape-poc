using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

			ShowAreaOfEffectSphere ();
                      
			int terrainLayer = 1 << LayerMask.NameToLayer("Terrain");
            Collider[] hitColliders = Physics.OverlapSphere(_explosionRadiusObj.transform.position, _explosionRadiusObj.transform.localScale.x, terrainLayer);
            for (int i = 0; i < hitColliders.Length; i ++)
            {
                //Debug.Log("mesh contact: " + hitColliders[i].transform.GetComponent<MeshFilter>().mesh.name);

				int[] verticesInBounds = GetVerticesInBounds (hitColliders[i]);
				if (verticesInBounds.Length > 0){

					Mesh collisionMesh = hitColliders[i].transform.GetComponent<MeshFilter> ().mesh;
					Vector3[] collisionMeshVertices = collisionMesh.vertices;
					
					for (int j = 0; j < verticesInBounds.Length; j++) {
						collisionMeshVertices [j].y -= 3;
					}

					collisionMesh.vertices = collisionMeshVertices;
				}
            }

            Destroy(gameObject);
        }
    }

	void ShowAreaOfEffectSphere() {
		//show area of effect
		_explosionRadiusObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		_explosionRadiusObj.gameObject.name = "explosionRadiusSphere";
		_explosionRadiusObj.transform.localScale = new Vector3(radius, radius, radius);
		_explosionRadiusObj.transform.position = transform.position;
		_explosionRadiusObj.transform.rotation = transform.rotation;
		_explosionRadiusObj.transform.parent = _projectileParent.transform;
		//_explosionRadiusObj.GetComponent<MeshRenderer> ().enabled = false;
	}


	int[] GetVerticesInBounds(Collider collider) {

		int[] matchedVerticesArray;
		List<int> matchedVerticesList = new List<int>();
		Mesh collisionMesh = collider.transform.GetComponent<MeshFilter> ().mesh;

		for (int iter = 0; iter < 1; iter++) {
			//Debug.Log ("collisionMesh.vertices [iter]: " + collisionMesh.vertices [iter]);
			//Debug.Log ("collider.transform.TransformPoint(collisionMesh.vertices [iter])): " + collider.transform.TransformPoint(collisionMesh.vertices [iter]));
			//Debug.Log ("transform.InverseTransformPoint(collider.transform.TransformPoint(collisionMesh.vertices [iter])): " + transform.InverseTransformPoint (collider.transform.TransformPoint (collisionMesh.vertices [iter])));

			if (collider.bounds.Contains (transform.TransformPoint(collisionMesh.vertices [iter]))) {
				Debug.Log ("matched vert: " + collisionMesh.vertices [iter]);
				matchedVerticesList.Add(iter);
			}
		}

		matchedVerticesArray = matchedVerticesList.ToArray();

		return matchedVerticesArray;
	}
}