//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MapGenerator : MonoBehaviour
{



    public GameObject HexTile;
    private List<Vector2> _emptyTileList = new List<Vector2>();

    private List<List<GameObject>> HiddenObjectList = new List<List<GameObject>>();

    public int[] MaxTileAmmountEasy = new int[(int)GameTile.TileType.COUNT - 1];
    public int[] MaxTileAmmountMedium = new int[(int)GameTile.TileType.COUNT - 1];
    public int[] MaxTileAmmountHard = new int[(int)GameTile.TileType.COUNT - 1];

    private int[] _curTileAmmount = new int[(int)GameTile.TileType.COUNT - 1];
    public Color[] BorderColors = new Color[(int)GameTile.TileType.COUNT - 1];



    public int VariableRandOffset = 3;

    [HideInInspector]
    public int TreasureAmount;

    // Use this for initialization
    void Awake()
    {
        LoadAssets();

    }

    void LoadAssets()
    {

        int assetListIndex = 0;

        //load treasure objects
        var treasureObjects = Resources.LoadAll("TreasureObjects");
        HiddenObjectList.Add(new List<GameObject>());
        foreach (var obj in treasureObjects)
        {
            HiddenObjectList[assetListIndex].Add(obj as GameObject);
        }
        assetListIndex++;
        //load bad objects

        var badObjects = Resources.LoadAll("BadObjects");
        HiddenObjectList.Add(new List<GameObject>());
        foreach (var obj in badObjects)
        {
            HiddenObjectList[assetListIndex].Add(obj as GameObject);
        }
        assetListIndex++;

        //load map objects
        var mapObjects = Resources.LoadAll("MapObjects");
        HiddenObjectList.Add(new List<GameObject>());
        foreach (var obj in mapObjects)
        {
            HiddenObjectList[assetListIndex].Add(obj as GameObject);
        }
        assetListIndex++;

        //load PowderkegObjects
        var flareObjects = Resources.LoadAll("PowderkegObjects");
        HiddenObjectList.Add(new List<GameObject>());
        foreach (var obj in flareObjects)
        {
            HiddenObjectList[assetListIndex].Add(obj as GameObject);
        }
        assetListIndex++;

        //load imune objects
        var imuneObjects = Resources.LoadAll("ImuneObjects");
        HiddenObjectList.Add(new List<GameObject>());
        foreach (var obj in imuneObjects)
        {
            HiddenObjectList[assetListIndex].Add(obj as GameObject);
        }
        assetListIndex++;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public HexMap CreateHexMap(int mapSize, float hexRadius)
    {
        int differentTyleTypes = (int)GameTile.TileType.COUNT - 1;

        for (int i = 0; i < differentTyleTypes; i++)
        {
            _curTileAmmount[i] = 0;
        }

        if (_emptyTileList.Count > 0)
            _emptyTileList.Clear();

        HexMap gameMap = new HexMap();


        List<Vector2> hiddenTiles = new List<Vector2>();
        float heigth = hexRadius * 2f;
        float width = Mathf.Sqrt(3f) / 2f * heigth;

        Vector3 startOffset = new Vector3(-(mapSize * width) / 4f + width / 4f, 0, (mapSize * heigth * 3 / 4) / 2f - (heigth * 3 / 4) / 2);

        int count = 0;
        int curXoffset = 1;
        int curIndex = 0;
        int counter = 0;


        int Q = 0, R = 0;

        for (int r = 0; r < mapSize; r++)
        {
            var newRow = new List<GameObject>();

            for (int q = 0; q < mapSize / 2 + curXoffset; q++)
            {
                Vector3 SpawnPos = startOffset + new Vector3((width * q - (curXoffset - 1) * width / 2), 0, -heigth * 3 / 4 * r);

                var tile = Instantiate(HexTile, SpawnPos, Quaternion.identity) as GameObject;

                tile.GetComponent<GameTile>().BorderColor = BorderColors[(int)GameTile.TileType.EMPTY];
                tile.GetComponent<GameTile>().IsEmpty = true;

                int indexX;

                if (r <= mapSize / 2)
                    indexX = q - r;
                else
                {
                    indexX = q - r + counter;
                }

                int indexY = r - mapSize / 2;


                tile.name = string.Format("X: {0} Y: {1}", indexX, indexY);

                tile.transform.parent = transform;
                tile.GetComponent<GameTile>().Q = indexX;
                tile.GetComponent<GameTile>().R = indexY;

                if (indexX == 0 && indexY == 0)
                {

                }
                else
                {
                    _emptyTileList.Add(new Vector2(q, r));
                    hiddenTiles.Add(new Vector2(indexX, indexY));
                }

                newRow.Add(tile);

                ++count;
                ++Q;


                tile.transform.parent = transform;

            }

            if (r < mapSize / 2)
                ++curXoffset;
            else
                --curXoffset;

            gameMap.AddRow(newRow);
            ++R;
            if (R > mapSize / 2)
                ++counter;

            curIndex--;
        }

        int ammountOfTiles = _emptyTileList.Count;


        int[] maxTileAmmount = MaxTileAmmountEasy;

        switch (DifficultyStateObject.CurDifficultyState)
        {
            case DifficultyStateObject.DifficultyState.Easy:
                maxTileAmmount = MaxTileAmmountEasy;
                break;
            case DifficultyStateObject.DifficultyState.Medium:
                maxTileAmmount = MaxTileAmmountMedium;
                break;
            case DifficultyStateObject.DifficultyState.Hard:
                maxTileAmmount = MaxTileAmmountHard;
                break;
        }

        for (int i = 0; i < maxTileAmmount.Length; ++i)
        {
            maxTileAmmount[i] += Random.Range(0, VariableRandOffset);
        }

        if (SceneManager.GetActiveScene().name == "HexMap")
        {
            GameManager.ThisManager.GetComponent<GameManager>().HiddenTileList = new List<Vector2>(hiddenTiles);
        }

        for (int i = 0; i < differentTyleTypes; i++)
        {
            while (_curTileAmmount[i] < ammountOfTiles / 100f * maxTileAmmount[i])
            {
                int randIndex = Random.Range(0, _emptyTileList.Count);
                Vector2 tileIndex = _emptyTileList[randIndex];

                if (gameMap.GetRow((int)tileIndex.y)[(int)tileIndex.x].GetComponentInChildren<GameTile>().IsEmpty)
                {
                    gameMap.GetRow((int)tileIndex.y)[(int)tileIndex.x].GetComponent<GameTile>().HiddenObject = HiddenObjectList[i][Random.Range(0, HiddenObjectList[i].Count)];
                    gameMap.GetRow((int)tileIndex.y)[(int)tileIndex.x].GetComponent<GameTile>().IsEmpty = false;
                    gameMap.GetRow((int)tileIndex.y)[(int)tileIndex.x].GetComponent<GameTile>().ThisType = (GameTile.TileType)i + 1;
                    gameMap.GetRow((int)tileIndex.y)[(int)tileIndex.x].GetComponent<GameTile>().BorderColor = BorderColors[i + 1];

                    if (i == (int)GameTile.TileType.BAD - 1)
                        ++GameManager.ThisManager.GetComponent<GameManager>().MaxKrakenAmmount;

                    if (i == (int)GameTile.TileType.TREASURE - 1)
                        ++GameManager.ThisManager.GetComponent<GameManager>().MaxTreasureAmmount;

                    _emptyTileList.RemoveAt(randIndex);
                    _curTileAmmount[i]++;
                }
            }
        }

        return gameMap;
    }



}
