using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextChangeSwipe : MonoBehaviour {

    public GameObject TextOnSwipe;
    public GameObject TextOnClick;

	// Update is called once per frame
	void Update () {
        if (PlayerPrefs.GetInt("Swiping")==0) {
            TextOnClick.SetActive(true);
            TextOnSwipe.SetActive(false);
        } else {
            TextOnClick.SetActive(false);
            TextOnSwipe.SetActive(true);
        }
    }
}
