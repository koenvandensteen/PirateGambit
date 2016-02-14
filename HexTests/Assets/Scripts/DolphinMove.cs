//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------

using UnityEngine;

public class DolphinMove : MonoBehaviour
{



    public float MoveDirection;
    public float Frequency;
    public float Magnitude;
    public float MoveSpeed;
    public float RotateSpeed;

    private float _curAngle;
    private bool _switchDir = false;

    private float lowestVal = 0;
    private float highestVal = 0;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        float curSin = Mathf.Sin(_curAngle * Frequency);
        _curAngle += RotateSpeed * Time.deltaTime;

       //
       // if (transform.position.y <= 0 && _switchDir)
       // {
       //     var randAngle = Random.Range(0, 6);
       //     transform.GetComponentInParent<Transform>().Rotate(new Vector3(0, randAngle * 60, 0));
       //     _switchDir = false;
       // }
       //
       // if (transform.position.y >= 0.50)
       // {
       //     Debug.Log("switch dir");
       //     _switchDir = true;
       // }

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, curSin * Magnitude);
        transform.position += Time.deltaTime * MoveSpeed * transform.right;


    }
}
