//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class ItemBopScript : MonoBehaviour
{
    Vector3 _initialPosition;

    public float BopSpeed = 0.1f;
    public float BopStrength = .2f;

    public Vector3 RotationAxis = Vector3.up;
    public float RotationSpeed = 160;
    // Use this for initialization
    void Start()
    {
        _initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 displacement = Vector3.zero;
        displacement.y = Mathf.Sin(Time.time * BopSpeed) * BopStrength;

        transform.position = _initialPosition + displacement;
        transform.Rotate(RotationAxis, RotationSpeed * Time.deltaTime);
    }
}
