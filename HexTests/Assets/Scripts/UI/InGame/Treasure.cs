//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
public class Treasure : MonoBehaviour {

    public Mesh[] Meshes;

    void Awake() {
        GameManager.ThisManager.TreasureChangedImplementation += UpdateCoins;

    }

    void UpdateCoins(int value) {
        float percentage = ((float)value) / GameManager.ThisManager.MaxTreasureAmmount;

        int index = Mathf.FloorToInt(percentage * Meshes.Length);

        index = Mathf.Min(index, Meshes.Length - 1);
        GetComponent<MeshFilter>().mesh = Meshes[index];

    }
}
