using UnityEngine;
using System.Collections;

public class basicMoveLook : MonoBehaviour {

	public float lookSpeed = 15.0F;
	public float moveSpeed = 15.0F;

	public float  rotationX = 0.0F;
	public float  rotationY = 0.0F;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		rotationX += Input.GetAxis("Mouse X")*lookSpeed;
		rotationY += Input.GetAxis("Mouse Y")*lookSpeed;
		rotationY = Mathf.Clamp (rotationY, -90, 90);

		transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
		transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

		transform.position += transform.forward*moveSpeed*Input.GetAxis("Vertical");
		transform.position += transform.right*moveSpeed*Input.GetAxis("Horizontal");
	}
}
