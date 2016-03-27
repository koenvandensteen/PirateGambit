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
        private set {}
    }

    //private PlayerController _playerController = PlayerController.Instance;

    public static bool IsMobile = false;

    #region UI

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

    public PlayerMove _curPlayer
    {
        get; private set;
    }

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

    #region Input player Variables
    private Vector2 _mouseStartPos = Vector2.zero;
    private Vector2 _mouseEndPos = Vector2.zero;

    private float _rightClickCounter;
    public float TimeBetweenRightClick = 0.1f;

    public float MinSwipeLength = 20.0f;
    private bool _isSwiping = false;
    public Vector2 _curPos;

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
            _curPlayer.Shield.SetActive(value);
        }
    }

    public int MaxRumStack = 3;

    public float RotationSpeed;
    public float MovementSpeed;
    private bool _isFirstTurn = true;
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
        _rightClickCounter = TimeBetweenRightClick;
    }

    //private HighScoreScript _highScoreManager; 

    // Use this for initialization
    void Start()
    {

#if UNITY_ANDROID || UNITY_IOS
        IsMobile = true;
#endif

        State = GameState.Play;
        StartNewGame();

        _arrowTransform = transform.FindChild("Arrow");
        _arrowTransform.gameObject.SetActive(false);

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

        if (_curPlayer != null)
        {
            Destroy(_curPlayer.gameObject);
        }

        if (GameMap != null)
        {
            GameMap.ClearMap();
        }

        _curPos = new Vector2(0, 0);
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

        if(PlayerObject == null)
            Debug.Log("player object is null");

        if (GameMap.GetTile(_curPos) == null)
            Debug.Log("get tile is null");


        _curPlayer = (Instantiate(PlayerObject, GameMap.GetTile(_curPos).transform.position, Quaternion.identity) as GameObject).GetComponent<PlayerMove>();
        _curPlayer.MovementSpeed = MovementSpeed;
        _curPlayer.RotationSpeed = RotationSpeed;
        _curPlayer.IsDead = false;

        GameCamera.GetComponent<CameraSpringZoom>().PlayerTransform = _curPlayer.transform;

        _isFirstTurn = true;
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
        ActivateTileKeg();
    }

    // Update is called once per frame
    void Update()
    {
        if (State != GameState.Play || _curPlayer.IsDead)
            return; //don't update swipe gestures when in menu, use UI Onclick.

        //Update timer for score
        _timer += Time.deltaTime;

        if (_rightClickCounter > 0)
        {
            _rightClickCounter -= Time.deltaTime;
        }

        if (!_curPlayer.IsMoving)
        {
            if (IsMobile || EnableSwipe)
            {
                //Start swipe

                if (Input.touchCount >= 2)
                {
                    _isSwiping = false;
                    return;
                }

                if (Input.GetMouseButtonDown(0) && Input.touchCount < 2)
                {
                    _isSwiping = true;
                    _mouseStartPos = Input.mousePosition;
                }
                //During swipe
                if (_isSwiping)
                {
                    Vector2 moveOffset = GetMoveOffset((float)GetSwipeAngle());
                    var tile = GameMap.GetTile(_curPos + moveOffset);

                    if (tile)
                    {
                        if (_selectedTile)
                        {
                            _selectedTile.Highlighted = false;
                            _selectedTile = null;
                        }
                        if (((Vector3)_mouseStartPos - Input.mousePosition).magnitude > MinSwipeLength)
                        {
                            SwipeVisualization.DoShow = true;
                            _selectedTile = tile.GetComponent<GameTile>();
                            _selectedTile.Highlighted = true;
                        }
                        else
                        {
                            SwipeVisualization.DoShow = false;
                        }
                    }
                }
                else
                {
                    if (_selectedTile)
                    {
                        _selectedTile.Highlighted = false;
                        _selectedTile = null;
                    }
                }

                //End swipe
                if (Input.GetMouseButtonUp(0) && _isSwiping && !_curPlayer.IsMoving)
                {
                    _isSwiping = false;
                    _mouseEndPos = Input.mousePosition;
                    ProcesSwipe();
                    if (_selectedTile)
                    {
                        _selectedTile.Highlighted = false;
                        _selectedTile = null;
                    }
                }
            }

            if (!_curPlayer.IsMoving)
            {
                CheckCurrentTile();
            }
        }

    }

    void ProcesSwipe()
    {
        Vector2 vectorDir = _mouseEndPos - _mouseStartPos;

        if (vectorDir.magnitude > MinSwipeLength)
        {
            MovePlayer(vectorDir);
        }
        else
        {
            //ProcessRightMouseClick();
        }
    }

    Vector2 GetMoveOffset(float angle)
    {
        if (angle > 0 && angle < 60) //Move Right Up
        {
            return new Vector2(+1, -1);
        }
        else if (angle > 60 && angle < 120) //move right
        {
            return new Vector2(+1, 0);
        }
        else if (angle > 120 && angle < 180) //move right down
        {
            return new Vector2(0, +1);
        }
        else if (angle > 180 && angle < 240) //move left down
        {
            return new Vector2(-1, +1);
        }
        else if (angle > 240 && angle < 300) //move left
        {
            return new Vector2(-1, 0);
        }
        else //move left up
        {
            return new Vector2(0, -1);
        }
    }

    void MovePlayer(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.x, direction.y) * 180 / Mathf.PI;
        if (angle < 0)
            angle += 360;

        Vector2 moveOffset = GetMoveOffset(angle);

        var tile = GameMap.GetTile(_curPos + moveOffset);

        if (tile)
        {
            //Check if the status is CLEAR (no flag placed on the tile)
            if (tile.GetComponent<GameTile>().CurrentTileStatus == GameTile.TileStatus.CLEAR)
            {
                if (_isFirstTurn && tile.GetComponent<GameTile>().ThisType == GameTile.TileType.BAD)
                {

                }
                else
                {
                    tile.GetComponent<GameTile>().ShowObject();
                }

                _curPlayer.SetMoveStart(tile.transform.position);
                _curPos += moveOffset;
                CurDangerlevel = GameMap.GetTile(_curPos).GetComponent<GameTile>().BadNeighbours;
            }
        }
    }

    int GetSwipeAngle()
    {
        Vector2 curMousePos = Input.mousePosition;
        Vector2 vectorDir = curMousePos - _mouseStartPos;

        if (!IsMobile && !EnableSwipe)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            float delta = 0;
            plane.Raycast(ray, out delta);

            var clickedPos = ray.origin + ray.direction * delta;

            vectorDir = new Vector2((clickedPos.x - _curPlayer.transform.position.x), (clickedPos.z - _curPlayer.transform.position.z));
        }

        float angle = Mathf.Atan2(vectorDir.x, vectorDir.y) * 180 / Mathf.PI;
        if (angle < 0)
            angle += 360;

        int lockedAngle = Mathf.FloorToInt(angle * (6.0f / 360.0f)) * 60 + 30;

        return lockedAngle;
    }

    public void CheckCurrentTile()
    {

        var curTile = GameMap.GetTile(_curPos).GetComponent<GameTile>();

        if (curTile.IsActivated)
            return;

        //curTile.ShowObject();

        //Set child rotation to match player
        for (int i = 0; i < curTile.transform.childCount; i++)
        {
            curTile.transform.GetChild(i).transform.rotation = _curPlayer.transform.rotation * Quaternion.Euler(0, 180, 0);
        }

        switch (curTile.ThisType)
        {
            case GameTile.TileType.EMPTY:
                curTile.ActivateTile();
                HiddenTileList.Remove(HiddenTileList.Find(v => v == new Vector2(curTile.Q, curTile.R)));
                break;

            case GameTile.TileType.TREASURE:
                curTile.ActivateTile();
                HiddenTileList.Remove(HiddenTileList.Find(v => v == new Vector2(curTile.Q, curTile.R)));
                AudioManager.Instance.PlaySound("coinSfx", 1.2f);
                ++CollectedTreasureAmount;

                if (TreasureParticles != null)
                {
                    var ps = Instantiate(TreasureParticles, curTile.transform.position, Quaternion.Euler(-90, 0, 0)) as ParticleSystem;
                    ps.Play();
                    Destroy(ps, 2.0f);
                }
                break;

            case GameTile.TileType.BAD:
                if (_arrowTransform)
                    _arrowTransform.gameObject.SetActive(false);

                if (!IsImune && !_isFirstTurn)
                {

                    curTile.ActivateTile();
                    GameOver(false);
                    Destroy(gameObject);

                }
                else if (_isFirstTurn)
                {
                    curTile.GetComponent<GameTile>().BorderColor = GetComponent<MapGenerator>().BorderColors[0];
                    curTile.GetComponent<GameTile>().ThisType = GameTile.TileType.EMPTY;
                    curTile.GetComponent<GameTile>().IsEmpty = true;

                    curTile.GetComponent<GameTile>().ActivateTile();
                    HiddenTileList.Remove(HiddenTileList.Find(v => v == new Vector2(curTile.Q, curTile.R)));
                    --MaxKrakenAmmount;
                    GameMap.LowerNeighbourDangerCounter(_curPos);
                }
                else if (_isImune)
                {
                    GameMap.LowerNeighbourDangerCounter(_curPos);
                    curTile.ActivateTile();
                    HiddenTileList.Remove(HiddenTileList.Find(v => v == new Vector2(curTile.Q, curTile.R)));
                    Destroy(curTile.GetComponent<GameTile>().HiddenObject);
                    AudioManager.Instance.PlaySound("KrakenDiesSfx");
                    ++CurKrakenAmmount;
                }
                break;

            case GameTile.TileType.MAP:
                {
                    curTile.ActivateTile();
                    HiddenTileList.Remove(HiddenTileList.Find(v => v == new Vector2(curTile.Q, curTile.R)));
                    AudioManager.Instance.PlaySound("ShieldGongSfx", 0.6f);

                    var list = new List<GameObject>();
                    list = ActivateTileMap();

                    ShowTutorialInfo(TutorialInfo.MapActivate, list);
                    _tutorialIsShown[(int)TutorialInfo.Map] = true;

                    foreach (var tile in list)
                    {
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

                    }
                }
                break;

            case GameTile.TileType.POWDERKEG:
                {
                    curTile.ActivateTile();
                    HiddenTileList.Remove(HiddenTileList.Find(v => v == new Vector2(curTile.Q, curTile.R)));
                    var list = ActivateTileKeg();
                    ShowTutorialInfo(TutorialInfo.Powderkeg, list);
                    foreach (var tile in list)
                    {
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

                    }

                    AudioManager.Instance.PlaySound("ExplosionSfx", 0.35f);
                }
                break;
            case GameTile.TileType.IMUNE:
                curTile.ActivateTile();
                HiddenTileList.Remove(HiddenTileList.Find(v => v == new Vector2(curTile.Q, curTile.R)));
                AudioManager.Instance.PlaySound("GlassClink", 1.0f);
                if (RumLevel <= MaxRumStack)
                    RumLevel++;

                ShowTutorialInfo(TutorialInfo.ImmunityActivate, null);
                _tutorialIsShown[(int)TutorialInfo.Immunity] = true;

                break;
            default:
                break;
        }

        IsImune = false;
        _isFirstTurn = false;

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

    List<GameObject> ActivateTileMap()
    {
        List<GameObject> tiles = new List<GameObject>(MapTilePower);

        for (int i = 0; i < MapTilePower; ++i)
        {
            if (HiddenTileList.Count <= 0)
                return tiles;

            int randomIndex = Random.Range(0, HiddenTileList.Count);
            var tileCoords = HiddenTileList[randomIndex];

            var tile = GameMap.GetTile(tileCoords);
            if (tile != null)
            {
                tiles.Add(tile);
                tile.GetComponent<GameTile>().ShowObject();
                if (tile.GetComponent<GameTile>().ThisType == GameTile.TileType.BAD && tile.GetComponent<GameTile>().CurrentTileStatus == GameTile.TileStatus.CLEAR)
                    ++CurKrakenAmmount;
                HiddenTileList.RemoveAt(randomIndex);
            }

        }
        return tiles;
    }

    List<GameObject> ActivateTileKeg()
    {
        List<GameObject> tiles = new List<GameObject>();

        foreach (var neighbour in GameMap.GetNeighBours(_curPos))
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
                HiddenTileList.Remove(HiddenTileList.Find(v => v == new Vector2(neighbour.GetComponent<GameTile>().Q, neighbour.GetComponent<GameTile>().R)));
            }
        }
        return tiles;
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
