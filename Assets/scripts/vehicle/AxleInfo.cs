using UnityEngine;
using System.Collections;

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
