//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------

using UnityEngine;

public class WhaleMove : MonoBehaviour
{
    private enum WhaleState
    {
        Climbing = 0,
        Breathing,
        Diving
    }


    public float Frequency;
    public float Magnitude;
    public float MoveSpeed;
    public float RotateSpeed;

    private float _curAngle;
   
    public float MaxBreathTime = 0.0f;
    private float _curBreathTime = 0;

    private WhaleState _curWhaleState = WhaleState.Climbing;
    // Use this for initialization
    void Start()
    {
        transform.rotation = Quaternion.Euler(0, Vector3.Angle(transform.position, new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f))), 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > 0.65f && _curWhaleState == WhaleState.Climbing)
        {
            _curWhaleState = WhaleState.Breathing;
            _curBreathTime = 0f;
        }

        if (_curBreathTime > MaxBreathTime && _curWhaleState == WhaleState.Breathing)
        {
            _curWhaleState = WhaleState.Diving;
            //Debug.Log("Breathing");
            //start particle effect
            GetComponentInChildren<ParticleSystem>().Play();
        }

        if (_curWhaleState == WhaleState.Breathing && _curBreathTime < MaxBreathTime)
        {
            _curBreathTime += Time.deltaTime;
            return;
        }

        if (_curWhaleState == WhaleState.Diving && transform.position.y < 0.1f)
        {
            Destroy(gameObject);
        }

        _curAngle += RotateSpeed * Time.deltaTime;
        float curSin = Mathf.Sin(_curAngle * Frequency);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, curSin * Magnitude);
        transform.position += Time.deltaTime * MoveSpeed * transform.right;
        
    }
}
