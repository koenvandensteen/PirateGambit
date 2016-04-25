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

public class GameManager : BaseManager 
{

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


        RumLevel = 0;
        CurKrakenAmmount = 0;


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

                if (tile.GetComponent<GameTile>().ThisType == GameTile.TileType.BAD && tile.GetComponent<GameTile>().CurrentTileStatus == GameTile.TileStatus.CLEAR)
                    ++CurKrakenAmmount;
                HiddenTileList.RemoveAt(randomIndex);
            }
        }

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


                HiddenTileList.Remove(HiddenTileList.Find(v => v == new Vector2(neighbour.GetComponent<GameTile>().Q, neighbour.GetComponent<GameTile>().R)));
            }
        }


        AudioManager.Instance.PlaySound("ExplosionSfx", 0.35f);

    }

    override protected void ActivateImuneTile(GameTile curTile)
    {
        HiddenTileList.Remove(HiddenTileList.Find(v => v == new Vector2(curTile.Q, curTile.R)));
        AudioManager.Instance.PlaySound("GlassClink", 1.0f);
        if (RumLevel <= MaxRumStack)
            RumLevel++;

    }

}
