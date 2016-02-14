//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlashColor : MonoBehaviour {

    public Color Color1;
    public Color Color2;
    public float FlashTime = 1;
    private Graphic _graphic;
    private Renderer _renderer;

    void Awake() {
        _graphic = GetComponent<Graphic>();
        _renderer = GetComponentInChildren<Renderer>();
    }

    void Update() {
        if (FlashTime == 0)
            return;

        float t = Mathf.PingPong(Time.time / FlashTime, 1);



        if (_graphic != null)
            _graphic.color = Color.Lerp(Color1, Color2, t);
        if (_renderer != null) {
            Color color = _renderer.material.GetColor("_Color");
            color.a = Color.Lerp(Color1, Color2, t).a;
            _renderer.material.SetColor("_Color", color);
        }
    }
}
