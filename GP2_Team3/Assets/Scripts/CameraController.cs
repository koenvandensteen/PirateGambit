using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public float CameraSpeed;

    public Vector3 ZoomedOutPos;
    public Vector3 ZoomedOutRot;
    public Vector3 ZoomedInPos;
    public Vector3 ZoomedInRot;

    public float CameraLAOffset = 0.4f;
    public float ZoomOffset = 5f;
    public float ZoomOutOffset = 10f;
    public float CameraRotateSpeed = 5f;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool ZoomIn(Vector3 position)
    {
        Vector3 deltaPos = Vector3.Normalize(position - transform.position);
        transform.position += deltaPos * Time.deltaTime * CameraSpeed * Vector3.Distance(position, transform.position);

        transform.LookAt(position + new Vector3(0, CameraLAOffset, 0));
        return Vector3.Distance(position,transform.position) > ZoomOffset;
    }

    public void RotateOnPos(Vector3 position)
    {
        transform.RotateAround(position,Vector3.up,Time.deltaTime * CameraRotateSpeed);
    }

    public bool ZoomOut(Vector3 playerPosition)
    {
        //Vector3 deltaPos = Vector3.Normalize(ZoomedOutPos - transform.position);
        //transform.position += deltaPos * Time.deltaTime * CameraSpeed * Vector3.Distance(playerPosition,ZoomedOutPos);
        //
        //transform.LookAt(Vector3.zero);
        //return Vector3.Distance(playerPosition, transform.position) < Vector3.Distance(transform.position, ZoomedOutPos) ;

        transform.position = ZoomedOutPos;
        transform.LookAt(Vector3.zero);
        return false;
    }
}
