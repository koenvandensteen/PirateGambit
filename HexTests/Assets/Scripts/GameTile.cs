//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameTile : MonoBehaviour {


    public bool IsEmpty = true;
    public GameObject HiddenObject;
    public Color BorderColor;
    public bool IsHidden = true;
    public bool IsActivated = false;
    public int BadNeighbours = 0;

    public GameObject FlagObject;

    public bool Highlighted {
        set {
            if (value) {
                GetComponent<Renderer>().material.SetFloat("_IsHighlighted", 1);
            } else {
                GetComponent<Renderer>().material.SetFloat("_IsHighlighted", 0);
            }
        }
    }
    public enum TileType {
        EMPTY,
        TREASURE,
        BAD,
        MAP,
        POWDERKEG,
        IMUNE,
        COUNT
    }

    public enum TileStatus {
        CLEAR = 0,
        FLAGGED_DANGER,
        SIZE
    }


    [System.Serializable]
    public class NamedColor {
        public string Name;
        public Color Color;
    }

    public NamedColor[] NamedColors;

    private Dictionary<string, Color> _colors = new Dictionary<string, Color>();

    public int Q, R;

    private TileStatus _currentStatus = TileStatus.CLEAR;
    public TileStatus CurrentTileStatus {
        get {
            return _currentStatus;
        }
        set {
            value = (TileStatus)((int)value % (int)TileStatus.SIZE);
            _currentStatus = (TileStatus)((int)value % (int)TileStatus.SIZE);

            Transform statusObject = transform.Find("Status");

            switch (_currentStatus) {
                case TileStatus.CLEAR:
                    ClearStatus();
                    break;
                case TileStatus.FLAGGED_DANGER:
                    {
                        ClearStatus();
                        var obj = Instantiate(FlagObject, transform.position, Quaternion.identity) as GameObject;
                        obj.transform.parent = statusObject;
                        obj.name = "FLAG";
                        var renderer = obj.GetComponentInChildren<Renderer>();
                        Color color = Color.white;
                        if (!_colors.TryGetValue("Danger", out color)) {
                            Debug.Log("GameTile::_colors >> Color Danger not found!");
                        }
                        renderer.materials[1].SetColor("_Color", color);
                        break;
                    }
                case TileStatus.SIZE:
                    break;
                default:
                    break;
            }

        }
    }

    void ClearStatus() {
        Transform statusObject = transform.Find("Status");
        for (int i = 0; i < statusObject.childCount; i++) {
            Destroy(statusObject.GetChild(i).gameObject);
        }

    }

    public TileType ThisType = TileType.EMPTY;

    public void ShowObject() {

        if (IsActivated || !IsHidden /* || GameManager.ThisManager.state != GameState.Play*/)
            return;

        if (SceneManager.GetActiveScene().name == "HexMap")
            --GameManager.Instance.HiddenTiles;

        if (IsEmpty) {
            GetComponent<Renderer>().material.SetColor("_BorderColor", BorderColor);
            IsHidden = false;
            return;
        } else {
            HiddenObject = Instantiate(HiddenObject, transform.position + new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            HiddenObject.transform.parent = transform;
            GetComponent<Renderer>().material.SetColor("_BorderColor", BorderColor);
            IsHidden = false;
        }
    }

    public void ActivateTile() {

        if (IsActivated)
            return;

        IsActivated = true;

        if (IsEmpty) {
            GetComponent<Renderer>().material.SetColor("_BorderColor", BorderColor);
            GetComponent<Renderer>().material.SetFloat("_IsActivated", 1);

            IsActivated = true;
            return;
        } else {
            if (IsHidden) {
                IsHidden = false;
                HiddenObject = Instantiate(HiddenObject, transform.position + new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                GetComponent<Renderer>().material.SetColor("_BorderColor", BorderColor);
                HiddenObject.transform.parent = transform;
            }

            GetComponent<Renderer>().material.SetFloat("_IsActivated", 1);

        }

        if (ThisType != TileType.BAD)
            DestroyObject(HiddenObject);
    }

    public void ShowEmphasis(float seconds) {
        //GetComponent<Renderer>().material.SetFloat("_EmphasisTime", 0);

        StartCoroutine(DeactivateEmphasis(seconds));
    }

    private IEnumerator DeactivateEmphasis(float seconds) {
        GetComponent<Renderer>().material.SetFloat("_IsEmphasis", 1);
        yield return new WaitForSeconds(seconds);
        GetComponent<Renderer>().material.SetFloat("_IsEmphasis", 0);
    }

    public TileStatus TileCliked() {
        CurrentTileStatus++;
        return CurrentTileStatus;
    }

    // Use this for initialization
    void Awake() {
        _colors.Clear();
        foreach (var item in NamedColors) {
            _colors.Add(item.Name, item.Color);
        }
    }

    // Update is called once per frame
    void Update() {

    }

    void OnValidate() {
        //if (Application.loadedLevelName == "MainMenu")
        //    return;

        //_colors.Clear();
        //foreach (var item in NamedColors) {
        //    if (item.Name != string.Empty && item.Color != null) {
        //        _colors.Add(item.Name, item.Color);
        //    }
        //}


    }

}