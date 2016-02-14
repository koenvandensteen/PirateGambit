//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
//using UnityEditor;

public class MainMenu_Main : MonoBehaviour {

    public GameObject MainMenuPanel;
    public GameObject DefaultButton;

    [System.Serializable]
    public class MenuObject {
        public string Name = string.Empty;
        public GameObject Panel;
    }

    public MenuObject[] Panels;
    private Dictionary<string, GameObject> _panelDictionary;
    private string _openPanel = string.Empty;

    void Awake() {
        _panelDictionary = new Dictionary<string, GameObject>(Panels.Length);
        foreach (var item in Panels) {
            _panelDictionary.Add(item.Name, item.Panel);
            //Deactivate open panels ...
            item.Panel.SetActive(false);
        }

        MainMenuPanel.SetActive(true);
    }

    void Start() {
        // DontDestroyOnLoad(transform.GetComponentInParent<Camera>());    
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ClosePanel(_openPanel);
        }
        if (Input.GetKeyDown(KeyCode.P)) {
            if (_panelDictionary.ContainsKey("Play"))
                OpenPanel("Play");
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            if (_panelDictionary.ContainsKey("Settings"))
                OpenPanel("Settings");
        }
    }

    void OnValidate() {
        //When a value in the editor changes, update the dictionary
        _panelDictionary = new Dictionary<string, GameObject>(Panels.Length);
        foreach (var item in Panels) {
            _panelDictionary.Add(item.Name, item.Panel);
        }
    }

    public void OpenPanel(string panelName) {
        GameObject panel;
        if (!_panelDictionary.TryGetValue(panelName, out panel)) {
            QuitGame();
            return;
        }

        foreach (var item in _panelDictionary) {
            //Close all other panels
            item.Value.SetActive(false);
        }

        panel.SetActive(true);
        _openPanel = panelName;
    }

    public void ClosePanel(string panelName) {
        GameObject panel;
        if (!_panelDictionary.TryGetValue(panelName, out panel)) {
            QuitGame();
            return;
        }

        panel.SetActive(false);
        _openPanel = string.Empty;

        EventSystem.current.SetSelectedGameObject(DefaultButton);
    }

    public void QuitGame() {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor) {
            //UnityEditor.EditorApplication.isPlaying = false;
        } else {
            Application.Quit();
        }
    }
}
