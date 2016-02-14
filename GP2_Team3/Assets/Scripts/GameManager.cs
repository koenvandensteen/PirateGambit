using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;


public class GameManager : MonoBehaviour {

    [System.Serializable]
    public struct Ship
    {
        //Movement variables
        public float MovementSpeed;
        public float RotSlowDown;
        public float MovSlowDown;
        public float RotationSpeed;
        public float PlayerHeigth;
        public float RotateOffset;
    }

    // Use this for initialization
    public Camera MainCamera;
    

    //map specific variables
    public List<int> FreeIndexList;
    public List<GameObject> MapList;
    public GameObject TileObject;
    public int MapSize = 10;
    public float TileSize = 5.0f;

    public bool IsPercentage = false;
    public int BadTiles;
    public int TreasureTiles;
    public int MapTiles;
    public int MapTilePower = 5;
    public int ImuneTiles;
    public int FlareTiles;
    public int CannonTiles;

    public List<GameObject> TreasureObjects = new List<GameObject>();
    public List<GameObject> EmptyObjects = new List<GameObject>();
    public List<GameObject> BadObjects = new List<GameObject>();
    public List<GameObject> ImuneObjects = new List<GameObject>();
    public List<GameObject> FlareObjects = new List<GameObject>();
    public List<GameObject> MapObjects = new List<GameObject>();
    public List<GameObject> CannonObjects = new List<GameObject>();

    public Material TileMouseOverMaterial;

    //game specific variables
    public Ship ShipStats;

    public GameObject PlayerShipObject;
    public CannonballControls CannonController;
    public static GameManager ThisManager;
    public float ScoreValue = 10;
    private bool _isPlayerImune = false;
    //private bool _isFirstTurn = true;


    public int PlayerCurIndex;
    private GameObject _curPLayerShip;

    public int CannonCurIndex;

    //camera controlls
    private bool _ZoomIn = true;
    private bool _ZoomOut = false;

    //game states
    public enum GameState
    {
        MENU,
        PLAY
    }

    //scores
    private float _curScore = 0;
   

    private GameState _thisGameState = GameState.MENU;
    void Awake()
    {

        ThisManager = this;
    }

	void Start () {
        LoadAssets();
        _curPLayerShip = Instantiate(PlayerShipObject, transform.position + new Vector3(0, ShipStats.PlayerHeigth, 0), Quaternion.identity) as GameObject;
        _ZoomIn = true;
    }
    

    public void StartNewGame()
    {
        GameObject.Find("menuCanvas").GetComponent<Canvas>().enabled = false;
            GameObject.Destroy(_curPLayerShip);
            _curPLayerShip = Instantiate(PlayerShipObject, transform.position + new Vector3(0, ShipStats.PlayerHeigth, 0), Quaternion.identity) as GameObject;
        PlayerCurIndex = (MapSize * MapSize) / 2;
        CannonCurIndex = (MapSize * MapSize) / 2;
        _thisGameState = GameState.PLAY;
        //_isFirstTurn = true;
        _isPlayerImune = false;
        FreeIndexList.Clear();
        if (MapList.Count > 0)
        {
            foreach (var tile in MapList)
                Destroy(tile);
            MapList.Clear();
        }
        
        MapList = GetComponent<MapGenerator>().GenerateMap();
        _ZoomOut = true;
    }

