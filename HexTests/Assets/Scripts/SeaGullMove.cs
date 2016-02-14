//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class SeaGullMove : MonoBehaviour
{



    public float MoveMentSpeed;
    public float Frequency;
    public float Magnitude;

    private float _heightDisplacement;
	// Use this for initialization
	void Start ()
	{
	    var direction = -transform.position;
	    float angle = Mathf.Atan2(direction.x, direction.z) * 57.2957795f;
	    angle += Random.Range(-1f, 1f);
        transform.rotation = Quaternion.Euler(0,angle,0);
	    _heightDisplacement += Random.Range(0f,1f);
	    Frequency += Random.Range(-0.5f, 0.5f);
    }

	
	// Update is called once per frame
	void Update ()
	{
	    _heightDisplacement += Time.deltaTime;

        transform.position += transform.forward*MoveMentSpeed*Time.deltaTime;
	    transform.position += transform.up*Mathf.Sin(_heightDisplacement*Frequency)*Magnitude;

	}
}
