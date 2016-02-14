//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class SoundRouter : MonoBehaviour {

    public void PlaySound(string soundName) {
        AudioManager.Instance.PlaySound(soundName);
    }
    public void PlaySound(string soundName, float volScale) {
        AudioManager.Instance.PlaySound(soundName, volScale);
    }
    public void PlayMusic(string soundName) {
        AudioManager.Instance.PlayMusic(soundName);
    }
    public void UpdateVolumes() {
        AudioManager.Instance.UpdateVolumes();
    }
}
