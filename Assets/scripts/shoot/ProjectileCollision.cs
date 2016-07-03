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
			//Debug.Log ("contact: " + collision.collider.name);

			ShowAreaOfEffectSphere ();

			int terrainLayer = 1 << LayerMask.NameToLayer("Terrain");
            Collider[] hitColliders = Physics.OverlapSphere(_explosionRadiusObj.transform.position, radius, terrainLayer);
					
            for (int i = 0; i < hitColliders.Length; i ++)
            {
                //Debug.Log("mesh contact: " + hitColliders[i].transform.GetComponent<MeshFilter>().mesh.name);

				Mesh collisionMesh = hitColliders[i].transform.GetComponent<MeshFilter> ().mesh;
				int[] verticesInBounds = GetVerticesInBounds (hitColliders[i], collisionMesh);

				if (verticesInBounds.Length > 0){
					Vector3[] collisionMeshVertices = collisionMesh.vertices;
					
					for (int j = 0; j < verticesInBounds.Length; j++) {
						//Debug.Log ("updating vertices -  j: " + j );
						collisionMeshVertices [verticesInBounds[j]].y -= 3;
					}

					collisionMesh.vertices = collisionMeshVertices;
				}
            }

			Destroy (_explosionRadiusObj);
            Destroy (gameObject);
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
		_explosionRadiusObj.GetComponent<MeshRenderer> ().enabled = false;
	}


	int[] GetVerticesInBounds(Collider collider, Mesh collisionMesh) {

		int[] matchedVerticesArray;
		List<int> matchedVerticesList = new List<int>();
		Color color = new Color (Random.value, Random.value, Random.value, 1.0f);

		for (int iter = 0; iter < collisionMesh.vertexCount; iter++) {
			Vector3 convertedVertex = collider.transform.TransformPoint (collisionMesh.vertices [iter]);

			if (_explosionRadiusObj.GetComponent<SphereCollider>().bounds.Contains (convertedVertex)) {
				//Debug.DrawRay (convertedVertex, Vector3.up, color, 50.0f);
				//Debug.Log ("matched vert - iter: " + iter + " - vector: " + collisionMesh.vertices [iter]);
				matchedVerticesList.Add(iter);
			}
		}

		matchedVerticesArray = matchedVerticesList.ToArray();

		return matchedVerticesArray;
	}
}