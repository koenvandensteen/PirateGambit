//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu_ScrollDropDown : MonoBehaviour {

    public Image TargetGraphic;
    public Sprite DefaultSprite;
    public Sprite ActiveSprite;
    public string SoundEffect;
    void Start() {
        if (TargetGraphic != null && ActiveSprite != null) {
            TargetGraphic.sprite = ActiveSprite;
        }

        AudioManager.Instance.PlaySound(SoundEffect);
    }
    void OnDestroy() {
        if (TargetGraphic != null && DefaultSprite != null) {
            TargetGraphic.sprite = DefaultSprite;
        }

    }
}
