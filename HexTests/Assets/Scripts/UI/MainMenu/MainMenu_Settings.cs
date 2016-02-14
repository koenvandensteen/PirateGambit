//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu_Settings : MonoBehaviour {

    public float GetFloat(string settingName) {
        return PlayerPrefs.GetFloat(settingName);
    }

    public void StoreFloat(string settingName, float value) {
        PlayerPrefs.SetFloat(settingName, value);
    }

    public bool GetBool(string settingName) {
        return (PlayerPrefs.GetInt(settingName) == 0) ? false : true;

    }

    public void StoreBool(string settingName, bool value) {
        PlayerPrefs.SetInt(settingName, value ? 1 : 0);
    }

    public int GetInt(string settingName) {
        return PlayerPrefs.GetInt(settingName);
    }

    public void StoreInt(string settingName, int value) {
        PlayerPrefs.SetInt(settingName, value);
    }

    public string GetString(string settingName) {
        return PlayerPrefs.GetString(settingName);
    }

    public void StoreString(string settingName, string value) {
        PlayerPrefs.SetString(settingName, value);
    }



}
