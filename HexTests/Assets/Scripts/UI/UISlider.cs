//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UISlider : MonoBehaviour {

    public enum SliderType {
        Treasure,
        Danger,
        Invulnerability
    }

    public SliderType Type = SliderType.Treasure;

    // Use this for initialization
    void Start() {
        switch (Type) {
            case SliderType.Treasure:
                GameManager.Instance.TreasureChangedImplementation += UpdateUserInterface;
                break;
            case SliderType.Danger:
                GameManager.Instance.DangerLevelChangedImplementation += UpdateUserInterface;
                break;
            case SliderType.Invulnerability:
                GameManager.Instance.RumlevelChangedImplementation += UpdateUserInterface;
                break;
            default:
                break;
        }
        GetComponent<Image>().fillAmount = 0;
    }

    // Update is called once per frame
    void Update() {

    }

    public void UpdateUserInterface(int newValue) {
        switch (Type) {
            case SliderType.Treasure:
                //Debug.LogFormat("Treasure Amount:\n Current: {0}, Total: {1}", newValue, GameManager.ThisManager.TotalTreasureAmount);

                if (GameManager.Instance.MaxTreasureAmmount > 0)
                    GetComponent<Image>().fillAmount = (float)newValue / GameManager.Instance.MaxTreasureAmmount;
                break;
            case SliderType.Danger:
               // Debug.LogFormat("Danger Level:\n Current: {0}", newValue);
                GetComponent<Image>().fillAmount = (float)newValue / 6.0f;
                break;
            case SliderType.Invulnerability:
                if (GameManager.Instance.MaxRumStack > 0)
                    GetComponent<Image>().fillAmount = (float)newValue / GameManager.Instance.MaxRumStack;
                break;
            default:
                break;
        }
    }
}
