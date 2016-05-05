using UnityEngine;
using System.Collections;


public class MAIN : MonoBehaviour {

	public int _gridExponential = 10;
	public float _terrainRoughness = 0.08F;
	public float _blockSize = 1;

	private int _arrayIndex;
	private int _arrayLength;
	private Vector3[,] _positionArray;
	private TerrainBlock[,] _blockArray;

	private float count = 0;

	// Use this for initialization
	void Start () {
		_arrayIndex = (int)Mathf.Pow (2, _gridExponential);
		_arrayLength = _arrayIndex + 1;

		_positionArray = new Vector3[_arrayLength, _arrayLength];
		_blockArray = new TerrainBlock[_arrayLength, _arrayLength];

		createFlatTerrain ();
		createDiamondTerrain ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void createFlatTerrain() {
		int length = _arrayLength;

		for (int i = 0; i < length; i++) {
			for (int j = 0; j < length; j++) {
				Vector3 pos = new Vector3(i, 0, j);
				TerrainBlock block = new TerrainBlock (pos * _blockSize, _blockSize);
				_blockArray [i, j] = block;
			}
		}
	}

	void updateTerrain(Vector3[,] posArray) {
		int length = _arrayLength;

		for (int i = 0; i < length; i++) {
			for (int j = 0; j < length; j++) {
				Vector3 pos = posArray[i, j];
				_blockArray[i, j].BlockPosition = pos * _blockSize;
			}
		}
	}

	void updateBlockPosition(int i, int j, Vector3 pos) {
		_blockArray[i, j].BlockPosition = pos * _blockSize;	
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
		_blockArray[i, j].setColor (new Color(0, 0.75F + (Random.value/4), 0));
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

		//updateTerrain (_positionArray);
	}

	void split(int size, float roughness, int full) {
		//Debug.Log ("split() params size: " + size + " roughness: " + roughness + " full: " + full	);
		int half = size / 2;
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
		StartCoroutine(moveBlock (x, y, _positionArray [x, y], count+=0.005F));
	}

	void square(int x, int y, int half, float offset, int full) {
		//Debug.Log ("square() params x: " + x + " y: " + y + " half: " + half + " offset: " + offset + " full: " + full);
		float s1 = ((x - half) >= 0 || (y - half) >= 0) ? _positionArray [(x - half), (y - half)].y : 0;
		float s2 = ((y - half) >= 0 || (x + half) < full) ? _positionArray [(x + half), (y - half)].y : 0;
		float s3 = ((x + half) < full || (y + half) < full) ? _positionArray [(x + half), (y + half)].y : 0;
		float s4 = ((x - half) >= 0 || (y + half) < full) ? _positionArray [(x - half), (y + half)].y : 0;

		float average = ( s1 + s2 + s3 + s4 ) / 4;
		_positionArray [x, y] = new Vector3 (x, average + offset, y);
		StartCoroutine(moveBlock (x, y, _positionArray [x, y], count+=0.005F));
	}

}
