using UnityEngine;
using System.Collections;

public class BasicMoveLook : MonoBehaviour {

	public Vector3 _initialCameraPosition = new Vector3 (107.6f, 36.4f, -0.66f);
	public Vector3 _currentCameraPosition;

	public Quaternion _initialCameraRotation = new Quaternion (0.0f, -60.0f, 0.0f, 0.0f);
	public Quaternion _currentCameraRotation;

    public float _cameraPositionXDefault = 0.0f;
    public float _cameraPositionXDiff = 4.0f;
    public float _cameraPositionXMin;
    public float _cameraPositionXMax;

    public float _cameraPositionZDefault = 10.0f;
    public float _cameraPositionZDiff = 20.0f;
    public float _cameraPositionZMin;
    public float _cameraPositionZMax;

	public float _defaultLookSpeed = 5.0f;
	public float _defaultMoveSpeed = 1.0f;

	public float _freeLookSpeed = 5.0f;
	public float _freeMoveSpeed = 1.0f;
    public float _boostMultiplier = 3.0f;

    public float _cameraVehicleOffset = 20.0f;

    public GameObject vehicleObject;
    public float vehicleDropHeight = 50.0f;

    private bool lockOnCar = false;
    private int arrayIndex = 0;

    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    // Use this for initialization
    void Start () {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);


        transform.position = _initialCameraPosition;
		_currentCameraPosition = _initialCameraPosition;

		transform.rotation = _initialCameraRotation;
		_currentCameraRotation = _initialCameraRotation;

        setupDefaults();
    }

    void setupDefaults()
    {
        GameObject terrainControllerObject = GameObject.Find("TerrainController");
        TerrainController terrainController = (TerrainController)terrainControllerObject.GetComponent<TerrainController>();

        arrayIndex = terrainController.getArrayIndex();
        Debug.Log(arrayIndex);

        _cameraPositionXMin = _cameraPositionXDefault + _cameraPositionXDiff;
        _cameraPositionXMax = arrayIndex - _cameraPositionXDiff;
        _cameraPositionZMin = _cameraPositionZDefault - _cameraPositionZDiff;
        _cameraPositionZMax = arrayIndex - _cameraPositionZDiff;
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        //in case values have not been initialized
        if (arrayIndex == 0)
        {
            setupDefaults();
        }

        Cursor.lockState = CursorLockMode.Confined;

        //rotationY = Mathf.Clamp (rotationY, -90, 90);
        //add rotation limits
        //add position limits

        transform.localRotation = Quaternion.AngleAxis(_currentCameraRotation.x, Vector3.up);
		transform.localRotation *= Quaternion.AngleAxis(_currentCameraRotation.y, Vector3.left);

        if (Input.GetAxis("Vehicle Horizontal") != 0 || Input.GetAxis("Vehicle Vertical") != 0 || lockOnCar == true)
        {            
            if (vehicleObject != null)
            {
                transform.Translate(vehicleObject.transform.position.x - transform.position.x, 0, (vehicleObject.transform.position.z - transform.position.z) - _cameraVehicleOffset, Space.World);

            }
            lockOnCar = true;
        }

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            lockOnCar = false;
            //transform.position += transform.forward * _defaultMoveSpeed * Input.GetAxis("Vertical");
            transform.position = _currentCameraPosition;
            if (transform.position.x >= _cameraPositionXMin && Input.GetAxis("Horizontal") < 0)
            {
                transform.position += transform.right * _defaultMoveSpeed * Input.GetAxis("Horizontal");
            }
            else if (transform.position.x <= _cameraPositionXMax && Input.GetAxis("Horizontal") > 0)
            {
                transform.position += transform.right * _defaultMoveSpeed * Input.GetAxis("Horizontal");
            }

            if (transform.position.z >= _cameraPositionZMin && Input.GetAxis("Vertical") < 0)
            {
                transform.Translate(0, 0, _defaultMoveSpeed * Input.GetAxis("Vertical"), Space.World);
            }
            else if (transform.position.z <= _cameraPositionZMax && Input.GetAxis("Vertical") > 0)
            {
                transform.Translate(0, 0, _defaultMoveSpeed * Input.GetAxis("Vertical"), Space.World);
            }
        }

		_currentCameraPosition = transform.position;     
	}

    public void resetCarPosition()
    {
        float boundaryLimitX = 10.0f;
        float boundaryLimitZ = 20.0f;
        Vector3 validCarPosition = transform.position;
        validCarPosition.y = vehicleDropHeight;

        if ((_cameraPositionXMin + boundaryLimitX) > validCarPosition.x)
        {
            validCarPosition.x = _cameraPositionXMin + boundaryLimitX;
        }

        if ((_cameraPositionXMax - boundaryLimitX) < validCarPosition.x)
        {
            validCarPosition.x = _cameraPositionXMax - boundaryLimitX;
        }

        if ((_cameraPositionZMin + boundaryLimitZ) > validCarPosition.z)
        {
            validCarPosition.z = _cameraPositionZMin + boundaryLimitZ;
        }

        if ((_cameraPositionZMax - boundaryLimitZ) < validCarPosition.z)
        {
            validCarPosition.z = _cameraPositionZMax - boundaryLimitZ;
        }

        vehicleObject.transform.position = validCarPosition;
        vehicleObject.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
    }
}
