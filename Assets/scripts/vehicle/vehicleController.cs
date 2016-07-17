using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public GameObject leftWheelObject;
    public GameObject rightWheelObject;
    public float leftWheelRotation;
    public float rightWheelRotation;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}

public class vehicleController : MonoBehaviour
{
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have

    public Color baseWheelColor;

    public Vector3 resetForce;
    public Vector3 resetTorque;

    private float colorInc;


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
        float motor = maxMotorTorque * Input.GetAxis("Vehicle Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Vehicle Horizontal");

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel, axleInfo.leftWheelObject);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel, axleInfo.rightWheelObject);

            //float leftWheelRotationCurrent = axleInfo.leftWheelObject.transform.eulerAngles.x;
            //float leftWheelRotationSpeed  = 0.0f;

            //if (axleInfo.leftWheelRotation > leftWheelRotationCurrent)
            //{
            //    leftWheelRotationSpeed = (360.0f + leftWheelRotationCurrent - axleInfo.leftWheelRotation) / Time.deltaTime;
            //}
            //else
            //{
            //    leftWheelRotationSpeed = (leftWheelRotationCurrent - axleInfo.leftWheelRotation) / Time.deltaTime;
            //}

            //axleInfo.leftWheelRotation = leftWheelRotationCurrent;

            //float rightWheelRotationCurrent = axleInfo.rightWheelObject.transform.eulerAngles.x;
            //float rightWheelRotationSpeed = 0.0f;

            //if (axleInfo.rightWheelRotation > rightWheelRotationCurrent)
            //{
            //    rightWheelRotationSpeed = (360.0f + rightWheelRotationCurrent - axleInfo.rightWheelRotation) / Time.deltaTime;
            //}
            //else
            //{
            //    rightWheelRotationSpeed = (rightWheelRotationCurrent - axleInfo.rightWheelRotation) / Time.deltaTime;
            //}

            //axleInfo.rightWheelRotation = rightWheelRotationCurrent;

            ////Debug.Log("leftWheelRotationSpeed: " + leftWheelRotationSpeed + " - rightWheelRotationSpeed: " + rightWheelRotationSpeed);
            //colorInc = (leftWheelRotationSpeed / 360.0f);
            //Debug.Log("left colorInc: " + colorInc);
            //axleInfo.leftWheelObject.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(baseWheelColor.r + colorInc, baseWheelColor.g + colorInc, baseWheelColor.b + colorInc);

            //colorInc = (rightWheelRotationSpeed / 360.0f);
            //Debug.Log("right colorInc: " + colorInc);
            //axleInfo.rightWheelObject.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(baseWheelColor.r + colorInc, baseWheelColor.g + colorInc, baseWheelColor.b + colorInc);
        }

        if (Input.GetButton("Reset"))
        {
            transform.GetComponent<Rigidbody>().AddForce(resetForce, ForceMode.Impulse);
            transform.GetComponent<Rigidbody>().AddTorque(resetTorque, ForceMode.Impulse);
        }


    }
}