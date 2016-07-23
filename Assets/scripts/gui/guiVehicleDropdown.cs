using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class guiVehicleDropdown : MonoBehaviour {

    public Dropdown vehicleDropdown;
    public Text text;
    public List<GameObject> vehicleObjects;

    private String defaultText = "none";

    void Start()
    {
        vehicleDropdown.captionText.text = defaultText;
        vehicleDropdown.options.Clear();
        vehicleDropdown.options.Add(new Dropdown.OptionData() { text = defaultText });
        foreach (GameObject obj in vehicleObjects)
        {
            vehicleDropdown.options.Add(new Dropdown.OptionData() { text=obj.name});
        }
    }

    public void updateVehicleSelected()
    {
        //Debug.Log("selection: " + vehicleObjects[vehicleDropdown.value].name);

        foreach (GameObject obj in vehicleObjects)
        {
            obj.SetActive(false);
        }

        if (vehicleDropdown.value != 0)
        {
            //since first item is blank
            int dropIndex = vehicleDropdown.value - 1;

            //vehicleObjects[dropIndex].transform.position = new Vector3(30.0f, 20.0f, 40.0f);
            vehicleObjects[dropIndex].SetActive(true);
            Camera.main.GetComponent<BasicMoveLook>().vehicleObject = vehicleObjects[dropIndex];
            Camera.main.GetComponent<BasicMoveLook>().resetCarPosition();
        }
    }

}
