//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIRumButton : MonoBehaviour {

    // Use this for initialization
    void Start() {
        GameManager.Instance.RumlevelChangedImplementation += level => {
            //Debug.LogFormat("Immune Button: Rum level: {0}", level);
            if (level == 0) {
                GetComponent<Button>().interactable = false;
            } else {
                GetComponent<Button>().interactable = true;
            }
        };
    }
}
