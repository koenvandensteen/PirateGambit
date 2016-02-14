//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class SwipeManager : MonoBehaviour {

    public GameObject SwipeEnd;
    public GameObject SwipeCenter;

    private Vector3 _swipeStart;
    private Vector3 _swipeEnd;

    private bool _isSwiping = false;
    public bool DoShow = true;

    public Color HiddenColor;
    private Color _baseColor;

    private GameObject[] _swipeElements = { null, null, null };

    // Use this for initialization
    void Awake() {
        _baseColor = SwipeCenter.GetComponent<Renderer>().sharedMaterial.GetColor("_Color");
    }

    // Update is called once per frame
    void Update() {

        if (!_isSwiping) {
            if (Input.GetMouseButtonDown(0)) {
                _swipeStart = _swipeEnd = Input.mousePosition;
                _isSwiping = true;
            }
        } else {
            if (!Input.GetMouseButton(0)) {
                _isSwiping = false;
                for (int i = 0; i < 3; i++) {
                    Destroy(_swipeElements[i]);
                }
            } else {
                if (!DoShow) {
                    for (int i = 0; i < 3; i++) {
                        Destroy(_swipeElements[i]);
                    }

                    return;
                }

                var camera = GetComponentInParent<Camera>();
                Plane plane = new Plane(-1 * camera.transform.forward, transform.position);

                var rayStart = camera.ScreenPointToRay(_swipeStart);
                var rayEnd = camera.ScreenPointToRay(Input.mousePosition);

                float deltaStart;
                float deltaEnd;
                plane.Raycast(rayStart, out deltaStart);
                plane.Raycast(rayEnd, out deltaEnd);

                Vector3 vStart = rayStart.origin + rayStart.direction * deltaStart;
                Vector3 vEnd = rayEnd.origin + rayEnd.direction * deltaEnd;

                UpdateUI(vStart, vEnd);

            }
        }


    }


    void UpdateUI(Vector3 start, Vector3 end) {
        //Create objects if they do not exist
        for (int i = 0; i < 3; i++) {
            if (_swipeElements[i] == null) {
                if (i == 0 || i == 2) {
                    _swipeElements[i] = Instantiate(SwipeEnd);
                } else {
                    _swipeElements[i] = Instantiate(SwipeCenter);
                }
                _swipeElements[i].transform.parent = transform;
            }
        }

        if (!DoShow) {
            for (int i = 0; i < 3; i++) {
                _swipeElements[i].GetComponent<Renderer>().material.SetColor("_Color", HiddenColor);
            }
        } else {
            for (int i = 0; i < 3; i++) {
                _swipeElements[i].GetComponent<Renderer>().material.SetColor("_Color", _baseColor);
            }

        }

        Vector3 center = (end + start) / 2;
        float angle = Vector3.Angle(Vector3.right, end - start);
        if (end.y < start.y) {
            angle *= -1;
        }

        float distance = Vector3.Distance(start, end);

        _swipeElements[0].transform.position = start;
        _swipeElements[0].transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        _swipeElements[2].transform.position = end;
        _swipeElements[2].transform.rotation = Quaternion.AngleAxis(angle + 180, Vector3.forward);

        _swipeElements[1].transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        _swipeElements[1].transform.localScale = new Vector3(distance, 1, 1);
        _swipeElements[1].transform.position = center;
    }
}
