using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    private static PlayerController _instance;

    public static PlayerController Instance
    {
        get { return _instance; }
        private set { }
    }

    public float SwipeMoveTreshold = 1f;
    private bool _isSwiping = false;
    private Vector3 _startSwipePos;
    private Vector3 _endSwipePos;
    private Plane _raycastPlane = new Plane(Vector3.up, Vector3.zero);

    private readonly List<Vector2> _directionlist = new List<Vector2> {
        new Vector2(+1, -1),
        new Vector2(+1, 0),
        new Vector2(0, +1),
        new Vector2(-1, +1),
        new Vector2(-1, 0),
        new Vector2(0, -1)
    };

    private GameTile _highlightedTile;

    public float TimeBetweenRightClick = 0.1f;
    private float _rightClickCounter = 0;


    void awake()
    {
        if (_instance == null)
            _instance = gameObject.GetComponent<PlayerController>();

        _rightClickCounter = TimeBetweenRightClick;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ////
        /// Mobile Only
        /// 

        //if (Input.touchCount >= 1 && !_isSwiping)
        //{
        //    _isSwiping = true;
        //    StartSwipe();
        //}
        //else if (Input.touchCount >= 1 && !_isSwiping)
        //{
        //    _isSwiping = true;
        //    StartSwipe();
        //}
        //else if (Input.touchCount >= 0)
        //{
        //    _isSwiping = false;
        //    EndSwipe();
        //}

        ///
        /// Desktop only
        /// 

        if (!GameManager.Instance._curPlayer.IsMoving)
        {
            ProccesMouse();
            if (_rightClickCounter <= 0)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    ProcessRightMouseClick();
                    _rightClickCounter = TimeBetweenRightClick;
                }
            }
        }

        if (_rightClickCounter > 0)
        {
            _rightClickCounter -= Time.deltaTime;
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
            var hexDirection = GetHexDirection(_endSwipePos- _startSwipePos);
            var moveTarget = GameManager.Instance.GameMap.GetTile(GameManager.Instance._curPos + hexDirection);
            if (moveTarget)
            {
               GameManager.Instance._curPlayer.SetMoveStart(moveTarget.transform.position);
               GameManager.Instance._curPos += hexDirection;
            }
        }
    }


    private void ProccesMouse()
    {
        if (_highlightedTile)
        {
            _highlightedTile.Highlighted = false;
            _highlightedTile = null;
        }

        var curMousePos = GetmousePosition();
        Vector2 vectorDir = new Vector2((curMousePos.x - GameManager.Instance._curPlayer.transform.position.x), (curMousePos.z - GameManager.Instance._curPlayer.transform.position.z));

        if (vectorDir.magnitude > 1 && vectorDir.magnitude < 2.5)
        {
            Vector2 moveOffset = GetHexDirection(vectorDir);
            var moveTarget = GameManager.Instance.GameMap.GetTile(GameManager.Instance._curPos + moveOffset);

            if (moveTarget)
            {
                if (moveTarget.GetComponent<GameTile>().CurrentTileStatus == GameTile.TileStatus.CLEAR)
                {
                    _highlightedTile = moveTarget.GetComponent<GameTile>();
                    _highlightedTile.Highlighted = true;
                }

                if (Input.GetMouseButtonUp(0))
                {

                    if(moveTarget.GetComponent<GameTile>().IsHidden)
                        moveTarget.GetComponent<GameTile>().ShowObject();

                    if (_highlightedTile)
                    {
                        _highlightedTile.Highlighted = false;
                        _highlightedTile = null;
                    }

                    GameManager.Instance._curPlayer.SetMoveStart(moveTarget.transform.position);
                    GameManager.Instance._curPos += moveOffset;
                }

            }
        }
    }

    void ProcessRightMouseClick()
    {
        var clickedPos = GetmousePosition();

        float x = clickedPos.x * Mathf.Sqrt(3f) / 3f - (-clickedPos.z) / 3f;
        float y = (-clickedPos.z) * 2f / 3f;

        var tile = GameManager.Instance.GameMap.GetClickedTile(new Vector2(x, y));

        if (tile != null)
        {
            tile.GetComponent<GameTile>().CurrentTileStatus++;
            if (!tile.GetComponent<GameTile>().IsHidden)
                return;
            if (tile.GetComponent<GameTile>().CurrentTileStatus == GameTile.TileStatus.FLAGGED_DANGER)
            {
                ++GameManager.Instance.CurKrakenAmmount;
            }
            else
            {
                --GameManager.Instance.CurKrakenAmmount;
            }
        }

    }

    Vector2 GetHexDirection(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.x, direction.y)*180/Mathf.PI;
        if (angle < 0)
            angle += 360;

        return _directionlist.ElementAt((int) (angle/6));
    }

    Vector2 GetHexDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.x, direction.y) * 180 / Mathf.PI;
        if (angle < 0)
            angle += 360;

        return _directionlist.ElementAt((int)(angle / 60));
    }

    Vector3 GetmousePosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float delta = 0;
        _raycastPlane.Raycast(ray, out delta);

        return (ray.origin + ray.direction * delta);
    }

    //this is for clicking and gettign direction 
    /*
    Vector2 GetHexDirection()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        float rayDistance = 0;
        _raycastPlane.Raycast(ray, out rayDistance);

        var touchedPos = ray.origin + ray.direction * rayDistance;
        var vectorDir = new Vector2((touchedPos.x - GameManager.Instance._curPlayer.transform.position.x), (touchedPos.z - GameManager.Instance._curPlayer.transform.position.z));


        float angle = Mathf.Atan2(vectorDir.x, vectorDir.y) * 180 / Mathf.PI;
        if (angle < 0)
            angle += 360;

        int lockedAngle = Mathf.FloorToInt(angle * (6.0f / 360.0f)) * 60 + 30;

        return _directionlist.ElementAt(lockedAngle);
    }
    */
}
