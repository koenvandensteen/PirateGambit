//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class DangerOverlay : MonoBehaviour {

    public Sprite[] Sprites;
    private Color _baseColor;

    void Awake() {
        GameManager.Instance.DangerLevelChangedImplementation += UpdateOverlay;
        _baseColor = GetComponent<Image>().color;
    }

    void UpdateOverlay(int value) {
        if (value == 0) {
            GetComponent<Image>().color = Color.clear;
            return;
        }
        GetComponent<Image>().color = _baseColor;

        int index = Mathf.Clamp(value - 1, 0, Sprites.Length);

        GetComponent<Image>().sprite = Sprites[index];

    }
}
