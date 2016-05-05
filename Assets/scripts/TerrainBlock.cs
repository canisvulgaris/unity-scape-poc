using UnityEngine;
using System.Collections;

public class TerrainBlock : MonoBehaviour {

	private GameObject block;
	private float blockX = 1;
	private float blockY = 1;
	private float blockZ = 1;
	private float blockSize;
	private Vector3 blockPosition;
	private Material mat;

	public TerrainBlock(Vector3 pos, float size) {
		blockPosition = pos;
		blockSize = size;

		block = GameObject.CreatePrimitive (PrimitiveType.Cube);
		block.transform.localScale = new Vector3(blockX * size, blockY * size, blockZ * size);
		block.transform.position = pos;

		mat = new Material (Shader.Find("Standard"));
		mat.color = new Color (0.5F, 0.5F, 0.5F);

		block.GetComponent<Renderer> ().material = mat;
	}

	public void setColor(Color color) {
		mat.color = color;
	}

	public void setEnabled(bool set) {
		block.SetActive (set);
	}

	public bool IsVisible {
		get {
			return block.activeSelf;
		}
	}

	public float BlockSize {
		get {
			return blockSize;
		}
	}

	public Vector3 BlockPosition {
		get {
			return blockPosition;
		}
		set {
			blockPosition = value;
			block.transform.position = value;
		}
	}
}
