//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class InGameMenu_HighscoreManager : MonoBehaviour {

    public GameObject Container;
    public GameObject Template;
    public GameObject InputTemplate;
    public GameObject LoadingObject;

    private GameObject _input;
    private float _score;
    private int _rank;

    void Awake() {
        if (Container == null) {
            Container = transform.FindChild("Container").gameObject;
        }
    }

    void OnDestroy() {
        ParseObject_Singleton.Instance.OnlistReceivedImplementation = null;
    }

    public void SetTopscores() {
        if (LoadingObject != null)
            LoadingObject.SetActive(true);
        ParseObject_Singleton.Instance.RequestTop(10, ShowTopScores);
    }

    public void SetSurroundingScores(float score) {
        if (LoadingObject != null)
            LoadingObject.SetActive(true);
        _score = score;
        ParseObject_Singleton.Instance.RequestSurroundingScores(score, 20, ShowSurroundingScoresInput);
    }

    void ShowTopScores(List<ParseObject_Singleton.PlayerScore> scores) {
        if (LoadingObject != null)
            LoadingObject.SetActive(false);

        if (gameObject == null) {
            return;
        }

        foreach (var score in scores) {
            var scoreObject = Instantiate<GameObject>(Template);
            var transformComp = scoreObject.GetComponent<RectTransform>();
            transformComp.SetParent(Template.transform.parent);
            transformComp.localScale = Vector3.one;
            transformComp.localPosition = Vector3.zero;
            scoreObject.SetActive(true);

            if (score.Score == -1) {
                scoreObject.transform.FindChild("Rank").GetComponent<Text>().text = score.Rank.ToString();
                scoreObject.transform.FindChild("Name").GetComponent<Text>().text = "...";
                scoreObject.transform.FindChild("Score").GetComponent<Text>().text = string.Empty;
            } else {
                scoreObject.transform.FindChild("Rank").GetComponent<Text>().text = score.Rank.ToString();
                scoreObject.transform.FindChild("Name").GetComponent<Text>().text = score.Name.ToString();
                int centiSeconds = (int)((score.Score - Mathf.Floor(score.Score)) * 100);
                scoreObject.transform.FindChild("Score").GetComponent<Text>().text = string.Format("{0}:{1},{2}", Mathf.FloorToInt(score.Score) / 60, (Mathf.FloorToInt(score.Score) % 60).ToString("00"), centiSeconds.ToString("00"));
            }
        }
    }

    void ShowSurroundingScoresInput(List<ParseObject_Singleton.PlayerScore> scores) {
        if (LoadingObject != null)
            LoadingObject.SetActive(false);

        if (gameObject == null) {
            return;
        }

        int index = 0;

        foreach (var score in scores) {
            if (score.Name == string.Empty) {
                var scoreObject = Instantiate<GameObject>(InputTemplate);
                _input = scoreObject;
                var transformComp = scoreObject.GetComponent<RectTransform>();
                transformComp.SetParent(Template.transform.parent);
                transformComp.localScale = Vector3.one;
                transformComp.localPosition = Vector3.zero;
                scoreObject.SetActive(true);

                scoreObject.transform.FindChild("Rank").GetComponent<Text>().text = score.Rank.ToString();
                int centiSeconds = (int)((score.Score - Mathf.Floor(score.Score)) * 100);
                scoreObject.transform.FindChild("Score").GetComponent<Text>().text = string.Format("{0}:{1},{2}", Mathf.FloorToInt(score.Score) / 60, (Mathf.FloorToInt(score.Score) % 60).ToString("00"), centiSeconds.ToString("00"));

            } else {

                var scoreObject = Instantiate<GameObject>(Template);
                var transformComp = scoreObject.GetComponent<RectTransform>();
                transformComp.SetParent(Template.transform.parent);
                transformComp.localScale = Vector3.one;
                transformComp.localPosition = Vector3.zero;
                scoreObject.SetActive(true);

                if (score.Score == -1) {
                    scoreObject.transform.FindChild("Rank").GetComponent<Text>().text = score.Rank.ToString();
                    scoreObject.transform.FindChild("Name").GetComponent<Text>().text = "...";
                    scoreObject.transform.FindChild("Score").GetComponent<Text>().text = string.Empty;
                } else {
                    scoreObject.transform.FindChild("Rank").GetComponent<Text>().text = score.Rank.ToString();
                    scoreObject.transform.FindChild("Name").GetComponent<Text>().text = score.Name.ToString();
                    int centiSeconds = (int)((score.Score - Mathf.Floor(score.Score)) * 100);
                    scoreObject.transform.FindChild("Score").GetComponent<Text>().text = string.Format("{0}:{1},{2}", Mathf.FloorToInt(score.Score) / 60, (Mathf.FloorToInt(score.Score) % 60).ToString("00"), centiSeconds.ToString("00"));
                }
            }

            if (++index >= 10)
                break;
        }

    }

    public void AddHighscore(string name) {
        if (!IsNameValid(name))
            return;
        ParseObject_Singleton.Instance.SubmitScore(name, _score);


        var scoreObject = Instantiate<GameObject>(Template);
        var transformComp = scoreObject.GetComponent<RectTransform>();
        transformComp.SetParent(Template.transform.parent);
        transformComp.localScale = Vector3.one;
        transformComp.localPosition = Vector3.zero;
        transformComp.SetSiblingIndex(_input.transform.GetSiblingIndex());
        scoreObject.SetActive(true);



        //Copy values
        scoreObject.transform.FindChild("Rank").GetComponent<Text>().text = _input.transform.FindChild("Rank").GetComponent<Text>().text;
        scoreObject.transform.FindChild("Name").GetComponent<Text>().text = name;
        int centiSeconds = (int)((_score - Mathf.Floor(_score)) * 100);
        scoreObject.transform.FindChild("Score").GetComponent<Text>().text = string.Format("{0}:{1},{2}", Mathf.FloorToInt(_score) / 60, (Mathf.FloorToInt(_score) % 60).ToString("00"), centiSeconds.ToString("00"));

        _input.SetActive(false);
    }

    public void ClearHighscores() {
        for (int i = 0; i < Container.transform.childCount; i++) {
            GameObject child = Container.transform.GetChild(i).gameObject;
            if (child.Equals(Template) || child.Equals(InputTemplate)) {
                //Dont delete the templates
            } else {
                Destroy(child);
            }
        }
    }

    bool IsNameValid(string name) {
        return !name.Equals(string.Empty);
    }
}
