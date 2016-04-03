using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HexMap
{

    
    private List<List<GameObject>> _hexMap = new List<List<GameObject>>();
    private int _curMapSize = 0;

    private readonly Vector2[] _neighBours = { new Vector2(+1, 0), new Vector2(0, +1), new Vector2(-1, +1), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(+1, -1) };

    public void AddRow(List<GameObject> row)
    {
        _hexMap.Add(row);
        ++_curMapSize;
    }

    public List<GameObject> GetRow(int y)
    {
        return _hexMap[y];
    }

    //Get tile with axial coordinates
    public GameObject GetTile(Vector2 pos)
    {
        int indexX = (int)pos.y + _curMapSize / 2;
        int indexY = (int)pos.x + _curMapSize / 2 + Mathf.Min(0, (int)pos.y);

        if ((indexX < _hexMap.Count && indexX >= 0) && (indexY < _hexMap[indexX].Count && indexY >= 0))
            return _hexMap[indexX][indexY];

        return null;
    }

    public void CheckNeighbours()
    {
        int q = 0, r = 0;
        int counter = 0;
        foreach (var tileList in _hexMap)
        {
            foreach (var tile in tileList)
            {
                int indexX;

                if (r <= _curMapSize / 2)
                    indexX = q - r;
                else
                {
                    indexX = q - r + counter;
                }

                int indexY = r - _curMapSize / 2;

                //tile.name = string.Format("X: {0} Y: {1}", indexX, indexY);

                var curTile = new Vector2(indexX, indexY);

                foreach (var dir in _neighBours)
                {
                    var neighbour = GetTile(curTile + dir);
                    if (neighbour)
                    {
                        if (neighbour.GetComponent<GameTile>().ThisType == GameTile.TileType.BAD)
                            tile.GetComponent<GameTile>().BadNeighbours++;
                    }
                }
                ++q;
            }
            ++r;

            if (r > _curMapSize / 2)
                ++counter;

            q = 0;
        }
    }

    public List<GameObject> GetNeighBours(Vector2 curpos)
    {
        var neighbourList = new List<GameObject>();

        foreach (var dir in _neighBours)
        {
            var neighbour = GetTile(curpos + dir);
            if (neighbour)
            {
                neighbourList.Add(neighbour);
            }
        }

        return neighbourList;
    } 

    public void LowerNeighbourDangerCounter(Vector2 curPos)
    {
        foreach (var dir in _neighBours)
        {
            var neighbour = GetTile(curPos + dir);
            if (neighbour)
            {
                --neighbour.GetComponent<GameTile>().BadNeighbours;
            }
        }
    }

    public GameObject GetClickedTile(Vector2 clickPos)
    {
        return GetTile(RoundHex(clickPos));
    }

    public void ClearMap()
    {
        _hexMap.Clear();
        //foreach (var tileList in _hexMap)
        //{
        //    foreach (var tile in tileList)
        //    {
               
        //    }
        //}
    }

    private Vector2 RoundHex(Vector2 coordinate)
    {
        //    function cube_round(h):
        var cubeCoord = AxialToCubeCoord(coordinate);

        int rx = Mathf.RoundToInt(cubeCoord.x);
        int ry = Mathf.RoundToInt(cubeCoord.y);
        int rz = Mathf.RoundToInt(cubeCoord.z);

        var x_diff = Mathf.Abs(rx - cubeCoord.x);
        var y_diff = Mathf.Abs(ry - cubeCoord.y);
        var z_diff = Mathf.Abs(rz - cubeCoord.z);

        if (x_diff > y_diff && x_diff > z_diff)
            rx = -ry - rz;
        else if (y_diff > z_diff)
            ry = -rx - rz;
        else
            rz = -rx - ry;

        return CubetoAxialCoord(rx, ry, rz);
    }

    private Vector3 AxialToCubeCoord(Vector2 coordinate)
    {
        return new Vector3(coordinate.x, -coordinate.x - coordinate.y, coordinate.y);
    }

    private Vector2 CubetoAxialCoord(float x, float y, float z)
    {
        return new Vector2(x, z);
    }


}
