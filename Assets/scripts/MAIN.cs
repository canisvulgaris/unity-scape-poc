using UnityEngine;
using System.Collections;


public class MAIN : MonoBehaviour {

	// Use this for initialization
	void Start () {
		createTerrain ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void createTerrain() {
		//TerrainBlock block = new TerrainBlock (new Vector3(0, 0, 0), 0.1F);

		float blockSize = 0.1F;
		int gridSize = 5;

		for (float iterX = 0.0F; iterX < gridSize; iterX += blockSize) {
			for (float iterZ = 0.0F; iterZ < gridSize; iterZ += blockSize) {
				Vector3 pos = new Vector3 (iterX, 0.0F, iterZ);
				TerrainBlock block = new TerrainBlock (pos, blockSize);
			}
		}
	}
}
