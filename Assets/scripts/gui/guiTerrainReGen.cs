using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class guiTerrainReGen : MonoBehaviour {

    public InputField tRoughText;
    public InputField tWaveText;
    public InputField tSizeText;

    private GameObject terrainControllerObject;
    private TerrainController terrainController;

    // Use this for initialization
    void Start () {
        terrainControllerObject = GameObject.Find("TerrainController");
        terrainController = (TerrainController)terrainControllerObject.GetComponent<TerrainController>();
        
        tRoughText.GetComponent<InputField>().text = terrainController._terrainRoughness.ToString();
        tWaveText.GetComponent<InputField>().text = terrainController._terrainWave.ToString();
        tSizeText.GetComponent<InputField>().text = terrainController._gridExponential.ToString();
    }
	
	// Update is called once per frame
	public void regenerateTerrain () {
        terrainController._terrainRoughness = float.Parse(tRoughText.GetComponent<InputField>().text);
        terrainController._terrainWave = float.Parse(tWaveText.GetComponent<InputField>().text);
        terrainController._gridExponential = int.Parse(tSizeText.GetComponent<InputField>().text);

        terrainController.RefreshTerrain();
    }
}
