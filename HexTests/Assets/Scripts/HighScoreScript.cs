//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using System;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using Parse;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class HighScoreScript : MonoBehaviour
{
    public static HighScoreScript HighScoreRef;
    public Canvas MainMenuCanvas;
    public GameObject HighScorePanel;
    public Canvas InputCanvas;
    public Text HighScoreText;

    private bool _updateList = false;

    public struct PlayerScore
    {
        public int Rank { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
    }
    public List<PlayerScore> ScoreList;
    public int Score;
    private Task _loadScore;
    private Task _addNewScore;

    [SerializeField]
    public InputField NameInputField = null;

    void Awake()
    {
        HighScoreRef = GetComponent<HighScoreScript>();
    }

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        ScoreList = new List<PlayerScore>();

        // Add validation
        NameInputField.characterValidation = InputField.CharacterValidation.Alphanumeric;
    }

    public void SubmitName()
    {
        var newScore = new ParseObject("HighScores" + DifficultyStateObject.CurDifficultyState);
        newScore["playerName"] = NameInputField.text;
        newScore["score"] = Score;
        _addNewScore = newScore.SaveAsync();
        InputCanvas.GetComponent<Canvas>().enabled = false;
        ScoreList.Clear();

        foreach (var text in HighScorePanel.GetComponentsInChildren<Text>())
        {
            text.text = "loading";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_updateList && _loadScore.IsCompleted && Application.loadedLevelName.Equals("MainMenu"))
        {
            PrintList();
            _updateList = false;
        }

        if (_addNewScore != null && _addNewScore.IsCompleted)
        {
            if(!_updateList && ScoreList.Count <= 0)
                LoadCurrentRank(DifficultyStateObject.CurDifficultyState.ToString(),Score);

            if (_loadScore.IsCompleted)
            {
                PrintList();
                _updateList = false;
            }
        }
        

        if (_loadScore != null && !_loadScore.IsCompleted)
        {
            foreach (var button in GetComponentsInChildren<Button>())
            {
                button.interactable = false;
            }
        }
        else
        {
            foreach (var button in GetComponentsInChildren<Button>())
            {
                button.interactable = true;
            }
        }
    }

    public void ShowHighScores()
    {
        if (Application.loadedLevelName.Equals("MainMenu"))
        {
            LoadTop10("Easy");
            MainMenuCanvas.GetComponent<Canvas>().enabled = false;
            GetComponentInChildren<Canvas>().enabled = true;
        }
        else
        {
            GetComponentInChildren<Canvas>().enabled = true;
            InputCanvas.GetComponent<Canvas>().enabled = true;
            HighScoreText.text = string.Format("Score: {0}:{1}",Score/60,Score%60);         
        }
    }

    public void LoadTop10(string difficulty)
    {
        foreach (var text in HighScorePanel.GetComponentsInChildren<Text>())
        {
            text.text = "loading";
        }

        if (ScoreList != null)
            ScoreList.Clear();

        var query = ParseObject.GetQuery("HighScores" + difficulty).OrderBy("score").Limit(10);
        int counter = 0;
        _loadScore = query.FindAsync().ContinueWith(t =>
        {
            IEnumerable<ParseObject> results = t.Result;
            foreach (var obj in results)
            {
                var score = obj.Get<int>("score");
                var name = obj.Get<string>("playerName");

                counter++;
                var newScore = new PlayerScore
                {
                    Rank = counter,
                    Name = name,
                    Score = score
                };
                ScoreList.Add(newScore);
            }
        });
        _updateList = true;
    }


    public void LoadCurrentRank(string difficulty,int endScore)
    {
        if (ScoreList != null)
            ScoreList.Clear();

        var query = ParseObject.GetQuery("HighScores" + difficulty).OrderBy("score");

        _loadScore = query.FindAsync().ContinueWith(t =>
        {
            IEnumerable<ParseObject> results = t.Result;
            int rankCounter = 0;
            int excessCounter = 0;
            foreach (var obj in results)
            {
                var score = obj.Get<int>("score");
                var name = obj.Get<string>("playerName");
                rankCounter++;
                var newScore = new PlayerScore
                {
                    Rank = rankCounter,
                    Name = name,
                    Score = score
                };

                if(ScoreList.Count >= 10)
                    ScoreList.RemoveAt(0);

                ScoreList.Add(newScore);
                

                if (endScore <= score)
                {
                    ++excessCounter;
                    if(excessCounter > 5)
                        break;
                }
            }


        });
        _updateList = true;
    }


    void PrintList()
    {
        int counter = 0;

        foreach (var text in HighScorePanel.GetComponentsInChildren<Text>())
        {
            text.text = "none? :o";
        }

        foreach (var score in ScoreList)
        {
            var text = HighScorePanel.GetComponentsInChildren<Text>()[counter];
            ++ counter;
            text.text = string.Format("{0}. {1} - {2}:{3}", score.Rank, score.Name, score.Score / 60, score.Score % 60);
            if(counter >= 10)
                break;
            
        }
    }

    public void ReturnToMainMenu()
    {
        if (Application.loadedLevelName.Equals("MainMenu"))
        {
            GetComponentInChildren<Canvas>().enabled = false;
            MainMenuCanvas.GetComponent<Canvas>().enabled = true;
        }
        else
        {
            GetComponentInChildren<Canvas>().enabled = false;
        }

    }
}
