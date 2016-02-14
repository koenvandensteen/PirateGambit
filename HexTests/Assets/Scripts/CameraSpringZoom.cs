//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.SocialPlatforms;

public class CameraSpringZoom : MonoBehaviour
{
    public Vector3 CamOffset = new Vector3(0, 0, -3);
       
    public float CamStiffness = 0.5f;
    public float MinZoom = 10f;
    public float MaxZoom = 0.1f;

    public float ZoomSpeed = 1f;
    public float AngleSpeed = 1f;
    public Transform PlayerTransform;
    private Transform _camTransform;

    public float CamOffsetAmmount = 1;
    public float CamAngleOffsetSpeed = 1f;
    private float _camAngleOffset = 5f;
    public float CameraMoveSpeed = 10f;
    private float _prevMagnitude = 0f;
    private float _zoomFactor = 0;
    public Vector2 LeftTopBound;
    public Vector2 RightBotBound;
    private Vector3 _maxZoomedInPosition;
    private Vector3 _startPosition;

    public Vector3 EasyStartPos;
    public Vector3 MediumStartPos;
    public Vector3 HardStartPos;

    // Use this for initialization
    void Start()
    {
        _camTransform = GetComponentInChildren<Camera>().transform;

        if (GameManager.IsMobile)
        {
            ZoomSpeed /= 3f;
        }


        if (DifficultyStateObject.CurDifficultyState == DifficultyStateObject.DifficultyState.Easy)
        {
            _camTransform.position = EasyStartPos;
        }
        else if (DifficultyStateObject.CurDifficultyState == DifficultyStateObject.DifficultyState.Medium)
        {
            _camTransform.position = MediumStartPos;
        }
        else if (DifficultyStateObject.CurDifficultyState == DifficultyStateObject.DifficultyState.Hard)
        {
            _camTransform.position = HardStartPos;
        }

        _startPosition = _camTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        _maxZoomedInPosition = PlayerTransform.position + new Vector3(0, 3.2f, -2.2f);
        _camTransform.position = Vector3.Lerp(_startPosition, _maxZoomedInPosition, _zoomFactor);
        
        if (GameManager.IsMobile)
        {
            if (Input.touchCount < 2)
                return;

            var deltaVec = Input.touches[0].position - Input.touches[1].position;

            if (_prevMagnitude == 0)
                _prevMagnitude = deltaVec.magnitude;

           var  dif = deltaVec.magnitude - _prevMagnitude;
            
            if (dif > 0)
            {
                ZoomIn();
            }
            else if (dif < 0)
            {
                ZoomOut();
            }

            _prevMagnitude = deltaVec.magnitude;
        }
        else
        {
            float zoom = Input.GetAxis("Zoom");

            if (zoom > 0)
            {
                ZoomIn();
            }
            else if (zoom < 0)
            {
                ZoomOut();
            }
        }
    }

    void ZoomIn()
    {      
        _zoomFactor += Time.deltaTime * ZoomSpeed;
        if (_zoomFactor >=  1)
            _zoomFactor = 1;
    }

    void ZoomOut()
    {
       _zoomFactor -= Time.deltaTime * ZoomSpeed;
        if (_zoomFactor <= 0)
            _zoomFactor = 0;
    }
}
