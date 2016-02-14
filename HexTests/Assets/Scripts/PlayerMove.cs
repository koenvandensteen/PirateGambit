//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {


    public bool IsMoving = false;
    public bool IsDead = false;
    public float RotationSpeed;
    public float MovementSpeed;

    private Vector3 _targetPos;
    public GameObject Shield;


    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (IsMoving)
            MoveTo();
    }

    public void SetMoveStart(Vector3 targetPos)
    {
        AudioManager.Instance.PlaySound("BoatSailSfx", 0.65f);
        IsMoving = true;
        _targetPos = targetPos;
        if (!IsDead)
            GetComponentInChildren<Animator>().SetTrigger   ("Move");
    }

    public void MoveTo() {
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_targetPos - transform.position, Vector3.up), RotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(_targetPos - transform.position, Vector3.up);
        transform.position += transform.forward * Time.deltaTime * MovementSpeed;

        if (Vector3.Distance(transform.position, _targetPos) < 0.2) {
            IsMoving = false;
            GameManager.ThisManager.CheckCurrentTile();
        }

    }

    public void Die() {
        IsDead = true;
        GetComponentInChildren<Animator>().SetTrigger("Die");
        AudioManager.Instance.PlaySound("ShipBreakSfx");
    }
}