    void LoadAssets()
    {
        //load bad objects
        var badObjects = Resources.LoadAll("BadObjects");
        foreach (var obj in badObjects)
        {
            BadObjects.Add(obj as GameObject);
        }

        //load treasure objects
        var treasureObjects = Resources.LoadAll("TreasureObjects");
        foreach (var obj in treasureObjects)
        {
            TreasureObjects.Add(obj as GameObject);
        }

        //load flare objects
        var flareObjects = Resources.LoadAll("FlareObjects");
        foreach (var obj in flareObjects)
        {
            FlareObjects.Add(obj as GameObject);
        }

        //load map objects
        var mapObjects = Resources.LoadAll("MapObjects");
        foreach (var obj in mapObjects)
        {
            MapObjects.Add(obj as GameObject);
        }

        //load imune objects
        var imuneObjects = Resources.LoadAll("ImuneObjects");
        foreach (var obj in imuneObjects)
        {
            ImuneObjects.Add(obj as GameObject);
        }

        var cannonObjects = Resources.LoadAll("CannonObjects");
        foreach (var obj in cannonObjects)
        {
            CannonObjects.Add(obj as GameObject);
        }

    }
	// Update is called once per frame
	void Update () {

        if (MapList.Count == 0)
            MapList = GetComponent<MapGenerator>().GenerateMap();

        if (_thisGameState == GameState.MENU)
        {
            if (_ZoomIn)
                _ZoomIn = MainCamera.GetComponent<CameraController>().ZoomIn(_curPLayerShip.transform.position);
            else
                MainCamera.GetComponent<CameraController>().RotateOnPos(_curPLayerShip.transform.position);
        }
        else
        {
            if (_ZoomOut)
                _ZoomOut = MainCamera.GetComponent<CameraController>().ZoomOut(_curPLayerShip.transform.position);

            if (_isPlayerImune)
                _curPLayerShip.GetComponentInChildren<Renderer>().material.color = new Color(255, 0, 0);
            else
                _curPLayerShip.GetComponentInChildren<Renderer>().material.color = new Color(0, 0, 0);


            if (Input.GetKeyDown(KeyCode.Space))
            {
                foreach (var tile in MapList)
                {
                    tile.GetComponentInChildren<GameTile>().ShowObject();
                }
            }

            CheckInputPlayer();
            CheckInputCannon();

            if (_curPLayerShip.GetComponent<PlayerMovement>().IsMoving)
            {
                _curPLayerShip.GetComponent<PlayerMovement>().MoveShip();
           }
        }

    }

    void CheckInputPlayer()
    {
        if (!_curPLayerShip.GetComponent<PlayerMovement>().IsActive) return;

        bool moveShip = _curPLayerShip.GetComponent<PlayerMovement>().IsMoving;
        if (Input.GetKeyDown(KeyCode.UpArrow) && PlayerCurIndex / MapSize < MapSize - 1 && !moveShip)
        {
            PlayerCurIndex += MapSize;
            _curPLayerShip.GetComponent<PlayerMovement>().SetMovement();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && PlayerCurIndex / MapSize > 0 && !moveShip)
        {
            PlayerCurIndex -= MapSize;
            _curPLayerShip.GetComponent<PlayerMovement>().SetMovement();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && PlayerCurIndex % MapSize > 0 && !moveShip)
        {
            --PlayerCurIndex;
            _curPLayerShip.GetComponent<PlayerMovement>().SetMovement();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && PlayerCurIndex % MapSize < MapSize - 1 && !moveShip)
        {
            ++PlayerCurIndex;
            _curPLayerShip.GetComponent<PlayerMovement>().SetMovement();
        }


    }

