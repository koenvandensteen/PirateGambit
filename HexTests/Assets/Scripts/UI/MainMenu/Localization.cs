//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class Localization : MonoBehaviour {

    public TextAsset SourceFile;

    private Dictionary<string, Dictionary<string, string>> _localizationDictionary;

    public string CurrentLanguage = "English";

    public static Localization Instance { get; private set; }

    // Use this for initialization
    void Awake() {
        if (Instance != null && Instance != this) {
            //destroy other instances
            Destroy(gameObject);
            return;
        }

        //Singleton instance
        Instance = this;

        //on't destroy between scenes
        DontDestroyOnLoad(gameObject);

        if (SourceFile == null) {
            this.enabled = false;
            return;
        }
        _localizationDictionary = new Dictionary<string, Dictionary<string, string>>();

        UpdateDictionary();

        //new List<MainMenu_Settings>(Resources.FindObjectsOfTypeAll<MainMenu_Settings>()).ForEach(i => i.gameObject.SetActive(true));

        var dropdownObject = new List<Dropdown>(Resources.FindObjectsOfTypeAll<Dropdown>()).Find(i => i.name == "LanguageSelect");
        if (dropdownObject == null) {
            Debug.LogError("Localization::UpdateLocalization >> Make sure the languageSelect dropdown is called \"LanguageSelect\"");
            CurrentLanguage = "English";
        } else {
            dropdownObject.options.Clear();
            foreach (var key in _localizationDictionary.Keys) {
                Dropdown.OptionData data = new Dropdown.OptionData(key);
                dropdownObject.options.Add(data);
            }

            CurrentLanguage = dropdownObject.options[PlayerPrefs.GetInt("Language")].text;
        }
        dropdownObject.value = PlayerPrefs.GetInt("Language");

//        new List<MainMenu_Settings>(Resources.FindObjectsOfTypeAll<MainMenu_Settings>()).ForEach(i => i.gameObject.SetActive(false));

        UpdateLocalization();
    }

    void OnLevelWasLoaded() {
        _localizationDictionary = new Dictionary<string, Dictionary<string, string>>();

        UpdateDictionary();
        var dropdownObject = new List<Dropdown>(Resources.FindObjectsOfTypeAll<Dropdown>()).Find(i => i.name == "LanguageSelect");
        if (dropdownObject == null) {
            Debug.LogError("Localization::UpdateLocalization >> Make sure the languageSelect dropdown is called \"LanguageSelect\"");
            CurrentLanguage = "English";
        } else {
            dropdownObject.options.Clear();
            foreach (var key in _localizationDictionary.Keys) {
                Dropdown.OptionData data = new Dropdown.OptionData(key);
                dropdownObject.options.Add(data);
            }

            CurrentLanguage = dropdownObject.options[PlayerPrefs.GetInt("Language")].text;
        }
        dropdownObject.value = PlayerPrefs.GetInt("Language");

        UpdateLocalization();
    }
    public void UpdateLocalization(int index) {
        var dropdownObject = new List<Dropdown>(Resources.FindObjectsOfTypeAll<Dropdown>()).Find(i => i.name == "LanguageSelect");
        if (dropdownObject == null) {
            Debug.LogError("Localization::UpdateLocalization >> Make sure the languageSelect dropdown is called \"LanguageSelect\"");
            return;
        }

        CurrentLanguage = dropdownObject.options[index].text;
        UpdateLocalization();
    }

    void UpdateLocalization() {
        List<Text> localizationItems = new List<Text>(Resources.FindObjectsOfTypeAll<Text>());
        localizationItems.RemoveAll(i => i.name.Length < 4);
        localizationItems.RemoveAll(i => !i.name.Substring(0, 4).Equals("loc_"));
        foreach (var item in localizationItems) {
            string itemName = item.name.Substring(4);
            string translation = string.Empty;
            if (_localizationDictionary[CurrentLanguage].TryGetValue(itemName, out translation)) {
                item.text = translation;
            } else if (_localizationDictionary["English"].TryGetValue(itemName, out translation)) {
                item.text = translation;

            } 
        }
    }

    private void UpdateDictionary() {

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(SourceFile.text);

        //Define languages to use in dictionary
        XmlNodeList languageList = xmlDoc.GetElementsByTagName("languageDefinition");

        foreach (XmlNode item in languageList) {
            _localizationDictionary.Add(item.InnerText, new Dictionary<string, string>());
        }


        XmlNodeList dictionaryList = xmlDoc.GetElementsByTagName("dictionary");
        foreach (XmlNode dictionary in dictionaryList) {
            XmlNodeList dictionaryItems = dictionary.ChildNodes;
            foreach (XmlNode dictionaryItem in dictionaryItems) {
                string sItem = dictionaryItem.Name;
                XmlNodeList translations = dictionaryItem.ChildNodes;
                foreach (XmlNode translation in translations) {
                    _localizationDictionary[translation.Name].Add(sItem, translation.InnerText);
                }

            }
        }
    }
}
