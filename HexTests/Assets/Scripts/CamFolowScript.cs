//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class CamFolowScript : MonoBehaviour {


    public float CamSpeed;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(GameManager.Instance.LookAtTarget.x,transform.position.y, GameManager.Instance.LookAtTarget.z), CamSpeed * Time.deltaTime);
    }
}