    void CheckInputCannon()
    {
        if (!CannonController.IsActive) return;

        //bool moveShip = _curPLayerShip.GetComponent<PlayerMovement>().IsMoving;
        if (Input.GetKeyDown(KeyCode.UpArrow) && CannonCurIndex / MapSize < MapSize - 1 )
        {
            MapList[CannonCurIndex].GetComponentInChildren<GameTile>().MouseLeave();

            CannonCurIndex += MapSize;

            MapList[CannonCurIndex].GetComponentInChildren<GameTile>().MouseOver();
            //_curPLayerShip.GetComponent<PlayerMovement>().EndRotation = 0;
            //_curPLayerShip.GetComponent<PlayerMovement>().SetMovement();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && CannonCurIndex / MapSize > 0 )
        {
            MapList[CannonCurIndex].GetComponentInChildren<GameTile>().MouseLeave();

            CannonCurIndex -= MapSize;

            MapList[CannonCurIndex].GetComponentInChildren<GameTile>().MouseOver();
            //_curPLayerShip.GetComponent<PlayerMovement>().EndRotation = 180;
            //_curPLayerShip.GetComponent<PlayerMovement>().SetMovement();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && CannonCurIndex % MapSize > 0 )
        {
            MapList[CannonCurIndex].GetComponentInChildren<GameTile>().MouseLeave();

            --CannonCurIndex ;

            MapList[CannonCurIndex].GetComponentInChildren<GameTile>().MouseOver();
            //_curPLayerShip.GetComponent<PlayerMovement>().EndRotation = 270;
            //_curPLayerShip.GetComponent<PlayerMovement>().SetMovement();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && CannonCurIndex % MapSize < MapSize - 1 )
        {
            MapList[CannonCurIndex].GetComponentInChildren<GameTile>().MouseLeave();

            ++CannonCurIndex ;

            MapList[CannonCurIndex].GetComponentInChildren<GameTile>().MouseOver();
            //_curPLayerShip.GetComponent<PlayerMovement>().EndRotation = 90;
            //_curPLayerShip.GetComponent<PlayerMovement>().SetMovement();
        }
        if (Input.GetKeyDown(KeyCode.Keypad0)) 
        {
            //activate tile and get rid of highlight
            MapList[CannonCurIndex].GetComponentInChildren<GameTile>().MouseLeave();
            MapList[CannonCurIndex].GetComponentInChildren<GameTile>().ActivateTile();
            
            //deactivate cannon, reactivate player
            CannonController.IsActive = false;
            _curPLayerShip.GetComponent<PlayerMovement>().IsActive = true;
            //reset position to center
            CannonCurIndex = (MapSize * MapSize) / 2;
        }

    }



    public void ActivateTile()
    {
        MapList[PlayerCurIndex].GetComponentInChildren<GameTile>().ActivateTile();

        switch (MapList[PlayerCurIndex].GetComponentInChildren<GameTile>().ThisType)
        {
            case GameTile.TileType.BAD:
                if (!_isPlayerImune)
                {
                    GameObject.Find("menuCanvas").GetComponent<Canvas>().enabled = true;
                    _thisGameState = GameState.MENU;
                    _ZoomIn = true;
                }
                break;
            case GameTile.TileType.TREASURE:
                _curScore += ScoreValue;
                break;
            case GameTile.TileType.FLARE:

                if (!(PlayerCurIndex / MapSize == 0)) // not first row
                {
                        MapList[PlayerCurIndex - MapSize].GetComponentInChildren<GameTile>().ShowObject();
                }

                if (!(PlayerCurIndex / MapSize == MapSize-1)) // not last row
                {
                        MapList[PlayerCurIndex + MapSize].GetComponentInChildren<GameTile>().ShowObject();
                }

                if (!(PlayerCurIndex % MapSize == 0)) //not first column
                {
                        MapList[PlayerCurIndex - 1].GetComponentInChildren<GameTile>().ShowObject();
                }

                if (!(PlayerCurIndex % MapSize == MapSize-1)) //not last column
                {
                        MapList[PlayerCurIndex + 1].GetComponentInChildren<GameTile>().ShowObject();
                }


                if (!(PlayerCurIndex / MapSize == 0) && !(PlayerCurIndex % MapSize == 0)) // not first row &&not first column
                {
                        MapList[PlayerCurIndex - MapSize - 1 ].GetComponentInChildren<GameTile>().ShowObject();
                }

                if (!(PlayerCurIndex / MapSize == 0) && !(PlayerCurIndex % MapSize == MapSize-1)) // not first row && not last column
                {
                        MapList[PlayerCurIndex - MapSize].GetComponentInChildren<GameTile>().ShowObject();
                }


                if (!(PlayerCurIndex / MapSize == MapSize-1) && !(PlayerCurIndex % MapSize == 0)) // not last row && not first column
                {
                      MapList[PlayerCurIndex + MapSize].GetComponentInChildren<GameTile>().ShowObject();
                }

                if (!(PlayerCurIndex / MapSize == MapSize-1) && !(PlayerCurIndex % MapSize == MapSize-1)) // not last row && not last colum
                {
                    MapList[PlayerCurIndex + MapSize +1 ].GetComponentInChildren<GameTile>().ShowObject();
                }

                break;
            case GameTile.TileType.MAP:

                for (int i = 0; i < MapTilePower; i++)
                {
                    int randIndex = Random.Range(0, MapList.Count);

                    if (!MapList[randIndex].GetComponentInChildren<GameTile>().IsActivated && MapList[randIndex].GetComponentInChildren<GameTile>().IsHidden)
                        MapList[randIndex].GetComponentInChildren<GameTile>().ShowObject();
                    else
                        i--;
                }

                break;
            case GameTile.TileType.IMUNE:
                _isPlayerImune = true;
                return;
            case GameTile.TileType.CANNON:
                //activate cannon movement, deactivate player movement
                _curPLayerShip.GetComponent<PlayerMovement>().IsActive = false;
                CannonController.IsActive = true;
                //make sure the central tile is lit up
                MapList[CannonCurIndex].GetComponentInChildren<GameTile>().MouseOver();

                break;
            default:
                break;
        }

        _isPlayerImune = false;
        //_isFirstTurn = false;
    }


    public void ExitGame()
    {
        Application.Quit();
    }
}
