//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialData {
    public TutorialInfo Type;
    public List<GameObject> Objects;

    public TutorialData(TutorialInfo type, List<GameObject> objects) {
        Type = type;
        Objects = objects;
    }
}

public class InGameMenu_Tutorial : MonoBehaviour {

    [System.Serializable]
    public class TutorialObject {
        public TutorialInfo Tutorial;
        public GameObject Panel;
        public Color VisColor;
    }

    public TutorialObject[] Panels;
    private Dictionary<TutorialInfo, GameObject> _panelDictionary;
    private Dictionary<TutorialInfo, Color> _colorDictionary;
    private bool _isPanelOpen = false;
    private Queue<TutorialData> _queue = new Queue<TutorialData>();
    private List<GameObject> _visualizationObjects = new List<GameObject>();
    public GameObject ArrowPrefab;

    void OpenPanel() {
        TutorialData data;
        TutorialInfo panelType;
        if (_queue.Count != 0) {
            data = _queue.Dequeue();
            panelType = data.Type;
        } else {
            GameManager.ThisManager.ResumeGame();
            return;
        }

        if (data.Objects != null) {
            foreach (var item in data.Objects) {
                var arrow = Instantiate(ArrowPrefab, item.transform.position, Quaternion.identity) as GameObject;
                var renderer = arrow.GetComponentInChildren<Renderer>();
                if (renderer) {
                    renderer.material.SetColor("_Color", _colorDictionary[data.Type]);
                }
                _visualizationObjects.Add(arrow);
            }
        }

        GameObject panel;
        if (!_panelDictionary.TryGetValue(panelType, out panel)) {
            Debug.LogErrorFormat("InGameMenu_Tutorial::OpenPanel() >> Panel with name {0} is not in dictionary!", panelType.ToString());
            return;
        }

        foreach (var item in _panelDictionary) {
            //Close all other panels
            item.Value.SetActive(false);
        }

        panel.SetActive(true);
        _isPanelOpen = true;
        GameManager.ThisManager.PauseGame();
    }

    public void ClosePanel(string panelName) {

        foreach (var item in _visualizationObjects) {
            Destroy(item, 1.0f);
        }
        _visualizationObjects.Clear();

        TutorialInfo panelType = (TutorialInfo)TutorialInfo.Parse(typeof(TutorialInfo), panelName);
        GameObject panel;
        if (!_panelDictionary.TryGetValue(panelType, out panel)) {
            Debug.LogErrorFormat("InGameMenu_Tutorial::ClosePanel() >> Panel with name {0} is not in dictionary!", panelType.ToString());
            return;
        }

        panel.SetActive(false);
        _isPanelOpen = false;
        if (panelType == TutorialInfo.FirstMove || panelType == TutorialInfo.Intro) {
            OpenPanel();
        } else {
            Invoke("OpenPanel", 1.0f);
        }
    }

    void Awake() {
        _panelDictionary = new Dictionary<TutorialInfo, GameObject>(Panels.Length);
        _colorDictionary = new Dictionary<TutorialInfo, Color>(Panels.Length);
        foreach (var item in Panels) {
            _panelDictionary.Add(item.Tutorial, item.Panel);
            _colorDictionary.Add(item.Tutorial, item.VisColor);
            //Deactivate open panels ...
            item.Panel.SetActive(false);
        }

        GameManager.ThisManager.ShowTutorialScreenImplementationScreen += ActivateTutorial;
    }

    void OnValidate() {
        //When a value in the editor changes, update the dictionary
        _panelDictionary = new Dictionary<TutorialInfo, GameObject>(Panels.Length);
        _colorDictionary = new Dictionary<TutorialInfo, Color>(Panels.Length);
        foreach (var item in Panels) {
            _panelDictionary.Add(item.Tutorial, item.Panel);
            _colorDictionary.Add(item.Tutorial, item.VisColor);
        }
    }


    void ActivateTutorial(TutorialData tutorial) {
        _queue.Enqueue(tutorial);
        if (!_isPanelOpen) {
            OpenPanel();
        }
    }

    public void ClearQueue() {
        _queue.Clear();
    }

}
