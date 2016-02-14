using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

    
    //map specifiks

    private float _tileSize = 0;
    private int _mapSize = 0;
    private GameObject _gameTile = null;
    private List<int> _freeIndexList;
    
    private int _badTiles;
    private int _treasureTiles;
    private int _mapTiles;
    private int _imuneTiles;
    private int _flareTiles;
    private int _cannonTiles;

    private int _curBadTiles;
    private int _curTreasureTiles;
    private int _curMapTiles;
    private int _curImuneTiles;
    private int _curFlareTiles;
    private int _curCannonTiles;

    public Material BadObjectMat;
    public Material TreasureObjectMat;
    public Material ImuneObjectMat;
    public Material MapObjectMat;
    public Material FlareObjectMat;
    public Material CannonObjectMat;
    public Material EmptyObjectMat;
    

    private GameManager _thisManager = null;
    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (!_thisManager)
        {
            _thisManager = GameManager.ThisManager;
            _tileSize = _thisManager.TileSize;
            _mapSize = _thisManager.MapSize;
            _gameTile = _thisManager.TileObject;
            _freeIndexList = _thisManager.FreeIndexList;

            if (_thisManager.IsPercentage)
            {
                _badTiles = _thisManager.BadTiles / (_mapSize * _mapSize);
                _treasureTiles = _thisManager.TreasureTiles / (_mapSize * _mapSize);
                _mapTiles = _thisManager.MapTiles / (_mapSize * _mapSize);
                _imuneTiles = _thisManager.ImuneTiles / (_mapSize * _mapSize);
                _flareTiles = _thisManager.FlareTiles / (_mapSize * _mapSize);
                _cannonTiles = _thisManager.CannonTiles / (_mapSize * _mapSize);
            }
            else
            {
                _badTiles = _thisManager.BadTiles;
                _treasureTiles = _thisManager.TreasureTiles;
                _mapTiles = _thisManager.MapTiles;
                _imuneTiles = _thisManager.ImuneTiles;
                _flareTiles = _thisManager.FlareTiles;
                _cannonTiles = _thisManager.CannonTiles;
            }

        }

    }

    public List<GameObject> GenerateMap()
    {

    _curBadTiles = 0;
    _curTreasureTiles = 0;
    _curMapTiles = 0;
    _curImuneTiles = 0;
    _curFlareTiles = 0;
    _curCannonTiles = 0;
    
    if(_freeIndexList.Count > 0)
        _freeIndexList.Clear();

    List<GameObject> mapList = new List<GameObject>();

    Vector3 startPos = transform.position - new Vector3(_mapSize*_tileSize/2,0, _mapSize * _tileSize / 2) + new Vector3(_tileSize/2,0,_tileSize/2);


    for (int i = 0; i < _mapSize * _mapSize; i++)
    {
        Vector3 tilePos = startPos + new Vector3(i % _mapSize * _tileSize,0, (int)(i / _mapSize) * _tileSize);
        GameObject gameTile = Instantiate(_gameTile, tilePos, Quaternion.identity) as GameObject;
        gameTile.GetComponentInChildren<GameTile>().ThisType = GameTile.TileType.EMPTY;
        gameTile.GetComponentInChildren<Renderer>().material.SetColor("_BorderColor", new Color(1, 0, 0,0));

        _freeIndexList.Add(i);

        mapList.Add(gameTile);
    }


    _freeIndexList.RemoveAt((_mapSize * _mapSize) / 2);
    //add all the treasure tiles

        while (_curTreasureTiles < _treasureTiles)
     {
         int randIndex = Random.Range(0, _freeIndexList.Count);
         int tileIndex = _freeIndexList[randIndex];
     
         if (mapList[tileIndex].GetComponentInChildren<GameTile>().IsEmpty)
         {
             mapList[tileIndex].GetComponentInChildren<GameTile>().HiddenObject = _thisManager.TreasureObjects[Random.Range(0, _thisManager.TreasureObjects.Count)]; //get a random event for this type of tile
             mapList[tileIndex].GetComponentInChildren<GameTile>().IsEmpty = false;
             mapList[tileIndex].GetComponentInChildren<GameTile>().name = "GOOD TILE";
             mapList[tileIndex].GetComponentInChildren<GameTile>().ThisType = GameTile.TileType.TREASURE;
             mapList[tileIndex].GetComponentInChildren<GameTile>().BorderColor = TreasureObjectMat;
             _freeIndexList.RemoveAt(randIndex);
     
             _curTreasureTiles++;
         }
     }
     

     while (_curBadTiles < _badTiles)
     {
         int randIndex = Random.Range(0, _freeIndexList.Count);
         int tileIndex = _freeIndexList[randIndex];

         if (mapList[tileIndex].GetComponentInChildren<GameTile>().IsEmpty)
         {
             mapList[tileIndex].GetComponentInChildren<GameTile>().HiddenObject = _thisManager.BadObjects[Random.Range(0, _thisManager.BadObjects.Count)]; //get a random event for this type of tile
             mapList[tileIndex].GetComponentInChildren<GameTile>().IsEmpty = false;
             mapList[tileIndex].GetComponentInChildren<GameTile>().name = "BAD TILE";
             mapList[tileIndex].GetComponentInChildren<GameTile>().ThisType = GameTile.TileType.BAD;
                mapList[tileIndex].GetComponentInChildren<GameTile>().BorderColor = BadObjectMat;
                _freeIndexList.RemoveAt(randIndex);

                _curBadTiles++;
         }
     }

        while (_curFlareTiles < _flareTiles)
        {
            int randIndex = Random.Range(0, _freeIndexList.Count);
            int tileIndex = _freeIndexList[randIndex];

            if (mapList[tileIndex].GetComponentInChildren<GameTile>().IsEmpty)
            {
                mapList[tileIndex].GetComponentInChildren<GameTile>().HiddenObject = _thisManager.FlareObjects[Random.Range(0, _thisManager.FlareObjects.Count)]; //get a random event for this type of tile
                mapList[tileIndex].GetComponentInChildren<GameTile>().IsEmpty = false;
                mapList[tileIndex].GetComponentInChildren<GameTile>().name = "FLARE TILE";
                mapList[tileIndex].GetComponentInChildren<GameTile>().ThisType = GameTile.TileType.FLARE;
                mapList[tileIndex].GetComponentInChildren<GameTile>().BorderColor = FlareObjectMat;
                _freeIndexList.RemoveAt(randIndex);

                _curFlareTiles++;
            }
        }

        while (_curImuneTiles < _imuneTiles)
        {
            int randIndex = Random.Range(0, _freeIndexList.Count);
            int tileIndex = _freeIndexList[randIndex];

            if (mapList[tileIndex].GetComponentInChildren<GameTile>().IsEmpty)
            {
                mapList[tileIndex].GetComponentInChildren<GameTile>().HiddenObject = _thisManager.ImuneObjects[Random.Range(0, _thisManager.ImuneObjects.Count)]; //get a random event for this type of tile
                mapList[tileIndex].GetComponentInChildren<GameTile>().IsEmpty = false;
                mapList[tileIndex].GetComponentInChildren<GameTile>().name = "IMUNE TILE";
                mapList[tileIndex].GetComponentInChildren<GameTile>().ThisType = GameTile.TileType.IMUNE;
                mapList[tileIndex].GetComponentInChildren<GameTile>().BorderColor = ImuneObjectMat;
                _freeIndexList.RemoveAt(randIndex);

                _curImuneTiles++;
            }
        }

        while (_curMapTiles < _mapTiles)
        {
            int randIndex = Random.Range(0, _freeIndexList.Count);
            int tileIndex = _freeIndexList[randIndex];

            if (mapList[tileIndex].GetComponentInChildren<GameTile>().IsEmpty)
            {
                mapList[tileIndex].GetComponentInChildren<GameTile>().HiddenObject = _thisManager.MapObjects[Random.Range(0, _thisManager.MapObjects.Count)]; //get a random event for this type of tile
                mapList[tileIndex].GetComponentInChildren<GameTile>().IsEmpty = false;
                mapList[tileIndex].GetComponentInChildren<GameTile>().name = "MAP TILE";
                mapList[tileIndex].GetComponentInChildren<GameTile>().ThisType = GameTile.TileType.MAP;
                mapList[tileIndex].GetComponentInChildren<GameTile>().BorderColor = MapObjectMat;
                _freeIndexList.RemoveAt(randIndex);

                _curMapTiles++;
            }
        }

        while (_curCannonTiles < _cannonTiles)
        {
            int randIndex = Random.Range(0, _freeIndexList.Count);
            int tileIndex = _freeIndexList[randIndex];

            if (mapList[tileIndex].GetComponentInChildren<GameTile>().IsEmpty)
            {
                mapList[tileIndex].GetComponentInChildren<GameTile>().HiddenObject = _thisManager.CannonObjects[Random.Range(0, _thisManager.CannonObjects.Count)]; //get a random event for this type of tile
                mapList[tileIndex].GetComponentInChildren<GameTile>().IsEmpty = false;
                mapList[tileIndex].GetComponentInChildren<GameTile>().name = "CANNON TILE";
                mapList[tileIndex].GetComponentInChildren<GameTile>().ThisType = GameTile.TileType.CANNON;
                mapList[tileIndex].GetComponentInChildren<GameTile>().BorderColor = CannonObjectMat;
                _freeIndexList.RemoveAt(randIndex);

                _curCannonTiles++;
            }
        }


        return mapList;
    }
}
