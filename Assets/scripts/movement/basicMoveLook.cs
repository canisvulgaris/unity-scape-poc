using UnityEngine;
using System.Collections;

public class BasicMoveLook : MonoBehaviour {

	public Vector3 _initialCameraPosition = new Vector3 (107.6f, 36.4f, -0.66f);
	public Vector3 _currentCameraPosition;

	public Quaternion _initialCameraRotation = new Quaternion (0.0f, -60.0f, 0.0f, 0.0f);
	public Quaternion _currentCameraRotation;

	public float _defaultLookSpeed = 5.0f;
	public float _defaultMoveSpeed = 1.0f;

	public float _freeLookSpeed = 5.0f;
	public float _freeMoveSpeed = 1.0f;
    public float _boostMultiplier = 3.0f;

	// Use this for initialization
	void Start () {
		transform.position = _initialCameraPosition;
		_currentCameraPosition = _initialCameraPosition;

		transform.rotation = _initialCameraRotation;
		_currentCameraRotation = _initialCameraRotation;

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        Cursor.lockState = CursorLockMode.Locked;


		//rotationY = Mathf.Clamp (rotationY, -90, 90);
		//add rotation limits
		//add position limits

		transform.localRotation = Quaternion.AngleAxis(_currentCameraRotation.x, Vector3.up);
		transform.localRotation *= Quaternion.AngleAxis(_currentCameraRotation.y, Vector3.left);

		//transform.position += transform.forward * _defaultMoveSpeed * Input.GetAxis("Vertical");
		transform.position = _currentCameraPosition;
		transform.position += transform.right * _defaultMoveSpeed * Input.GetAxis("Horizontal");
		transform.Translate(0, 0, _defaultMoveSpeed * Input.GetAxis("Vertical"), Space.World);
		_currentCameraPosition = transform.position;

//      rotationX += Input.GetAxis("Mouse X")*lookSpeed;
//		rotationY += Input.GetAxis("Mouse Y")*lookSpeed;
//		rotationY = Mathf.Clamp (rotationY, -90, 90);

//		transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
//		transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

//        if (Input.GetAxis("Boost") == 1)
//        {
//            transform.position += transform.forward * moveSpeed * boostMultiplier * Input.GetAxis("Vertical");
//            transform.position += transform.right * moveSpeed * boostMultiplier * Input.GetAxis("Horizontal");
//        }
//        else
//        {
//            transform.position += transform.forward * moveSpeed * Input.GetAxis("Vertical");
//            transform.position += transform.right * moveSpeed * Input.GetAxis("Horizontal");
//        }        
	}
}
