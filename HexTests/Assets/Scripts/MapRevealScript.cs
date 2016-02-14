//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class MapRevealScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z);

    }

    // Update is called once per frame
    void Update () {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + 1 * Time.deltaTime, transform.localScale.z);
	}
}
