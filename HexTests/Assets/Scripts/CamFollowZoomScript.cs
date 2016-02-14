//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class CamFollowZoomScript : MonoBehaviour {

    public Transform Target;

    public Vector3 NeutralPos;

    public float _cameraDistance = -20;
    public Transform _cameraBeam;

    private float _zoomFactor = 0.0f;
    public float ZoomSpeed;
    public float TargetZoom = 0.5f;

    private Quaternion _startRotation;
    public Vector3 RotationPerSec;

    public enum State {
        NEUTRAL,
        TRACKING,
        ROTATING
    }

    private State _cameraState;
    public State CameraState {
        get {
            return _cameraState;
        }
        set {
            if (value == State.TRACKING) {

            } else if (value == State.ROTATING) {
                StartCoroutine("Zoom");
            }
            _cameraState = value;
        }
    }

    private float _previousZoom;

    // Use this for initialization
    void Awake() {
        if (_cameraBeam == null)
            _cameraBeam = GetComponentInChildren<Camera>().transform;
        //_cameraDistance = _cameraBeam.transform.position.z;
        _startRotation = transform.rotation;
        CameraState = State.TRACKING;
        GetComponentInChildren<Camera>().depth = 1;
    }

    // Update is called once per frame
    void Update() {

        switch (CameraState) {
            case State.NEUTRAL:
                break;
            case State.TRACKING:
                transform.position = Target.position;

                float zoom = Input.GetAxis("Zoom");
                if (GameManager.IsMobile)
                {
                    zoom = ProcessZoom();
                }
                _zoomFactor += zoom;
                _zoomFactor = Mathf.Clamp(_zoomFactor, 0.0f, TargetZoom);

                break;
            case State.ROTATING: 
                transform.Rotate(RotationPerSec * Time.deltaTime, Space.World);
                break;
            default:
                break;

        }

        _cameraBeam.localPosition = Vector3.forward * Mathf.Lerp(_cameraDistance, 0, _zoomFactor);
        transform.position = Vector3.Lerp(NeutralPos, Target.position, _zoomFactor * (1/TargetZoom));

    }


    float ProcessZoom()
    {
        if (Input.touchCount < 2)
            return 0;

        float zoomFactor = 0;

        var deltaVec = Input.touches[0].position - Input.touches[1].position;
        if (_previousZoom == 0)
            _previousZoom = deltaVec.magnitude;

         zoomFactor = deltaVec.magnitude - _previousZoom;
        _previousZoom = deltaVec.magnitude;

        return zoomFactor*0.01f*Time.deltaTime;
    }


    public void ResetCamera() {
        transform.rotation = _startRotation;
        CameraState = State.TRACKING;
        StopCoroutine("Zoom");
        _zoomFactor = 0.0f;
    }

    private IEnumerator Zoom() {
        while (Mathf.Abs(TargetZoom - _zoomFactor) > 0.01f) {
            _zoomFactor += Mathf.Sign(TargetZoom - _zoomFactor) * ZoomSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
