//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class BarrelRevealScript : MonoBehaviour {

    private bool _isPlaying = false;
    public float PlayTime;
    public float ScaleDiff;

    // Use this for initialization
    void Start() {
       
    }

    // Update is called once per frame
    void Update() {
        if (_isPlaying) {
            PlayTime -= Time.deltaTime;
            transform.localScale *= ScaleDiff;
            if (PlayTime <= 0) {
                Destroy(gameObject);
            }
        }
    }

    public void Play() {
        _isPlaying = true;
    }
}
