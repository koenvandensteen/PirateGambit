//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainMenu_LoadSetting : MonoBehaviour {

    public enum ObjectType {
        Slider,
        DropDown,
        Toggle,
        InputField
    }

    public string SettingName;
    public ObjectType InputType;

    private MainMenu_Settings _settingsObject;

    // Use this for initialization
    void Awake() {
        LoadSettings();
    }

    public void LoadSettings() {
        _settingsObject = GetComponentInParent<MainMenu_Settings>();
        if (_settingsObject != null) {
            switch (InputType) {
                case ObjectType.Slider:
                    GetComponent<Slider>().value = _settingsObject.GetFloat(SettingName);
                    break;
                case ObjectType.DropDown:
                    GetComponent<Dropdown>().value = _settingsObject.GetInt(SettingName);
                    break;
                case ObjectType.Toggle:
                    GetComponent<Toggle>().isOn = _settingsObject.GetBool(SettingName);
                    break;
                case ObjectType.InputField:
                    GetComponent<InputField>().text = _settingsObject.GetString(SettingName);
                    break;
                default:
                    break;
            }
        }
    }

    public void StoreSetting(bool value) {
        if (_settingsObject != null)
            _settingsObject.StoreBool(SettingName, value);
    }

    public void StoreSetting(float value) {
        if (_settingsObject != null)
            _settingsObject.StoreFloat(SettingName, value);
    }

    public void StoreSetting(string value) {
        if (_settingsObject != null)
            _settingsObject.StoreString(SettingName, value);
    }

    public void StoreSetting(int value) {
        if (_settingsObject != null)
            _settingsObject.StoreInt(SettingName, value);
    }

    public void UpdateLocalization() {
        Localization.Instance.UpdateLocalization(GetComponent<Dropdown>().value);
    }
}