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

public class BaseManager : MonoBehaviour
{
    private static BaseManager _instance;
    public static BaseManager Instance
    {
        get { return _instance; }
        protected set { _instance = value; }
    }

    #region UI

    protected PlayerController _playerController;
    protected Player _player;
    public InGameMenu_Main InGameMenu;

    private GameState _state = GameState.Play;
    public GameState GameState
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
        protected set
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

    protected Transform _arrowTransform;

    public HexMap GameMap { get; set; }


    #region mapVariables
    protected MapGenerator _mapGenerator;


    public int EasyMapSize = 7;
    public int MediumMapSize = 9;
    public int HardMapSize = 11;
    protected int _curMapSize;
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
        protected set
        {
            _isImune = value;
            PlayerController.Instance.PlayerRef.Shield.SetActive(value);
        }
    }

    public int MaxRumStack = 3;


    #endregion

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
    protected float _timer = 0;
    public float GameTime { get { return _timer; } }

    #endregion

    void Awake()
    {
        _instance = gameObject.GetComponent<BaseManager>();
    }

    public void PauseGame()
    {
        SwipeVisualization.gameObject.SetActive(false);
        GameState = GameState.Pause;
    }

    public void ResumeGame()
    {
        SwipeVisualization.gameObject.SetActive(true);
        GameState = GameState.Play;
    }

    // Update is called once per frame
    protected void Update()
    {

        if (GameState != GameState.Play || _player.IsDead)
            return;

        //Update timer for score
        _timer += Time.deltaTime;
    }

    virtual protected void ActivateTreasureTile(GameTile curTile){}

    virtual protected void ActivateBadTile(GameTile curTile){}

    virtual protected void ActivateTileMap(GameTile curTile){}

    virtual protected void ActivateTileKeg(GameTile curTile){}

    virtual protected void ActivateImuneTile(GameTile curTile){}

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

}
