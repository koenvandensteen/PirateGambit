//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class Localization_InitLanguage : MonoBehaviour {

    public enum DataType {
        Int,
        Float,
        String,
        Boolean
    }

    [System.Serializable]
    public class InitializationSetting {
        public string Setting;
        public DataType Type;
        public string InitialValue;
    }

    public InitializationSetting[] Settings;

    void Awake() {
        foreach (var item in Settings) {
            if (PlayerPrefs.HasKey(item.Setting)) {
                //Do nothing if the key is already set
            } else {
                switch (item.Type) {
                    case DataType.Int:
                        PlayerPrefs.SetInt(item.Setting, int.Parse(item.InitialValue));
                        break;
                    case DataType.Float:
                        PlayerPrefs.SetFloat(item.Setting, float.Parse(item.InitialValue));
                        break;
                    case DataType.String:
                        PlayerPrefs.SetString(item.Setting, item.InitialValue);
                        break;
                    case DataType.Boolean:
                        PlayerPrefs.SetInt(item.Setting, (bool.Parse(item.InitialValue)) ? 1 : 0);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
