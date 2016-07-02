using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainController : MonoBehaviour {

    public GameObject[] _objectsAvailable;
    public int _objSelection;
    private GameObject _objType;
    public GameObject _terrainParent;
    public GameObject _borderBlock;
    public GameObject _borderParent;


    public int _gridExponential = 10;
	public float _terrainRoughness = 0.08F;
	public float _objSize = 1;
    public int _meshLimit = 16;
    public int _normalSmoothAngle = 60;

    private int _arrayIndex;
	private int _arrayLength;
	private Vector3[,] _positionArray;
	private TerrainObject[,] _objArray;
    private List<GameObject> _objRef = new List<GameObject>();

    //private System.DateTime _startTime = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
    //private float _count = 0;

    /*************************************************************** 
     * Initialization
     * 
     ***************************************************************/
    void Start () {
        //init
        RefreshTerrain();
    }

    public void RefreshTerrain()
    {
        //Debug.Log("called RefreshTerrain");
        _objType = _objectsAvailable[_objSelection];

        if (_objType == null)
        {
            Debug.LogError("TerrainController - _objType not set");
        }
        if (_terrainParent == null)
        {
            Debug.LogError("TerrainController - _terrainParent not set");
        }

        ClearTerrain();
        BuildTerrain();
    }

    public void ClearTerrain()
    {
        //Debug.Log("called ClearTerrain");
        for (var i = 0; i < _terrainParent.transform.childCount; i++)
        {
            Destroy(_terrainParent.transform.GetChild(i).gameObject);
        }
    }

    public void BuildTerrain()
    {
        //Debug.Log("called BuildTerrain");
        //set up initial params
        _arrayIndex = (int)Mathf.Pow(2, _gridExponential);
        _arrayLength = _arrayIndex + 1;
        _positionArray = new Vector3[_arrayLength, _arrayLength];

        CalcTerrainHeightMap();
        AddBorder();

        if (_objType.name == "mesh")
        {
            //Debug.Log("found mesh object");
            //set up the _positionArray height map using the Diamond-Square algorithm             
            createMeshTerrain(_positionArray, _meshLimit);
            updateVertexNormalsForAllMeshes();
        }
        else
        {
            //Debug.Log("found terrain object");
            _objArray = new TerrainObject[_arrayLength, _arrayLength];
            createPositionalTerrainUsingObject(_positionArray);
        }
    }

    /***************************************************************
    * creates a border around the terrain
    * 
   ***************************************************************/
    void AddBorder()
    {
        //TODO : need to remove old borders when refreshing terrain
        float borderHeight = 64;
        float borderWidth = 2;

        GameObject borderN = Instantiate(_borderBlock, new Vector3(-_arrayIndex, 0, _arrayIndex/2), Quaternion.identity) as GameObject;
        borderN.transform.localScale = new Vector3(borderWidth, borderHeight, _arrayIndex);
        borderN.transform.parent = _borderParent.transform;

        GameObject borderE = Instantiate(_borderBlock, new Vector3(-_arrayIndex / 2, 0, _arrayIndex), Quaternion.identity) as GameObject;
        borderE.transform.localScale = new Vector3(_arrayIndex, borderHeight, borderWidth);
        borderE.transform.parent = _borderParent.transform;

        GameObject borderS = Instantiate(_borderBlock, new Vector3(0, 0, _arrayIndex / 2), Quaternion.identity) as GameObject;
        borderS.transform.localScale = new Vector3(borderWidth, borderHeight, _arrayIndex);
        borderS.transform.parent = _borderParent.transform;

        GameObject borderW = Instantiate(_borderBlock, new Vector3(-_arrayIndex / 2, 0, 0), Quaternion.identity) as GameObject;
        borderW.transform.localScale = new Vector3(_arrayIndex, borderHeight, borderWidth);
        borderW.transform.parent = _borderParent.transform;

    }

    /***************************************************************
    * Calculate the Terrain Height Map using Diamond Square Algorithm
    * 
    ***************************************************************/
    public void CalcTerrainHeightMap()
    {
        if (_positionArray == null || _arrayLength == 0 || _arrayIndex == 0)
        {
            Debug.LogError("TerrainController - CalcTerrainHeightMap - params not set");
        }

        //create random terrain positions
        updatePositionUsingDiamondSquare();
        //normalizeBoundaries();
    }

    /***************************************************************
     * creates a new mesh for the object (does not use terrainobject)
     * 
    ***************************************************************/
    void createMeshTerrain(Vector3[,] posArray, int meshLimit) {

        int meshLength = _arrayIndex % meshLimit;

        if (meshLength > 0)
        {
            Debug.LogError("meshLimit needs to be a factor of the position total.");
        }

        int meshCount = meshLength * meshLength;

        //generateMesh(meshLimit, _objSize, 0, 32);

        for (int x = 0; x < _arrayIndex; x += meshLimit)
        {
            for (int y = 0; y < _arrayIndex; y += meshLimit)
            {
                //Debug.Log("generateMesh - meshLimit: " + meshLimit + " -_objSize: " + _objSize + " - x: " + x + " - y: " + y);
                generateMesh(meshLimit + 1, _objSize, x, y);
            }
        }
    }

    /***************************************************************
     * generate the mesh based on length required and size
     * 
    ***************************************************************/
    void generateMesh(int length, float size, int startx, int starty)
    {
        Vector3[] meshVertices = new Vector3[length * length];
        Vector2[] meshUV = new Vector2[length * length];
        int[] meshTriangles = new int[(3 * 2 * (length - 1) * (length - 1))];

        //create a new mesh 
        Mesh mesh = new Mesh();

        //set up the mesh vertices array
        int vert_index = 0;
        for (int i = startx; i < length + startx; i++)
        {
            for (int j = starty; j < length + starty; j++)
            {
                meshVertices[vert_index] = _positionArray[i, j];
                vert_index++;
            }
        }

        //set up the mesh uv array
        int uv_index = 0;
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                meshUV[uv_index] = new Vector2(i, j);
                uv_index++;
            }
        }

        //set up the mesh triangles array
        int tri_index = 0;
        for (int x = 0; x < length - 1; x++)
        {
            for (int y = 0; y < length - 1; y++)
            {
                //add in first three vertices for mesh
                meshTriangles[tri_index] = (x * length) + y;
                meshTriangles[tri_index + 1] = ((x + 1) * length) + y;
                meshTriangles[tri_index + 2] = (x * length) + y + 1;

                //add in second three vertices for mesh
                meshTriangles[tri_index + 3] = ((x + 1) * length) + y;
                meshTriangles[tri_index + 4] = ((x + 1) * length) + y + 1;
                meshTriangles[tri_index + 5] = (x * length) + y + 1;

                //bump index by 6 for previous puts
                tri_index += 6;
            }
        }

        //Debug.Log("about to calc normals");

        mesh.name = "terrain-" + length + "-"+ startx + "-" + starty;
        mesh.Clear();
        mesh.vertices = meshVertices;
        mesh.uv = meshUV;
        mesh.triangles = meshTriangles;

        //mesh.RecalculateNormals();
        mesh.RecalculateNormals(_normalSmoothAngle);
        mesh.RecalculateBounds();
        mesh.Optimize();

        //instantiate object with newly added mesh       
        GameObject newMeshObj = Instantiate(_objType, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 180)) as GameObject;
        newMeshObj.GetComponent<MeshFilter>().mesh = mesh;
        newMeshObj.AddComponent<MeshCollider>();
        newMeshObj.transform.localScale = new Vector3(size, size, size);
        newMeshObj.transform.parent = _terrainParent.transform;

        //add to object ref array
        _objRef.Add(newMeshObj);

        //GameObject objN = Instantiate(_objRef, new Vector3(0, 0, _arrayIndex * _objSize), Quaternion.Euler(0, 0, 180)) as GameObject;
        //objN.transform.parent = _terrainParent.transform;
    }

    /***************************************************************
     * Generate a flat terrain using TerrainObject
     * 
     ****************************************************************/
    void createFlatTerrainUsingObject() {
		int length = _arrayLength;

		for (int i = 0; i < length; i++) {
			for (int j = 0; j < length; j++) {
				Vector3 pos = new Vector3(i, 0, j);
                TerrainObject obj = new TerrainObject(_objType, pos * _objSize, _objSize, _terrainParent);
                _objArray[i, j] = obj;
			}
		}
	}

    /***************************************************************
     * Generate a terrain based on position array using TerrainObject
     * 
     ****************************************************************/
    void createPositionalTerrainUsingObject(Vector3[,] posArray) {
		int length = _arrayLength;

		for (int i = 0; i < length; i++) {
			for (int j = 0; j < length; j++) {
				Vector3 pos = posArray [i, j];
                TerrainObject obj = new TerrainObject(_objType, pos * _objSize, _objSize, _terrainParent);
                _objArray[i, j] = obj;
            }
		}
	}

    /***************************************************************
    * update existing positional terrain using TerrainObject
    * 
    ****************************************************************/
    void updateDynamicTerrainUsingObject(Vector3[,] posArray) {
		int length = _arrayLength;

		for (int i = 0; i < length; i++) {
			for (int j = 0; j < length; j++) {
				Vector3 pos = posArray[i, j];
                _objArray[i, j].ObjectPosition = pos * _objSize;
			}
		}
	}

    /***************************************************************
    * update the position of a TerrainObject
    * 
    ****************************************************************/
    void updateObjectPosition(int i, int j, Vector3 pos) {
        _objArray[i, j].ObjectPosition = pos * _objSize;	
	}

    /***************************************************************
    * move a TerrainObject with after delay
    * 
    ****************************************************************/
    IEnumerator moveObject(int i, int j, Vector3 pos, float sec) {
		yield return new WaitForSeconds (sec);
        updateObjectPosition(i, j, pos);
        _objArray[i, j].setColor (new Color(0, 0.75F + (Random.value/4), 0));
	}

    /***************************************************************
    * update the edge points to match each other so they can map infinitely
    * 
    ****************************************************************/
    void normalizeBoundaries()
    {
        for (int i = 1; i < _arrayLength-1; i++)
        {
            //update X coords
            float avgX = (_positionArray[i, 0].y + _positionArray[i, _arrayLength - 1].y)/2;
            _positionArray[i, 0].y = avgX;
            _positionArray[i, _arrayLength - 1].y = avgX;

            //update Y coords
            float avgY = (_positionArray[0, i].y + _positionArray[_arrayLength - 1, i].y)/2;
            _positionArray[0, i].y = avgY;
            _positionArray[_arrayLength - 1, i].y = avgY;
        }
    }

    /***************************************************************
    * update vertext normals and draw debug lines
    * 
    ****************************************************************/
    void updateVertexNormals(GameObject meshObj)
    {

        Mesh mesh = meshObj.GetComponent<MeshFilter>().mesh;
        Vector3[] normals = mesh.normals;

        Color color = new Color(Random.value, Random.value, Random.value);

        //try setting all bounding vertices to vector3.up
        for (int x = 0; x < _meshLimit + 1; x++)
        {
            for (int y = 0; y < _meshLimit + 1; y++)
            {
                //only update normals bordering on other meshes
                if (x == 0 || y == 0 || x == _meshLimit || y == _meshLimit)
                {
                    int index = ((_meshLimit + 1) * x) + y;

                    //draw lines to represent normals
                    //Vector3 fixedVertex = Quaternion.Euler(0, 0, 180) * mesh.vertices[index];
                    //Vector3 fixedNormal = Quaternion.Euler(0, 0, 180) * mesh.normals[index];                    
                    //Debug.DrawLine(fixedVertex, fixedVertex + fixedNormal * 2, color, 100.0f);

                    //set intersecting normals to default value
                    normals[index] = new Vector3(0.0f, -1.0f, 0.0f);
                }
            }
        }

        //update normal array
        mesh.normals = normals;
    }


    /***************************************************************
    * update the vertice normals to fix lighting issues
    * 
    ****************************************************************/
    void updateVertexNormalsForAllMeshes()
    {
        for (int j = 0; j < _objRef.Count; j++)
        {
            updateVertexNormalWithNeighbours(_objRef[j]);
        }
    }


    /***************************************************************
    * update normals for vertex and neighbours
    * 
    ****************************************************************/
    void updateVertexNormalWithNeighbours(GameObject meshObj)
    {

        //Mesh mesh = meshObj.GetComponent<MeshFilter>().mesh;
        //Vector3[] normals = mesh.normals;       
        //normals = findNeighbouringVertices(meshObj, normals, x, y);       

        //nw point
        findNeighbouringVertices(meshObj, 0, _meshLimit);

        //se point
        findNeighbouringVertices(meshObj, _meshLimit, 0);

    }

    /***************************************************************
    * find all neighbouring vertices in other meshes
    * 
    ****************************************************************/
    void findNeighbouringVertices(GameObject mainObj, int x, int y)
    {
        Mesh mainMesh = mainObj.GetComponent<MeshFilter>().mesh;
        Vector3[] mainNormals = mainMesh.normals;

        int objCount = _objRef.Count;
        int mainObjIndex = _objRef.IndexOf(mainObj);
        int terrainLength = _arrayIndex/_meshLimit;

        //Debug.Log("mainObjIndex:" + mainObjIndex);
        //Debug.Log("mainMesh name: " + mainMesh.name);

        if (x == 0 && y == _meshLimit)
        {
            int northObjIndex = mainObjIndex + 1;
            //Debug.Log("northObjIndex: " + northObjIndex);

            if ((northObjIndex % terrainLength) == 0)
            {
                //Debug.Log("north most piece; returning");
                return;
            }

            GameObject northObj = _objRef[northObjIndex];
            Mesh northMesh = northObj.GetComponent<MeshFilter>().mesh;
            Vector3[] northNormals = northMesh.normals;
            float tempColor = 0.0f;

            for (int iter = x; iter < _meshLimit + 1; iter++)
            {
                tempColor += 0.1f;
                int mainIndex = ((_meshLimit + 1) * iter) + y;
                int northIndex = ((_meshLimit + 1) * iter);

                Vector3 newNormal = mainNormals[mainIndex] + northNormals[northIndex];
                newNormal.Normalize();

                mainNormals[mainIndex] = newNormal;// new Vector3(0.0f, -1.0f, 0.0f); 
                northNormals[northIndex] = newNormal;// new Vector3(0.0f, -1.0f, 0.0f); 

                mainMesh.normals = mainNormals;
                northMesh.normals = northNormals;

                //Vector3 fixedVertex = Quaternion.Euler(0, 0, 180) * mainMesh.vertices[mainIndex];
                //Vector3 fixedNormal = Quaternion.Euler(0, 0, 180) * mainMesh.normals[mainIndex];
                //Debug.DrawLine(fixedVertex, fixedVertex + fixedNormal * 2, new Color(1, tempColor, tempColor), 5.0f);

                //fixedVertex = Quaternion.Euler(0, 0, 180) * northMesh.vertices[northIndex];
                //fixedNormal = Quaternion.Euler(0, 0, 180) * northMesh.normals[northIndex];
                //Debug.DrawLine(fixedVertex, fixedVertex + fixedNormal * 2, new Color(tempColor, tempColor, 1), 8.0f);
            }
        }
        else if (x == _meshLimit && y == 0)
        {
            int westObjIndex = mainObjIndex + terrainLength;                      
            if (westObjIndex >= objCount)
            {
                //Debug.Log("west most piece; returning");
                return;
            }
            GameObject westObj = _objRef[westObjIndex];
            Mesh westMesh = westObj.GetComponent<MeshFilter>().mesh;
            //Debug.Log("westObjIndex: " + westObjIndex); 
            //Debug.Log("westMesh name: " + westMesh.name);
            Vector3[] westNormals = westMesh.normals;
            float tempColor = 0.0f;
            for (int iter = 0; iter < _meshLimit + 1; iter++)
            {
                tempColor += 0.1f;
                int mainIndex = ((_meshLimit + 1) * _meshLimit) + iter;
                int westIndex = iter;
                Vector3 newNormal = mainNormals[mainIndex] + westNormals[westIndex];
                newNormal.Normalize();

                mainNormals[mainIndex] = newNormal;// new Vector3(0.0f, -1.0f, 0.0f); 
                westNormals[westIndex] = newNormal;// new Vector3(0.0f, -1.0f, 0.0f); 

                mainMesh.normals = mainNormals;
                westMesh.normals = westNormals;

                //Vector3 fixedVertex = Quaternion.Euler(0, 0, 180) * mainMesh.vertices[mainIndex];
                //Vector3 fixedNormal = Quaternion.Euler(0, 0, 180) * mainMesh.normals[mainIndex];
                //Debug.DrawLine(fixedVertex, fixedVertex + fixedNormal * 2, new Color(1, tempColor, tempColor), 5.0f);

                //fixedVertex = Quaternion.Euler(0, 0, 180) * westMesh.vertices[westIndex];
                //fixedNormal = Quaternion.Euler(0, 0, 180) * westMesh.normals[westIndex];
                //Debug.DrawLine(fixedVertex, fixedVertex + fixedNormal * 2, new Color(tempColor, tempColor, 1), 8.0f);
            }
        }
        else
        {
            Debug.LogError("something went wrong");
        }
    }

    /***************************************************************
    * update the positional array using the Diamond Square Algorithim
    * 
    ****************************************************************/
    void updatePositionUsingDiamondSquare() {	
		int size = _arrayIndex;
		int length = _arrayLength;
		float roughness = _terrainRoughness;

		//set default corners
		_positionArray [0, 0] = Vector3.zero;
		_positionArray [size, 0] = new Vector3 (size, 0.0f, 0.0f);
		_positionArray [0, size] = new Vector3 (0.0f, 0.0f, size);
		_positionArray [size, size] = new Vector3 (size, 0.0f, size);

		split (size, roughness, length);
	}

    /***************************************************************
    * Diamond Square Algorithim recursive function 
    * 
    ****************************************************************/
    void split(int size, float roughness, int full) {
		//Debug.Log ("split() params size: " + size + " roughness: " + roughness + " full: " + full	);
		int half = size / 2;
		//float scale = roughness * _blockSize;
		float scale = roughness * size;

		if (half < 1) {
			return;
		}

		for (int y = half; y < full; y += size) {
			for (int x = half; x < full; x += size) {
                float rand = Random.value * scale * 2 - scale;
                square (x, y, half, rand, full);
			}
		}

		for (int y = 0; y < full; y += half) {
			for (int x = (y + half) % size; x < full; x += size) {
                float rand = Random.value * scale * 2 - scale;
                diamond (x, y, half, rand, full);
			}
		}

		split (half, roughness, full);
	}

    /***************************************************************
    * Diamond Square Algorithim recursive function 
    * 
    ****************************************************************/
    void diamond(int x, int y, int half, float offset, int full) {
		//Debug.Log ("diamond() params x: " + x + " y: " + y + " half: " + half + " offset: " + offset + " full: " + full);	
		float d1 = (y - half) >= 0 ? _positionArray [x, (y - half)].y : 0;
		float d2 = (x + half) < full ? _positionArray [(x + half), y].y : 0;
		float d3 = (y + half) < full ? _positionArray [x, (y + half)].y : 0;
		float d4 = ((x - half) >= 0) ? _positionArray [(x - half), y].y : 0;

		float average = ( d1 + d2 + d3 + d4 ) / 4;
		_positionArray [x, y] = new Vector3 (x, average + offset, y);
        //StartCoroutine(moveBlock (x, y, _positionArray [x, y], _count +=0.001F));
    }

    /***************************************************************
    * Diamond Square Algorithim recursive function 
    * 
    ****************************************************************/
    void square(int x, int y, int half, float offset, int full) {
		//Debug.Log ("square() params x: " + x + " y: " + y + " half: " + half + " offset: " + offset + " full: " + full);
		float s1 = ((x - half) >= 0 || (y - half) >= 0) ? _positionArray [(x - half), (y - half)].y : 0;
		float s2 = ((y - half) >= 0 || (x + half) < full) ? _positionArray [(x + half), (y - half)].y : 0;
		float s3 = ((x + half) < full || (y + half) < full) ? _positionArray [(x + half), (y + half)].y : 0;
		float s4 = ((x - half) >= 0 || (y + half) < full) ? _positionArray [(x - half), (y + half)].y : 0;

		float average = ( s1 + s2 + s3 + s4 ) / 4;
		_positionArray [x, y] = new Vector3 (x, average + offset, y);
        //StartCoroutine(moveBlock (x, y, _positionArray [x, y], _count +=0.001F));
    }


    /*************************************************************** 
     * Update the positional Array with Default Values
     * 
     ***************************************************************/
    void updatePositionUsingDefault()
    {
        for (int x = 0; x < _arrayLength; x++)
        {
            for (int y = 0; y < _arrayLength; y++)
            {
                _positionArray[x, y] = new Vector3(x, y, 0);
            }
        }
    }

    //	IEnumerator flashBlock(TerrainBlock block, float sec) {
    //		yield return new WaitForSeconds (sec);
    //		if (block.IsVisible) {
    //			block.setColor (Color.red);
    //		}
    //	}
}
