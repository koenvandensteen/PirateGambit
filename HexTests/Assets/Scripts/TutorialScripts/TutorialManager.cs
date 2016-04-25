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
    PowderkegActivate,
    Count,
}

public class TutorialManager : BaseManager
{
    public bool ShowTutorial = true;
    private bool[] _tutorialIsShown = new bool[(int)TutorialInfo.Count];
    public delegate void ShowTutorialScreen(TutorialData info);
    public ShowTutorialScreen ShowTutorialScreenImplementationScreen;

    new private TutorialGenerator _mapGenerator;

    void Start()
    {
        GameState = GameState.Play;

        _arrowTransform = transform.FindChild("Arrow");
        _arrowTransform.gameObject.SetActive(false);

        StartNewGame();
    }

    public void StartNewGame()
    {
        GameState = GameState.Play;
        _timer = 0;


        if (GameMap != null)
        {
            GameMap.ClearMap();
        }

        CurDangerlevel = 0;
        if (_mapGenerator == null)
            _mapGenerator = GetComponent<TutorialGenerator>();

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

    public void CheckCurrentTile()
    {
        var curTile = GameMap.GetTile(_playerController.PlayerPosition).GetComponent<GameTile>();
        CurDangerlevel = curTile.BadNeighbours;

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

    override protected void ActivateTreasureTile(GameTile curTile)
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

    override protected void ActivateBadTile(GameTile curTile)
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

    override protected void ActivateTileMap(GameTile curTile)
    {
        if (curTile)
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
                    case GameTile.TileType.POWDERKEG:
                        if (!_tutorialIsShown[(int)TutorialInfo.PowderkegActivate])
                            ShowTutorialInfo(TutorialInfo.Powderkeg, new List<GameObject>() { tile.gameObject });
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

    override protected void ActivateTileKeg(GameTile curTile)
    {
        List<GameObject> tiles = new List<GameObject>();

        if (curTile)
        {
            HiddenTileList.Remove(HiddenTileList.Find(v => v == new Vector2(curTile.Q, curTile.R)));
        }
        
        
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
                    case GameTile.TileType.POWDERKEG:
                        if (!_tutorialIsShown[(int)TutorialInfo.PowderkegActivate])
                            ShowTutorialInfo(TutorialInfo.Powderkeg, new List<GameObject>() { neighbour.gameObject });
                        break;
                    default:
                        ShowTutorialInfo((TutorialInfo)neighbour.GetComponent<GameTile>().ThisType + 1, new List<GameObject>() { neighbour.gameObject });
                        break;
                }

                HiddenTileList.Remove(HiddenTileList.Find(v => v == new Vector2(neighbour.GetComponent<GameTile>().Q, neighbour.GetComponent<GameTile>().R)));
            }
        }

        if (curTile)
        {
            ShowTutorialInfo(TutorialInfo.PowderkegActivate, tiles);
            _tutorialIsShown[(int)TutorialInfo.Powderkeg] = true;
        }

        AudioManager.Instance.PlaySound("ExplosionSfx", 0.35f);

    }

    override protected void ActivateImuneTile(GameTile curTile)
    {
        HiddenTileList.Remove(HiddenTileList.Find(v => v == new Vector2(curTile.Q, curTile.R)));
        AudioManager.Instance.PlaySound("GlassClink", 1.0f);
        if (RumLevel <= MaxRumStack)
            RumLevel++;

        ShowTutorialInfo(TutorialInfo.ImmunityActivate, null);
        _tutorialIsShown[(int)TutorialInfo.Immunity] = true;
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
