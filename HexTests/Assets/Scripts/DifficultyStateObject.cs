//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Reflection;
using UnityEngine.SceneManagement;


public class DifficultyStateObject : MonoBehaviour {

    public enum DifficultyState {
        Easy = 0,
        Medium = 1,
        Hard = 2
    }

    public static DifficultyState CurDifficultyState;
    public static DifficultyStateObject Reference;
    // Use this for initialization
    void Awake() {
        Reference = GetComponent<DifficultyStateObject>();
    }

    void Start() {
        CurDifficultyState = DifficultyState.Easy;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update() {

    }

    public void ShowDifficultyOptions() {

    }

    public void SetDifficulty(int difficulty) {
        CurDifficultyState = (DifficultyState)difficulty;
        SceneManager.LoadScene("NormalMode");
    }

    static public string GetDifficultyName() {
        string returnVal = string.Empty;

        switch (CurDifficultyState) {
            case DifficultyState.Easy:
                returnVal = "Easy";
                break;
            case DifficultyState.Medium:
                returnVal = "Medium";
                break;
            case DifficultyState.Hard:
                returnVal = "Hard";
                break;
            default:
                break;
        }
        return returnVal;
    }
    static public string GetDifficultyName(int difficulty) {
        string returnVal = string.Empty;

        switch (CurDifficultyState) {
            case DifficultyState.Easy:
                returnVal = "Easy";
                break;
            case DifficultyState.Medium:
                returnVal = "Medium";
                break;
            case DifficultyState.Hard:
                returnVal = "Hard";
                break;
            default:
                break;
        }
        return returnVal;
    }
}
