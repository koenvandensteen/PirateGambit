//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
//#define AUTOSELECTPLAY 

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Parse;
using System.Threading;
using System.Threading.Tasks;



public class MainMenu_Play : MonoBehaviour {

    [System.Serializable]
    public class DifficultyObject {
        public string Name = string.Empty;
        public GameObject Panel;
        [HideInInspector]
        public List<PlayerScore> HighScore = new List<PlayerScore>();
        [HideInInspector]
        public Task LoadScore;
        [HideInInspector]
        public GameObject HighScoreTemplate;

        public void UpdateHighscores() {
            if (HighScore != null)
                HighScore.Clear();

            //Get top 10 from server
            var query = ParseObject.GetQuery("HighScores" + Name).OrderBy("score").Limit(SCORE_COUNT + 1);
            int counter = 0;

            LoadScore = query.FindAsync().ContinueWith(t => {
                IEnumerable<ParseObject> results = t.Result;
                foreach (var obj in results) {

                    var score = obj.Get<float>("score");
                    var name = obj.Get<string>("playerName");
                    counter++;
                    var newScore = new PlayerScore {
                        Rank = counter,
                        Name = name,
                        Score = score
                    };
                    HighScore.Add(newScore);
                }

                if (HighScore.Count <= SCORE_COUNT) {
                    for (int i = HighScore.Count; i <= SCORE_COUNT; i++) {
                        var dummyPlayer = new PlayerScore() { Rank = ++counter, Name = string.Empty, Score = -1 };
                        HighScore.Add(dummyPlayer);
                    }
                }
            });
        }

        public void FillHighscores() {
            GameObject highScoreObject = Panel.transform.FindChild("Highscores").FindChild("Container").gameObject;
            if (highScoreObject == null) {
                return;
            }

            ClearHighscores();
            Panel.transform.FindChild("Highscores").FindChild("Loading").gameObject.SetActive(false);

            int counter = 0;

            foreach (var score in HighScore) {
                if (++counter > SCORE_COUNT)
                    break;

                var highscoreItem = Instantiate(HighScoreTemplate, Vector3.zero, Quaternion.identity) as GameObject;
                highscoreItem.transform.SetParent(highScoreObject.transform);
                highscoreItem.name = string.Format("Highscore_{0}", counter);
                var transformCompponent = highscoreItem.GetComponent<RectTransform>();
                transformCompponent.localScale = Vector3.one;
                transformCompponent.localPosition = Vector3.zero;

                if (score.Score == -1) {
                    highscoreItem.transform.FindChild("Rank").GetComponent<Text>().text = score.Rank.ToString();
                    highscoreItem.transform.FindChild("Name").GetComponent<Text>().text = "...";
                    highscoreItem.transform.FindChild("Score").GetComponent<Text>().text = string.Empty;
                } else {
                    highscoreItem.transform.FindChild("Rank").GetComponent<Text>().text = score.Rank.ToString();
                    highscoreItem.transform.FindChild("Name").GetComponent<Text>().text = score.Name.ToString();
                    int centiSeconds = (int)((score.Score - Mathf.Floor(score.Score)) * 100);
                    highscoreItem.transform.FindChild("Score").GetComponent<Text>().text = string.Format("{0}:{1},{2}", Mathf.FloorToInt(score.Score) / 60, (Mathf.FloorToInt(score.Score )% 60).ToString("00"), centiSeconds.ToString("00"));
                }
            }

        }

        public void ClearHighscores() {
            GameObject highScoreObject = Panel.transform.FindChild("Highscores").FindChild("Container").gameObject;
            if (highScoreObject == null) {
                return;
            }

            for (int i = 0; i < highScoreObject.transform.childCount; i++) {
                Destroy(highScoreObject.transform.GetChild(i).gameObject);
            }
        }
    }

    public DifficultyObject[] Panels;
    private int _currentPanel = 0;

    public struct PlayerScore {
        public int Rank { get; set; }
        public string Name { get; set; }
        public float Score { get; set; }
    }
    public GameObject HighScoreTemplate;
    static readonly int SCORE_COUNT = 10;

    public Dictionary<string, int> Difficulties = new Dictionary<string, int>() { { "Easy", 0 }, { "Medium", 1 }, { "Hard", 2 } };

    void Awake() {
        foreach (var item in Panels) {
            item.Panel.SetActive(false);
            item.HighScore = new List<PlayerScore>();
            item.HighScoreTemplate = HighScoreTemplate;
        }

        Panels[0].Panel.SetActive(true);
        _currentPanel = 0;
    }


    void OnEnable() {
        foreach (var item in Panels) {
            item.Panel.SetActive(false);
        }

        Panels[(int)DifficultyStateObject.CurDifficultyState].Panel.SetActive(true);
        _currentPanel = (int)DifficultyStateObject.CurDifficultyState;

#if AUTOSELECTPLAY
        var selectedButton = Panels[_currentPanel].Panel.GetComponentInChildren<Button>().gameObject;
        bool isComputer = Application.platform != RuntimePlatform.IPhonePlayer || Application.platform != RuntimePlatform.Android;
        if (selectedButton != null && isComputer)
            EventSystem.current.SetSelectedGameObject(selectedButton);
#endif

        UpdateHighScores();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            PreviousPanel();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            NextPanel();
        }

        foreach (var item in Panels) {
            if (item.LoadScore != null && item.LoadScore.IsCompleted) {
                item.FillHighscores();
            }
        }
    }

    public void NextPanel() {
        foreach (var item in Panels) {
            item.Panel.SetActive(false);
        }

        if (++_currentPanel >= Panels.Length)
            _currentPanel = 0;

        Panels[_currentPanel].Panel.SetActive(true);

#if AUTOSELECTPLAY
        var selectedButton = Panels[_currentPanel].Panel.GetComponentInChildren<Button>().gameObject;
        bool isComputer = Application.platform != RuntimePlatform.IPhonePlayer || Application.platform != RuntimePlatform.Android;
        if (selectedButton != null && isComputer)
            EventSystem.current.SetSelectedGameObject(selectedButton);
#endif
        UpdateHighScores();
    }

    public void PreviousPanel() {
        foreach (var item in Panels) {
            item.Panel.SetActive(false);
        }

        if (--_currentPanel < 0)
            _currentPanel = Panels.Length - 1;

        Panels[_currentPanel].Panel.SetActive(true);

#if AUTOSELECTPLAY
        var selectedButton = Panels[_currentPanel].Panel.GetComponentInChildren<Button>().gameObject;
        bool isComputer = Application.platform != RuntimePlatform.IPhonePlayer || Application.platform != RuntimePlatform.Android;
        if (selectedButton != null && isComputer)
            EventSystem.current.SetSelectedGameObject(selectedButton);
#endif
        UpdateHighScores();
    }

    private void UpdateHighScores() {
        Panels[_currentPanel].ClearHighscores();
        Panels[_currentPanel].Panel.transform.FindChild("Highscores").FindChild("Loading").gameObject.SetActive(true);

        Panels[_currentPanel].UpdateHighscores();
    }

    //This fixes a bug where returning to main menu would make the difficulty confirmation buttons stop functioning
    public void SetDifficulty(int difficulty)
    {
        DifficultyStateObject.Reference.SetDifficulty(difficulty);
    }
}
