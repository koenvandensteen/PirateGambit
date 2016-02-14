//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraAnchor : MonoBehaviour {

    public enum AnchorPosition {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public AnchorPosition Position;
    public float DistanceFromCamera;
    public Camera CameraObject;

    void Update() {
        if (CameraObject == null) {
            CameraObject = transform.parent.GetComponent<Camera>();
        }

        if (CameraObject == null) {
            return;
        }

        if (CameraObject.orthographic) {
            float xSign = 1;
            float ySign = 1;

            switch (Position) {
                case AnchorPosition.TopLeft:
                    xSign = -1;
                    ySign = 1;
                    break;
                case AnchorPosition.TopRight:
                    xSign = 1;
                    ySign = 1;
                    break;
                case AnchorPosition.BottomLeft:
                    xSign = -1;
                    ySign = -1;
                    break;
                case AnchorPosition.BottomRight:
                    xSign = 1;
                    ySign = -1;
                    break;
                default:
                    break;
            }


            float aspect = CameraObject.aspect;
            float size = CameraObject.orthographicSize;


            transform.position = new Vector3(xSign * size * aspect, ySign * size, DistanceFromCamera) + CameraObject.transform.position;
        } else {
            float xSign = 1;
            float ySign = 1;

            switch (Position) {
                case AnchorPosition.TopLeft:
                    xSign = 0;
                    ySign = 1;
                    break;
                case AnchorPosition.TopRight:
                    xSign = 1;
                    ySign = 1;
                    break;
                case AnchorPosition.BottomLeft:
                    xSign = 0;
                    ySign = 0;
                    break;
                case AnchorPosition.BottomRight:
                    xSign = 1;
                    ySign = 0;
                    break;
                default:
                    break;
            }

            transform.position = CameraObject.ViewportToWorldPoint(new Vector3(xSign , ySign, DistanceFromCamera));
        }
    }
}
