using UnityEngine;
using System.Collections;




public class PlayerMovement : MonoBehaviour {

    public bool IsActive = true;
    
    public bool IsMoving = false;
    public int PlayerCurIndex;
    
    private Vector3 _startPos;
    private Vector3 _startRot;
    public float EndRotation;

    private float _deltaMove;
    private float _deltaRotation;

    private GameManager.Ship _thisShip; 

    // Use this for initialization
    void Start () {
        _thisShip = GameManager.ThisManager.ShipStats;
    }
	
	// Update is called once per frame
	void Update () {
        if (IsMoving)
            MoveShip();
	}

   public void SetMovement()
   {
       PlayerCurIndex = GameManager.ThisManager.PlayerCurIndex;
       IsMoving = true;
       
       _startPos = transform.position;
       _startRot = transform.rotation.eulerAngles;

       _deltaRotation = 0;
        _deltaMove = 0;
    }

   
   
   public void MoveShip()
   {

        PlayerCurIndex = GameManager.ThisManager.PlayerCurIndex;

        _deltaRotation += Time.deltaTime * _thisShip.RotSlowDown;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(GameManager.ThisManager.MapList[PlayerCurIndex].transform.position-transform.position,Vector3.up), _thisShip.RotationSpeed * Time.deltaTime + _deltaRotation);

        if (_thisShip.MovementSpeed - _deltaMove > 0.05)
        {
            _deltaMove += Time.deltaTime * _thisShip.MovSlowDown;
        }

        transform.position += transform.forward * Time.deltaTime * (_thisShip.MovementSpeed - _deltaMove);

        if (Vector3.Distance(GameManager.ThisManager.MapList[PlayerCurIndex].transform.position, new Vector3(transform.position.x,0, transform.position.z)) < _thisShip.RotateOffset)
         {
            IsMoving = false;
            if (!GameManager.ThisManager.MapList[PlayerCurIndex].GetComponentInChildren<GameTile>().IsActivated)
                  GameManager.ThisManager.ActivateTile();

         }
  
   }
}
