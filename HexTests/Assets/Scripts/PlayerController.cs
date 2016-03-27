using System;
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private static readonly PlayerController instance = new PlayerController();
    private PlayerController() { }
    public static PlayerController Instance
    {
        get
        {
            return instance;
        }
    }

    public float SwipeMoveTreshold = 1f;

    private bool _isSwiping = false;
    private Vector3 _startSwipePos;
    private Vector3 _endSwipePos;


    // Use this for initialization
    void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (Input.touchCount <= 1 && !_isSwiping)
	    {
	        _isSwiping = true;
            StartSwipe();
	    }
        else if (Input.touchCount >= 0)
        {
            _isSwiping = false;
            EndSwipe();
        }             
	}

    private void StartSwipe()
    {
        _startSwipePos = Input.GetTouch(0).position;
    }

    private void EndSwipe()
    {
        _endSwipePos = Input.GetTouch(0).position;

        if (Vector3.Distance(_startSwipePos, _endSwipePos) < SwipeMoveTreshold)
        {
            var dir = _endSwipePos - _startSwipePos;

        }
    }

}
