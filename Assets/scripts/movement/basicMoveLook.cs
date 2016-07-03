using UnityEngine;
using System.Collections;

public class BasicMoveLook : MonoBehaviour {

	public float lookSpeed = 15.0f;
	public float moveSpeed = 15.0f;
    public float boostMultiplier = 3.0f;

    public float  rotationX = 0.0f;
	public float  rotationY = 0.0f;

	// Use this for initialization
	void Start () {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        Cursor.lockState = CursorLockMode.Locked;

        rotationX += Input.GetAxis("Mouse X")*lookSpeed;
		rotationY += Input.GetAxis("Mouse Y")*lookSpeed;
		rotationY = Mathf.Clamp (rotationY, -90, 90);

		transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
		transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

        if (Input.GetAxis("Boost") == 1)
        {
            transform.position += transform.forward * moveSpeed * boostMultiplier * Input.GetAxis("Vertical");
            transform.position += transform.right * moveSpeed * boostMultiplier * Input.GetAxis("Horizontal");
        }
        else
        {
            transform.position += transform.forward * moveSpeed * Input.GetAxis("Vertical");
            transform.position += transform.right * moveSpeed * Input.GetAxis("Horizontal");
        }        
	}
}
