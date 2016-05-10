using UnityEngine;
using System.Collections;


public class Main : MonoBehaviour {

    public GameObject _objType;
    public int _gridExponential = 10;
	public float _terrainRoughness = 0.08F;
	public float _objSize = 1;

	private int _arrayIndex;
	private int _arrayLength;
	private Vector3[,] _positionArray;
	private TerrainObject[,] _objArray;

	//private float _count = 0;

    private System.DateTime _startTime = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);

    // Use this for initialization
    void Start () {

        if (_objType == null)
        {
            Debug.LogError("Main - _objType not set");
        }

		_arrayIndex = (int)Mathf.Pow (2, _gridExponential);
		_arrayLength = _arrayIndex + 1;

		_positionArray = new Vector3[_arrayLength, _arrayLength];
		

        //for (int x = 0; x < _arrayLength; x++)
        //{
        //    for (int y = 0; y < _arrayLength; y++)
        //    {
        //        _positionArray[x, y] = new Vector3(x, y, 0);
        //    }
        //}

		createDiamondTerrain ();
        Debug.Log("diamond complete. " + (System.DateTime.UtcNow - _startTime).Seconds);

        //_objArray = new TerrainObject[_arrayLength, _arrayLength];
        //updateObjectTerrain (_positionArray);
        //createObjectTerrain (_positionArray);
        createMeshTerrain(_positionArray);
        Debug.Log("mesh complete. " + (System.DateTime.UtcNow - _startTime).Seconds);
    }

    // Update is called once per frame
    //void Update () {	
	//}


    void createMeshTerrain(Vector3[,] posArray) {

        int length = _arrayLength;

        Vector3[] newVertices = new Vector3[ length * length ];
        Vector2[] newUV = new Vector2[ length * length ];
        int[] newTriangles = new int[ (3 * 2 * (length - 1) * (length - 1))];

        Mesh mesh = new Mesh();
        _objType.GetComponent<MeshFilter>().mesh = mesh;

        //set vertices
        int k = 0;
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                newVertices[k] = _positionArray[i, j];
                //Debug.Log("newVertices : k : " + k + " - " + newVertices[k]);
                k++;
            }
        }

        //set uv
        int m = 0;
        for (int n = 0; n < length; n++)
        {
            for (int o = 0; o < length; o++)
            {
                newUV[m] = new Vector2(n, o);
                m++;
            }
        }

        int z = 0;
        for (int x = 0; x < length - 1; x++)
        {
            for (int y = 0; y < length - 1; y++)
            {
                newTriangles[z] =     (x     * length) + y;
                newTriangles[z + 1] = ((x+1) * length) + y;
                newTriangles[z + 2] = (x     * length) + y + 1;

                newTriangles[z + 3] = ((x+1) * length) + y;
                newTriangles[z + 4] = ((x+1) * length) + y + 1;
                newTriangles[z + 5] = (x     * length) + y + 1;


                //Debug.Log("count: z : " + z);
                //Debug.Log("tri: " + newTriangles[z] + ", " + newTriangles[z + 1] + ", " + newTriangles[z + 2] + " -  " + newTriangles[z + 3] + ", " + newTriangles[z + 4] + ", " + newTriangles[z + 5]);
                //Debug.Log("vert: " + newVertices[newTriangles[z]] + ", " + newVertices[newTriangles[z + 1]] + ", " + newVertices[newTriangles[z + 2]] + " -  " + newVertices[newTriangles[z + 3]] + ", " + newVertices[newTriangles[z + 4]] + ", " + newVertices[newTriangles[z + 5]]);

                z += 6;
            }
        }

        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;

        mesh.RecalculateNormals();
        //mesh.Optimize();

        Instantiate(_objType, Vector3.zero, Quaternion.Euler(0, 0, 180));
    }
    
	void createFlatObjectTerrain() {
		int length = _arrayLength;

		for (int i = 0; i < length; i++) {
			for (int j = 0; j < length; j++) {
				Vector3 pos = new Vector3(i, 0, j);
                TerrainObject obj = new TerrainObject(_objType, pos * _objSize, _objSize);
                _objArray[i, j] = obj;
			}
		}
	}

	void createObjectTerrain(Vector3[,] posArray) {
		int length = _arrayLength;

		for (int i = 0; i < length; i++) {
			for (int j = 0; j < length; j++) {
				Vector3 pos = posArray [i, j];
                TerrainObject obj = new TerrainObject(_objType, pos * _objSize, _objSize);
                _objArray[i, j] = obj;
            }
		}
	}

	void updateObjectTerrain(Vector3[,] posArray) {
		int length = _arrayLength;

		for (int i = 0; i < length; i++) {
			for (int j = 0; j < length; j++) {
				Vector3 pos = posArray[i, j];
                _objArray[i, j].ObjectPosition = pos * _objSize;
			}
		}
	}

	void updateBlockPosition(int i, int j, Vector3 pos) {
        _objArray[i, j].ObjectPosition = pos * _objSize;	
	}

//	IEnumerator flashBlock(TerrainBlock block, float sec) {
//		yield return new WaitForSeconds (sec);
//		if (block.IsVisible) {
//			block.setColor (Color.red);
//		}
//	}

	IEnumerator moveBlock(int i, int j, Vector3 pos, float sec) {
		yield return new WaitForSeconds (sec);
		updateBlockPosition (i, j, pos);
        _objArray[i, j].setColor (new Color(0, 0.75F + (Random.value/4), 0));
	}

	void createDiamondTerrain() {
		
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
}
