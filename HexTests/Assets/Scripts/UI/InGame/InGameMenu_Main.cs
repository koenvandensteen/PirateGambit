//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InGameMenu_Main : MonoBehaviour {

    [System.Serializable]
    public class MenuObject {
        public string Name = string.Empty;
        public GameObject Panel;
    }

    public MenuObject[] Panels;
    private Dictionary<string, GameObject> _panelDictionary;
    private string _openPanel = string.Empty;

    public string EndGamePanelName = "EndGame";

    void Awake() {
        _panelDictionary = new Dictionary<string, GameObject>(Panels.Length);
        foreach (var item in Panels) {
            _panelDictionary.Add(item.Name, item.Panel);
            //Deactivate open panels ...
            item.Panel.SetActive(false);
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
            Debug.LogErrorFormat("InGameMenu_Main::OpenPanel(string) >> Panel with name {0} is not in dictionary!", panelName);
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
            Debug.LogErrorFormat("InGameMenu_Main::OpenPanel(string) >> Panel with name {0} is not in dictionary!", panelName);
            return;
        }

        panel.SetActive(false);
        _openPanel = string.Empty;

    }

    public void GameOver(bool isWin, float score) {
        OpenPanel(EndGamePanelName);
        var endGameScript = _panelDictionary[EndGamePanelName].GetComponent<InGameMenu_EndGame>();
        endGameScript.GameOver(isWin, score);
    }

    public void LoadLevel(int difficulty) {
        DifficultyStateObject.Reference.SetDifficulty(difficulty);
    }

    public void PlaySound(string fileName) {
        AudioManager.Instance.PlaySound(fileName);
    }

    public void ExitToMenu() {

        AudioManager.Instance.StopCurrentAmbientSfx();

        Application.LoadLevel(0);
    }

}
