//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public enum GameState
{
    Pause,
    Play,
}

public enum TutorialInfo
{
    Intro,
    FirstMove,
    Treasure,
    Kraken,
    Map,
    Powderkeg,
    Immunity,
    ImmunityActivate,
    MapActivate,
    Count
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
        private set { }
    }

    #region UI

    private PlayerController _playerController;
    private Player _player;
    public GameObject GameCamera;
    public InGameMenu_Main InGameMenu;

    private GameState _state = GameState.Play;
    public GameState State
    {
        get
        {
            return _state;
        }
        set
        {
            switch (value)
            {
                case GameState.Pause:
                    break;
                case GameState.Play:
                    break;
                default:
                    break;
            }
            _state = value;
        }
    }


    public int MaxTreasureAmmount { get; set; }

    public ParticleSystem TreasureParticles;
    public BarrelRevealScript BarrelParticles;
    public MapRevealScript MapParticles;

    public List<Vector2> HiddenTileList;

    [HideInInspector]
    public int MaxKrakenAmmount;

    #region Current Krakens
    private int _curKrakenAmmount;

    public int CurKrakenAmmount
    {
        get { return _curKrakenAmmount; }
        set
        {
            _curKrakenAmmount = value;
            if (CurKrakenAmmountChangedImplementation != null)
                CurKrakenAmmountChangedImplementation(value);
        }
    }

    public delegate void OnKrakenAmountChanged(int newValue);
    public OnKrakenAmountChanged CurKrakenAmmountChangedImplementation;
    #endregion



    #endregion

    #region Collected Treasure
    private int _collectedTreasureAmount;
    public int CollectedTreasureAmount
    {
        get { return _collectedTreasureAmount; }
        private set
        {
            _collectedTreasureAmount = value;
            if (_collectedTreasureAmount >= MaxTreasureAmmount)
            {
                GameOver(true);
            }
            if (TreasureChangedImplementation != null)
                TreasureChangedImplementation(value);
        }
    }

    public delegate void OnTreasureChanged(int newValue);
    public OnTreasureChanged TreasureChangedImplementation;
    #endregion

    #region Danger Level
    private int _curDangerLevel;
    public int CurDangerlevel
    {
        get { return _curDangerLevel; }
        set
        {
            _curDangerLevel = value;
            if (DangerLevelChangedImplementation != null)
                DangerLevelChangedImplementation(value);
        }
    }

    public delegate void DangerLevelChanged(int newValue);
    public DangerLevelChanged DangerLevelChangedImplementation;
    #endregion

    #region Immunity Level
    private int _RumLevel;
    public int RumLevel
    {
        get { return _RumLevel; }
        set
        {
            _RumLevel = value;
            if (RumlevelChangedImplementation != null)
                RumlevelChangedImplementation(value);
        }
    }

    public delegate void RumLevelChanged(int newValue);
    public RumLevelChanged RumlevelChangedImplementation;

    #endregion

    public bool ShowTutorial = true;
    private bool[] _tutorialIsShown = new bool[(int)TutorialInfo.Count];
    public delegate void ShowTutorialScreen(TutorialData info);
    public ShowTutorialScreen ShowTutorialScreenImplementationScreen;

    public GameObject PlayerObject;

    private Transform _arrowTransform;
    public GameTile _selectedTile;

    public HexMap GameMap;

    public Vector3 LookAtTarget { get; set; }

    public bool IsNewPosition = false;
    public GameObject SelectedTile;

    #region mapVariables
    private MapGenerator _mapGenerator;


    public int EasyMapSize = 7;
    public int MediumMapSize = 9;
    public int HardMapSize = 11;
    private int _curMapSize;
    public int HexRadius = 1;
    public int MapTilePower = 5;

    public int HiddenTiles { get; set; }
    #endregion


    #region Player Variables
    private bool _isImune = false;
    public bool IsImune
    {
        get
        {
            return _isImune;
        }
        private set
        {
            _isImune = value;
            PlayerController.Instance.PlayerRef.Shield.SetActive(value);
        }
    }

    public int MaxRumStack = 3;

    
    public bool HasMoved = false;
    #endregion

    #region Camera Variables
    public float CameraFollowSpeed;
    public Vector2 CameraOffset;

    #endregion
    Plane _plane = new Plane(Vector3.up, Vector3.zero);


    public SwipeManager SwipeVisualization;
    private bool _swipingEnabled = false;

    public bool EnableSwipe
    {
        get
        {
            return _swipingEnabled;
        }
        set
        {
            if (SwipeVisualization != null)
            {
                SwipeVisualization.DoShow = value;
            }
            _swipingEnabled = value;
        }
    }
    #region Timer
    private float _timer = 0;
    public float GameTime { get { return _timer; } }
    #endregion

    void Awake()
    {
        _instance = gameObject.GetComponent<GameManager>();
    }

    //private HighScoreScript _highScoreManager; 

    // Use this for initialization
    void Start()
    {
        State = GameState.Play;
        
        _arrowTransform = transform.FindChild("Arrow");
        _arrowTransform.gameObject.SetActive(false);

        StartNewGame();
    }

    public void PauseGame()
    {
        SwipeVisualization.gameObject.SetActive(false);
        State = GameState.Pause;
    }

    public void ResumeGame()
    {
        SwipeVisualization.gameObject.SetActive(true);
        State = GameState.Play;
    }

    public void StartNewGame()
    {
        State = GameState.Play;
        _timer = 0;


        if (GameMap != null)
        {
            GameMap.ClearMap();
        }

        CurDangerlevel = 0;
        if (_mapGenerator == null)
            _mapGenerator = GetComponent<MapGenerator>();

        switch (DifficultyStateObject.CurDifficultyState)
        {
            case DifficultyStateObject.DifficultyState.Easy:
                _curMapSize = EasyMapSize;
                break;
            case DifficultyStateObject.DifficultyState.Medium:
                _curMapSize = MediumMapSize;
                break;
            case DifficultyStateObject.DifficultyState.Hard:
                _curMapSize = HardMapSize;
                break;
        }

        GameMap = _mapGenerator.CreateHexMap(_curMapSize, HexRadius);
        GameMap.CheckNeighbours();

        GameMap.GetTile(Vector2.zero).GetComponent<GameTile>().ThisType = GameTile.TileType.EMPTY;
        GameMap.GetTile(Vector2.zero).GetComponent<GameTile>().ActivateTile();

        CollectedTreasureAmount = 0;
        
        EnableSwipe = PlayerPrefs.GetInt("Swiping") == 0 ? false : true;
        ShowTutorial = PlayerPrefs.GetInt("Tutorial") == 0 ? false : true;

        RumLevel = 0;
        CurKrakenAmmount = 0;

        for (int i = 0; i < (int)TutorialInfo.Count; i++)
        {
            _tutorialIsShown[i] = false;
        }
        ShowTutorialInfo(TutorialInfo.Intro, null);
        ShowTutorialInfo(TutorialInfo.FirstMove, null);

        //Play the ambient sound effect
        AudioManager.Instance.PlayAmbientSfx("NewAmbienceSfx_00");
       

        _playerController = PlayerController.Instance;
        _playerController.StartGame();
        _player = _playerController.PlayerRef;
        _player.ArrivedOnHex += CheckCurrentTile;
        ActivateTileKeg(null);
    }


    // Update is called once per frame
    void Update()
    {

        if (State != GameState.Play || _player.IsDead)
            return;

        //Update timer for score
        _timer += Time.deltaTime;

    }



    public void CheckCurrentTile()
    {

        var curTile = GameMap.GetTile(_playerController.PlayerPosition).GetComponent<GameTile>();

        if (curTile.IsActivated)
            return;
        curTile.ActivateTile();

        //Set child rotation to match player
        for (int i = 0; i < curTile.transform.childCount; i++)
        {
            curTile.transform.GetChild(i).transform.rotation = _player.transform.rotation * Quaternion.Euler(0, 180, 0);
        }

        switch (curTile.ThisType)
        {
            case GameTile.TileType.EMPTY:            
                HiddenTileList.Remove(HiddenTileList.Find(v => v == new Vector2(curTile.Q, curTile.R)));
                break;

            case GameTile.TileType.TREASURE:
                ActivateTreasureTile(curTile);
                break;

            case GameTile.TileType.BAD:
                ActivateBadTile(curTile);
                break;

            case GameTile.TileType.MAP:
                ActivateTileMap(curTile);
                break;

            case GameTile.TileType.POWDERKEG:
                ActivateTileKeg(curTile);
                break;

            case GameTile.TileType.IMUNE:
                ActivateImuneTile(curTile);
                break;

            default:
                break;
        }

        IsImune = false;

        switch (curTile.GetComponent<GameTile>().ThisType)
        {
            case GameTile.TileType.MAP:
                if (!_tutorialIsShown[(int)TutorialInfo.MapActivate])
                    ShowTutorialInfo(TutorialInfo.Map, null);
                break;
            case GameTile.TileType.IMUNE:
                if (!_tutorialIsShown[(int)TutorialInfo.ImmunityActivate])
                    ShowTutorialInfo(TutorialInfo.Immunity, null);
                break;
            default:
                ShowTutorialInfo((TutorialInfo)curTile.GetComponent<GameTile>().ThisType + 1, null);
                break;
        }

    }

    void ActivateTreasureTile(GameTile curTile)
    {
        HiddenTileList.Remove(HiddenTileList.Find(v => v == new Vector2(curTile.Q, curTile.R)));
        AudioManager.Instance.PlaySound("coinSfx", 1.2f);
        ++CollectedTreasureAmount;

        if (TreasureParticles != null)
        {
            var ps = Instantiate(TreasureParticles, curTile.transform.position, Quaternion.Euler(-90, 0, 0)) as ParticleSystem;
            ps.Play();
            Destroy(ps, 2.0f);
        }
    }

    void ActivateBadTile(GameTile curTile)
    {
        if (_arrowTransform)
            _arrowTransform.gameObject.SetActive(false);

        if (!IsImune)
        {
            GameOver(false);
            Destroy(gameObject);
        }
        else
        {
            GameMap.LowerNeighbourDangerCounter(_playerController.PlayerPosition);
            HiddenTileList.Remove(HiddenTileList.Find(v => v == new Vector2(curTile.Q, curTile.R)));
            Destroy(curTile.GetComponent<GameTile>().HiddenObject);
            AudioManager.Instance.PlaySound("KrakenDiesSfx");
            ++CurKrakenAmmount;
        }
    }

    void ShowTutorialInfo(TutorialInfo tutInfo, List<GameObject> gameObjects)
    {
        if (ShowTutorial)
        {

            if (!_tutorialIsShown[(int)tutInfo])
            {
                if (ShowTutorialScreenImplementationScreen != null)
                {
                    ShowTutorialScreenImplementationScreen(new TutorialData(tutInfo, gameObjects));
                    _tutorialIsShown[(int)tutInfo] = true;
                }
            }

        }
    }

    void ActivateTileMap(GameTile curTile)
    {
        if(curTile)
        {
            HiddenTileList.Remove(HiddenTileList.Find(v => v == new Vector2(curTile.Q, curTile.R)));
        }

        AudioManager.Instance.PlaySound("ShieldGongSfx", 0.6f);
        List<GameObject> tiles = new List<GameObject>(MapTilePower);

        for (int i = 0; i < MapTilePower; ++i)
        {
            if (HiddenTileList.Count <= 0)
                return;

            int randomIndex = Random.Range(0, HiddenTileList.Count);
            var tileCoords = HiddenTileList[randomIndex];

            var tile = GameMap.GetTile(tileCoords);
            if (tile != null)
            {
                tiles.Add(tile);
                tile.GetComponent<GameTile>().ShowObject();
                tile.GetComponent<GameTile>().ShowEmphasis(3.0f);
                switch (tile.GetComponent<GameTile>().ThisType)
                {
                    case GameTile.TileType.MAP:
                        if (!_tutorialIsShown[(int)TutorialInfo.MapActivate])
                            ShowTutorialInfo(TutorialInfo.Map, new List<GameObject>() { tile.gameObject });
                        break;
                    case GameTile.TileType.IMUNE:
                        if (!_tutorialIsShown[(int)TutorialInfo.ImmunityActivate])
                            ShowTutorialInfo(TutorialInfo.Immunity, new List<GameObject>() { tile.gameObject });
                        break;
                    default:
                        ShowTutorialInfo((TutorialInfo)tile.GetComponent<GameTile>().ThisType + 1, new List<GameObject>() { tile.gameObject });
                        break;
                }

                if (tile.GetComponent<GameTile>().ThisType == GameTile.TileType.BAD && tile.GetComponent<GameTile>().CurrentTileStatus == GameTile.TileStatus.CLEAR)
                    ++CurKrakenAmmount;
                HiddenTileList.RemoveAt(randomIndex);
            }

        }

        ShowTutorialInfo(TutorialInfo.MapActivate, tiles);
        _tutorialIsShown[(int)TutorialInfo.Map] = true;
    }

    void ActivateTileKeg(GameTile curTile)
    {
        if (curTile)
        {
            HiddenTileList.Remove(HiddenTileList.Find(v => v == new Vector2(curTile.Q, curTile.R)));
        }
        List<GameObject> tiles = new List<GameObject>();

        foreach (var neighbour in GameMap.GetNeighBours(_playerController.PlayerPosition))
        {
            if (neighbour)
            {
                if (neighbour.GetComponent<GameTile>().IsHidden)
                {
                    neighbour.GetComponent<GameTile>().ShowEmphasis(3.0f);
                    tiles.Add(neighbour);
                }
                if (neighbour.GetComponent<GameTile>().ThisType == GameTile.TileType.BAD
                    && neighbour.GetComponent<GameTile>().CurrentTileStatus == GameTile.TileStatus.CLEAR
                    && !neighbour.GetComponent<GameTile>().IsActivated
                    && neighbour.GetComponent<GameTile>().IsHidden)
                {
                    ++CurKrakenAmmount;
                }

                neighbour.GetComponent<GameTile>().ShowObject();
                neighbour.GetComponent<GameTile>().ShowEmphasis(3.0f);

                switch (neighbour.GetComponent<GameTile>().ThisType)
                {
                    case GameTile.TileType.MAP:
                        if (!_tutorialIsShown[(int)TutorialInfo.MapActivate])
                            ShowTutorialInfo(TutorialInfo.Map, new List<GameObject>() { neighbour.gameObject });
                        break;
                    case GameTile.TileType.IMUNE:
                        if (!_tutorialIsShown[(int)TutorialInfo.ImmunityActivate])
                            ShowTutorialInfo(TutorialInfo.Immunity, new List<GameObject>() { neighbour.gameObject });
                        break;
                    default:
                        ShowTutorialInfo((TutorialInfo)neighbour.GetComponent<GameTile>().ThisType + 1, new List<GameObject>() { neighbour.gameObject });
                        break;
                }

                HiddenTileList.Remove(HiddenTileList.Find(v => v == new Vector2(neighbour.GetComponent<GameTile>().Q, neighbour.GetComponent<GameTile>().R)));
            }
        }


        ShowTutorialInfo(TutorialInfo.Powderkeg, tiles);

        AudioManager.Instance.PlaySound("ExplosionSfx", 0.35f);

    }

    void ActivateImuneTile(GameTile curTile)
    {
        HiddenTileList.Remove(HiddenTileList.Find(v => v == new Vector2(curTile.Q, curTile.R)));
        AudioManager.Instance.PlaySound("GlassClink", 1.0f);
        if (RumLevel <= MaxRumStack)
            RumLevel++;

        ShowTutorialInfo(TutorialInfo.ImmunityActivate, null);
        _tutorialIsShown[(int)TutorialInfo.Immunity] = true;
    }

    public void ActivateImunity()
    {
        if (RumLevel <= 0)
            return;

        --RumLevel;
        IsImune = true;
        AudioManager.Instance.PlaySound("GlugSfx", 1.8f);

    }

    public void Quit()
    {
        //if windows
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ToMain()
    {
        AudioManager.Instance.StopCurrentAmbientSfx();

        SceneManager.LoadScene(0);
    }

    public void GameOver(bool isWin)
    {

        if (!isWin)
            AudioManager.Instance.PlaySound("ShipBreakSfx");
        else
            AudioManager.Instance.PlaySound("Tada", 1.1f);

        InGameMenu.GameOver(isWin, _timer);
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            Destroy(transform.parent.GetChild(i).gameObject);
        }
    }

    public void SetSwiping(bool value)
    {
        EnableSwipe = value;
    }

    public void SetTutorial(bool value)
    {
        ShowTutorial = value;
        PlayerPrefs.SetInt("Tutorial", value ? 1 : 0);
    }
    public void SetTutorialInverse(bool value)
    {
        ShowTutorial = !value;
        PlayerPrefs.SetInt("Tutorial", !value ? 1 : 0);
    }
}
