using UnityEngine;
using System.Collections;


public class TerrainController : MonoBehaviour {

    public GameObject[] _objectsAvailable;
    public int _objSelection;
    private GameObject _objType;
    public GameObject _terrainParent;


    public int _gridExponential = 10;
	public float _terrainRoughness = 0.08F;
	public float _objSize = 1;

	private int _arrayIndex;
	private int _arrayLength;
	private Vector3[,] _positionArray;
	private TerrainObject[,] _objArray;
    private GameObject _objRef;

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

        if (_objType.name == "mesh")
        {
            //Debug.Log("found mesh object");
            //set up the _positionArray height map using the Diamond-Square algorithm             
            createMeshTerrain(_positionArray);
        }
        else
        {
            //Debug.Log("found terrain object");
            _objArray = new TerrainObject[_arrayLength, _arrayLength];
            createPositionalTerrainUsingObject(_positionArray);
        }
    }

    public void CalcTerrainHeightMap()
    {
        //Debug.Log("called CalcTerrainHeightMap");

        if (_positionArray == null || _arrayLength == null || _arrayIndex == null)
        {
            Debug.LogError("TerrainController - CalcTerrainHeightMap - params not set");
        }

        //create random terrain positions
        updatePositionUsingDiamondSquare();
    }

    /***************************************************************
     * creates a new mesh for the object (does not use terrainobject)
     * 
    ***************************************************************/
    void createMeshTerrain(Vector3[,] posArray) {
        int length = _arrayLength;

        Vector3[] meshVertices = new Vector3[ length * length ];
        Vector2[] meshUV = new Vector2[ length * length ];
        int[] meshTriangles = new int[ (3 * 2 * (length - 1) * (length - 1))];

        //create a new mesh 
        Mesh mesh = new Mesh();        

        //set up the mesh vertices array
        int vert_index = 0;
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
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
                meshTriangles[tri_index] =     (x     * length) + y;
                meshTriangles[tri_index + 1] = ((x+1) * length) + y;
                meshTriangles[tri_index + 2] = (x     * length) + y + 1;

                //add in second three vertices for mesh
                meshTriangles[tri_index + 3] = ((x+1) * length) + y;
                meshTriangles[tri_index + 4] = ((x+1) * length) + y + 1;
                meshTriangles[tri_index + 5] = (x     * length) + y + 1;

                //bump index by 6 for previous puts
                tri_index += 6;
            }
        }

        mesh.vertices = meshVertices;
        mesh.uv = meshUV;
        mesh.triangles = meshTriangles;

        mesh.RecalculateNormals();
        mesh.Optimize();

        //instantiate object with newly added mesh        
        _objRef = Instantiate(_objType, Vector3.zero, Quaternion.Euler(0, 0, 180)) as GameObject;
        _objRef.GetComponent<MeshFilter>().mesh = mesh;
        _objRef.AddComponent<MeshCollider>();
        _objRef.transform.parent = _terrainParent.transform;


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
				square (x, y, half, Random.value * scale * 2 - scale, full);
			}
		}

		for (int y = 0; y < full; y += half) {
			for (int x = (y + half) % size; x < full; x += size) {
				diamond (x, y, half, Random.value * scale * 2 - scale, full);
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
