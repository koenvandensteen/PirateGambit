using UnityEngine;
using System.Collections;

public class CannonballControls : MonoBehaviour
{
    public bool IsActive = false;    //set true temporarily for testing, should be false by default
    public int CannonCurIndex;

    GameTile curMouseOver = null;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        //if (!IsActive) return;

        //raycast and light up the correct ocean tile.
        //CheckTileRaycast();

        //if (curMouseOver != null && Input.GetMouseButtonDown(0))
        //{
        //    curMouseOver.ShowObject();
        //}
    }

    void CheckTileRaycast()
    {
        if (!Input.mousePresent) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            //Debug.Log("Called");
            var obj = hitInfo.collider.gameObject;
            var tileCmp = obj.GetComponent<GameTile>();
            if (tileCmp == null) return;

            if (!tileCmp.Equals(curMouseOver))
            {
                if (curMouseOver != null)
                    curMouseOver.MouseLeave();
                curMouseOver = tileCmp;
                tileCmp.MouseOver();
            }



            //Debug.Log("Hit");
            //tileCmp.LightUp()
        }
        else
        {
            if (curMouseOver != null) curMouseOver.MouseLeave();
            curMouseOver = null;
        }

    }
}
