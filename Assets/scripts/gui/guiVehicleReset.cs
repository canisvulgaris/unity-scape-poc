using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class guiVehicleReset : MonoBehaviour {
    
    public void resetVehiclePosition()
    {
        //Debug.Log("reset clicked");
        Camera.main.GetComponent<BasicMoveLook>().resetCarPosition();
    }
}
