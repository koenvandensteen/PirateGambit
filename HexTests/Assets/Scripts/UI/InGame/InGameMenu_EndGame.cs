//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class InGameMenu_EndGame : MonoBehaviour {

    public GameObject WinStateScreen;
    public GameObject LoseStateScreen;

    void Awake() {
        WinStateScreen.SetActive(false);
        LoseStateScreen.SetActive(false);
    }

    public void GameOver(bool isWin, float score) {
        if (isWin) {
            LoseStateScreen.SetActive(false);
            WinStateScreen.SetActive(true);

            WinStateScreen.GetComponentInChildren<InGameMenu_HighscoreManager>().SetSurroundingScores(score);

        } else {
            WinStateScreen.SetActive(false);
            LoseStateScreen.SetActive(true);

            LoseStateScreen.GetComponentInChildren<InGameMenu_HighscoreManager>().SetTopscores();
        }
    }

    public void ShareLink()
    {
        FaceBookShare.FBShare.ShareFBLink();
    }

}
