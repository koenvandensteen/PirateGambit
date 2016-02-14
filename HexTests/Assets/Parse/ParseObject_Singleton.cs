using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Parse;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

public class ParseObject_Singleton : MonoBehaviour {

    public static ParseObject_Singleton Instance { get; private set; }
    public struct PlayerScore {
        public int Rank { get; set; }
        public string Name { get; set; }
        public float Score { get; set; }
    }

    public delegate void OnListReceived(List<PlayerScore> scoreList);
    public OnListReceived OnlistReceivedImplementation;

    private Task _loadScore = null;
    private Task _saveScore = null;
    private List<PlayerScore> _scoreList;

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

        _scoreList = new List<PlayerScore>(10);

    }

    void Update() {
        if (_loadScore != null && _loadScore.IsCompleted) {
            if (OnlistReceivedImplementation != null) {
                OnlistReceivedImplementation(_scoreList);
                OnlistReceivedImplementation = null;
            }
            _loadScore = null;
        }

        if (_saveScore != null && _saveScore.IsCompleted) {
            _saveScore = null;
        }

    }

    public void SubmitScore(string name, float score) {
        if (_saveScore != null) {
            return;
        }

        var newScore = new ParseObject("HighScores" + DifficultyStateObject.CurDifficultyState);
        newScore["playerName"] = name;
        newScore["score"] = score;
        _saveScore = newScore.SaveAsync();
    }

    public void RequestSurroundingScores(float inputScore, int amount, OnListReceived callback, int difficulty = -1) {
        if (_loadScore != null) {
            return;
        }

        OnlistReceivedImplementation += callback;

        if (difficulty == -1)
            difficulty = (int)DifficultyStateObject.CurDifficultyState;
        string sDifficulty = DifficultyStateObject.GetDifficultyName(difficulty);

        if (_scoreList != null)
            _scoreList.Clear();

        var query = ParseObject.GetQuery("HighScores" + sDifficulty).OrderBy("score");

        int counter = 0;

        _loadScore = query.FindAsync().ContinueWith(t => {
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
                _scoreList.Add(newScore);
            }

            //filter score
            var lessThan = _scoreList.FindAll(i => i.Score <= inputScore);
            lessThan.Reverse();
            var greaterThan = _scoreList.FindAll(i => i.Score > inputScore);

            _scoreList.Clear();

            for (int i = 0; i < ((lessThan.Count > (amount / 2)) ? (amount / 2) : lessThan.Count); i++) {
                _scoreList.Add(lessThan[i]);
            }
            lessThan.Sort((first, second) => first.Rank.CompareTo(second.Rank));

            if (lessThan.Count > 0) {
                _scoreList.Add(new PlayerScore() {
                    Rank = (lessThan[lessThan.Count - 1].Rank) + 1,
                    Name = string.Empty,
                    Score = inputScore
                });
            } else {
                _scoreList.Add(new PlayerScore() {
                    Rank = 1,
                    Name = string.Empty,
                    Score = inputScore
                });
            }

            for (int i = 0; i < ((greaterThan.Count > (amount / 2)) ? (amount / 2) : greaterThan.Count); i++) {
                var playerScore = greaterThan[i];
                playerScore.Rank++;
                _scoreList.Add(playerScore);
            }

            _scoreList.Sort((first, second) => first.Rank.CompareTo(second.Rank));


            if (_scoreList.Count < amount) {
                ++counter;
                for (int i = _scoreList.Count; i < amount; i++) {
                    var dummyPlayer = new PlayerScore() { Rank = ++counter, Name = "...", Score = -1 };
                    _scoreList.Add(dummyPlayer);
                }
            }
        });

    }

    public void RequestTop(int amount, OnListReceived callback, int difficulty = -1) {
        if (_loadScore != null) {
            return;
        }
        OnlistReceivedImplementation += callback;

        if (difficulty == -1)
            difficulty = (int)DifficultyStateObject.CurDifficultyState;
        string sDifficulty = DifficultyStateObject.GetDifficultyName(difficulty);

        if (_scoreList != null)
            _scoreList.Clear();


        var query = ParseObject.GetQuery("HighScores" + sDifficulty).OrderBy("score").Limit(amount);

        int counter = 0;

        _loadScore = query.FindAsync().ContinueWith(t => {
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
                _scoreList.Add(newScore);
            }

            if (_scoreList.Count < amount) {
                for (int i = _scoreList.Count; i < amount; i++) {
                    var dummyPlayer = new PlayerScore() { Rank = ++counter, Name = string.Empty, Score = -1 };
                    _scoreList.Add(dummyPlayer);
                }
            }
        });

    }
}
