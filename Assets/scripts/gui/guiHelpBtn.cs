using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class guiHelpBtn : MonoBehaviour {

    public Button helpBtn;
    public Text helpText;

	// Use this for initialization
	void Start () {
        helpText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    public void ToggleHelpText() {
        if (helpText.gameObject.activeSelf) {
            helpText.gameObject.SetActive(false);
        }
        else
        {
            helpText.gameObject.SetActive(true);
        }
    }
}
