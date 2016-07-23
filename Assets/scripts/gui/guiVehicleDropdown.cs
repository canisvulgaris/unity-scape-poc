using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class guiVehicleDropdown : MonoBehaviour {

    public Dropdown vehicleDropdown;
    public Text text;
    public List<GameObject> vehicleObjects;

    void Start()
    {
        //vehicleDropdown.options.Clear();
        foreach (GameObject obj in vehicleObjects)
        {
            vehicleDropdown.options.Add(new Dropdown.OptionData() { text=obj.name});
        }
        vehicleDropdown.value = 0;
    }

    public void updateVehicleSelected()
    {
        //text.text = vehicleObjects[vehicleDropdown.value].name;
        Debug.Log("selection: " + vehicleObjects[vehicleDropdown.value].name);

        foreach (GameObject obj in vehicleObjects)
        {
            obj.SetActive(false);
        }

        vehicleObjects[vehicleDropdown.value].transform.position = new Vector3(30.0f, 20.0f, 40.0f);
        vehicleObjects[vehicleDropdown.value].SetActive(true);
        Camera.main.GetComponent<BasicMoveLook>().vehicleObject = vehicleObjects[vehicleDropdown.value];
    }

}
