using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileCollision : MonoBehaviour
{
    public float _areaOfEffectRadius = 5.0f;
    public float _explosiveRadius = 5.0f;
    public float _explosiveForce = 5.0f;
    public Color _explosionDecalColor;

    private GameObject _explosionRadiusObj;
    private GameObject _projectileParent;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Terrain")
        {
            //decrease terrain height within radius of _explosionRadiusObj
            _projectileParent = GameObject.Find("ProjectileParent");

            _areaOfEffectRadius += (Random.value * 6) - 3;

            CreateAreaOfEffectSphere();

            int terrainLayer = (1 << LayerMask.NameToLayer("Terrain")) | (1 << LayerMask.NameToLayer("PhysicsObject"));
            Collider[] hitColliders = Physics.OverlapSphere(_explosionRadiusObj.transform.position, _areaOfEffectRadius, terrainLayer);

            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].tag == "Terrain")
                {
                    Mesh collisionMesh = hitColliders[i].GetComponent<MeshFilter>().mesh;
                    int[] verticesInBounds = GetVerticesInBounds(hitColliders[i], collisionMesh);

                    GameObject terrainControllerObject = GameObject.Find("TerrainController");
                    TerrainController terrainController = (TerrainController)terrainControllerObject.GetComponent<TerrainController>();
                    List<GameObject> localObjRef = terrainController.getObjRefArray();
                    int indexObjRef = localObjRef.IndexOf(hitColliders[i].gameObject);

                    int localArrayLength = terrainController.getArrayLength();
                    int localMeshLimit = terrainController._meshLimit;
                    int meshTotalPerLength = (localArrayLength / localMeshLimit);
                    int meshColorIndex = -1;
                    int meshRows = indexObjRef % meshTotalPerLength;
                    int meshCols = (int)Mathf.Floor((indexObjRef * 1.0f) / (meshTotalPerLength * 1.0f));
                    Color meshColor;

                    if (meshRows == 0)
                    {
                        meshColorIndex = localMeshLimit * meshCols;
                    }
                    else
                    {
                        if (meshCols == 0)
                        {
                            meshColorIndex = localArrayLength * localMeshLimit * meshRows;
                        }
                        else
                        {
                            meshColorIndex = (localArrayLength * localMeshLimit * meshRows) + (localMeshLimit * meshCols);
                        }
                    }

                    if (verticesInBounds.Length > 0)
                    {
                        Vector3[] collisionMeshVertices = collisionMesh.vertices;

                        float circleRadius = _areaOfEffectRadius * 0.6f;

                        for (int j = 0; j < verticesInBounds.Length; j++)
                        {

                            float distanceToSphereCenter = Vector3.Distance(collisionMeshVertices[verticesInBounds[j]], _explosionRadiusObj.transform.position);

                            if (circleRadius > distanceToSphereCenter && collisionMeshVertices[verticesInBounds[j]].y < _explosionRadiusObj.transform.position.y)
                            {
                                collisionMeshVertices[verticesInBounds[j]] = Vector3.MoveTowards(collisionMeshVertices[verticesInBounds[j]], _explosionRadiusObj.transform.position, -1.0f * (circleRadius - distanceToSphereCenter));
                            }

                            //update the color of vertices
                            if (meshColorIndex > -1)
                            {
                                int vertexXIndex = (int)Mathf.Floor((verticesInBounds[j] * 1.0f) / (localMeshLimit * 1.0f));
                                int vertexYIndex = (verticesInBounds[j] % (localMeshLimit + 1));
                                int updateColorIndex = 0;

                                if (verticesInBounds[j] > localMeshLimit)
                                {
                                    updateColorIndex = meshColorIndex + (localArrayLength * vertexYIndex) + vertexXIndex;
                                }
                                else
                                {
                                    updateColorIndex = meshColorIndex + (localArrayLength * vertexYIndex);
                                }
                                terrainController.setMeshColorArrayValue(updateColorIndex, _explosionDecalColor);
                            }
                        }

                        collisionMesh.vertices = collisionMeshVertices;
                        MeshCollider collisionMeshCollider = hitColliders[i].GetComponent<MeshCollider>();
                        collisionMeshCollider.sharedMesh = null;
                        collisionMeshCollider.sharedMesh = collisionMesh;

                        terrainController.updateTerrainTexture();
                        if (meshCols == 0)
                        {
                            terrainController.updateMeshMaterials(indexObjRef, 0, localMeshLimit * meshRows);
                        }
                        else
                        {
                            terrainController.updateMeshMaterials(indexObjRef, localMeshLimit * meshCols, localMeshLimit * meshRows);
                        }
                    }

                    //create debris
                    GetComponent<CreateDebris>().createDebrisParticleSystem();
                }

                if (hitColliders[i].tag == "PhysicsObject")
                {
                    //create explosion
                    Rigidbody rigidbody = hitColliders[i].attachedRigidbody;
                    rigidbody.AddExplosionForce(_explosiveForce, transform.position, _explosiveRadius, 5, ForceMode.Impulse);
                }
            }

            Destroy(_explosionRadiusObj);
            Destroy(gameObject);
        }
    }

    void CreateAreaOfEffectSphere()
    {
        //show area of effect
        _explosionRadiusObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _explosionRadiusObj.gameObject.name = "explosionRadiusSphere";
        _explosionRadiusObj.transform.localScale = new Vector3(_areaOfEffectRadius, _areaOfEffectRadius, _areaOfEffectRadius);
        _explosionRadiusObj.transform.position = transform.position;
        _explosionRadiusObj.transform.rotation = transform.rotation;
        _explosionRadiusObj.transform.parent = _projectileParent.transform;
        _explosionRadiusObj.GetComponent<MeshRenderer>().enabled = false;
    }


    int[] GetVerticesInBounds(Collider collider, Mesh collisionMesh)
    {

        int[] matchedVerticesArray;
        List<int> matchedVerticesList = new List<int>();

        for (int iter = 0; iter < collisionMesh.vertexCount; iter++)
        {
            Vector3 convertedVertex = collider.transform.TransformPoint(collisionMesh.vertices[iter]);

            if (_explosionRadiusObj.GetComponent<SphereCollider>().bounds.Contains(convertedVertex))
            {
                //Debug.DrawRay(convertedVertex, Vector3.up, new Color(colorValue, 0.0f, 0.0f), 50.0f);
                matchedVerticesList.Add(iter);
            }
        }

        matchedVerticesArray = matchedVerticesList.ToArray();

        return matchedVerticesArray;
    }
}