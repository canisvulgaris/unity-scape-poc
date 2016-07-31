using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class cabController : MonoBehaviour
{
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have

    public Color baseWheelColor;

    public Vector3 resetForce;
    public Vector3 resetTorque;

    public float brakeForce = 100.0f;

    private float colorInc;
    private bool forward = false;
    private bool reverse = false;
    //private bool acceleration = false;
    private bool braking = false;

    private GameObject terrainControllerObject;
    private TerrainController terrainController;

    void Start()
    {
        terrainControllerObject = GameObject.Find("TerrainController");
        terrainController = (TerrainController)terrainControllerObject.GetComponent<TerrainController>();
    }


    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider, GameObject wheelObject)
    {
        if (wheelObject == null)
        {
            Debug.LogError("wheelObject not found");
            return;
        }

        Transform visualWheel = wheelObject.transform;

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    public void FixedUpdate()
    {
        Vector3 currentVelocity = gameObject.GetComponent<Rigidbody>().velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(currentVelocity);

        foreach (AxleInfo axleInfo in axleInfos)
        {
            ApplyLocalPositionToVisuals(axleInfo.leftWheel, axleInfo.leftWheelObject);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel, axleInfo.rightWheelObject);
        }
    }
}