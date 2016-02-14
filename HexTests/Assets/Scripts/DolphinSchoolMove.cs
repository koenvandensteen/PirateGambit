//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class DolphinSchoolMove : MonoBehaviour
{
    private Vector3 dolphinPos;
    private bool _switchDir = false;
    private int _curJumpAmmount = 0;
    private int _maxJumpAmmount = 5;
    // Use this for initialization

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        dolphinPos = GetComponentsInChildren<Transform>()[1].position;
        if (dolphinPos.y <= 0 && _switchDir)
        {
            if (_maxJumpAmmount < _curJumpAmmount)
            {
                Destroy(gameObject);
            }

            var randAngle = Random.Range(0, 6);
            foreach (var dolphin in GetComponentsInChildren<DolphinMove>())
            {
                dolphin.GetComponent<Transform>().Rotate(0, randAngle * 60, 0);
            }
            _switchDir = false;
            _curJumpAmmount++;
        }

        if (dolphinPos.y >= 0.5)
        {
            _switchDir = true;
        }
    }
}
