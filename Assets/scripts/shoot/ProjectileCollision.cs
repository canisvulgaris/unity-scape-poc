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

			CreateAreaOfEffectSphere ();

			int terrainLayer = 1 << LayerMask.NameToLayer("Terrain");
            Collider[] hitColliders = Physics.OverlapSphere(_explosionRadiusObj.transform.position, radius, terrainLayer);
					
            for (int i = 0; i < hitColliders.Length; i ++)
            {
				Mesh collisionMesh = hitColliders[i].GetComponent<MeshFilter> ().mesh;
				int[] verticesInBounds = GetVerticesInBounds (hitColliders[i], collisionMesh);

				if (verticesInBounds.Length > 0){
					Vector3[] collisionMeshVertices = collisionMesh.vertices;
					
					for (int j = 0; j < verticesInBounds.Length; j++) {

						float distanceToSphereCenter = Vector3.Distance(collisionMeshVertices [verticesInBounds[j]], _explosionRadiusObj.transform.position);
						Debug.Log ("distanceToSphereCenter: " + distanceToSphereCenter);

						if (radius > distanceToSphereCenter) { // && collisionMeshVertices [verticesInBounds[j]].y < _explosionRadiusObj.transform.position.y) {
							collisionMeshVertices [verticesInBounds [j]] = Vector3.MoveTowards (collisionMeshVertices [verticesInBounds [j]], _explosionRadiusObj.transform.position, -1.0f * (radius - distanceToSphereCenter));
						}
						else if (radius < distanceToSphereCenter) {
							collisionMeshVertices [verticesInBounds [j]] = Vector3.MoveTowards (collisionMeshVertices [verticesInBounds [j]], _explosionRadiusObj.transform.position, distanceToSphereCenter - radius);
						}

						Debug.Log ("NEW distanceToSphereCenter: " + Vector3.Distance(collisionMeshVertices [verticesInBounds[j]], _explosionRadiusObj.transform.position));

											
//						if (radius > distanceToSphereCenter) {
//							collisionMeshVertices [verticesInBounds [j]].y -= (radius - distanceToSphereCenter);// + (Random.value * 0.2f);
//						} else {
//							collisionMeshVertices [verticesInBounds [j]].y += (distanceToSphereCenter - radius);// + (Random.value * 0.2f);
//						}

//						if (collisionMeshVertices [verticesInBounds [j]].x > _explosionRadiusObj.transform.position.x) {
//							collisionMeshVertices [verticesInBounds [j]].x += Random.value * 0.5f;
//						} else {
//							collisionMeshVertices [verticesInBounds[j]].x -= Random.value * 0.5f;
//						}
//							
//						if (collisionMeshVertices [verticesInBounds [j]].z > _explosionRadiusObj.transform.position.z) {
//							collisionMeshVertices [verticesInBounds[j]].z += Random.value * 0.5f;
//						} else {
//							collisionMeshVertices [verticesInBounds[j]].z -= Random.value * 0.5f;
//						}
					}

					collisionMesh.vertices = collisionMeshVertices;
					MeshCollider collisionMeshCollider = hitColliders [i].GetComponent<MeshCollider> ();
					collisionMeshCollider.sharedMesh = null;
					collisionMeshCollider.sharedMesh = collisionMesh;
				}
            }

			Destroy (_explosionRadiusObj);
            Destroy (gameObject);
        }
    }

	void CreateAreaOfEffectSphere() {
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