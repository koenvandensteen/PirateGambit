//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Immunity_Parent : MonoBehaviour {


    private List<Immunity_Child> children;

    // Use this for initialization
    void Awake() {
        children = new List<Immunity_Child>(GetComponentsInChildren<Immunity_Child>());

        foreach (var item in children) {
            item.gameObject.SetActive(false);
        }
        GameManager.ThisManager.RumlevelChangedImplementation += UpdateBottles;
    }


    void UpdateBottles(int value) {

        int amount = Mathf.Min(value, children.Count);

        int activeObjects = 0;
        foreach (var child in children) {
            if (child.gameObject.activeSelf) {
                ++activeObjects;
            }
        }

        int index = 0;
        while (activeObjects < amount) {
            if (!children[index].gameObject.activeSelf) {
                children[index].gameObject.SetActive(true);
                ++activeObjects;
            }
            ++index;
        }
    }
}
