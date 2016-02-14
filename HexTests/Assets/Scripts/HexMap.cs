using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HexMap : MonoBehaviour {

    
    private List<List<GameTile>> _hexMap;
    

    

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AddRow(List<GameTile> row)
    {
        _hexMap.Add(row);
    }

    //public void AddCol(GameTile tile)
    //{
        
    //}




}
