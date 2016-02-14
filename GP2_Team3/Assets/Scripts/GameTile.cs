using UnityEngine;
using System.Collections;

public class GameTile : MonoBehaviour {


    public bool IsEmpty = true;
    public bool IsHidden = true;
    public bool IsActivated = false;

    bool IsLit = false;

    public GameObject HiddenObject;
    public Material BorderColor;

    public enum TileType
    {
        BAD,
        TREASURE,
        FLARE,
        MAP,
        IMUNE,
        CANNON,
        EMPTY      
    }

    public TileType ThisType = TileType.EMPTY;

    public void MouseOver()
    {
        //shows a small cursor over the tile, but doesn't show the actual tile contents
        if (GameManager.ThisManager.TileMouseOverMaterial == null) return;
        if (IsLit) return;
        GetComponent<Renderer>().material.SetColor("_BorderColor", GameManager.ThisManager.TileMouseOverMaterial.color);

        IsLit = true;
    }

    public void MouseLeave()
    {
        if (BorderColor == null && !IsEmpty) return;
        if (!IsLit) return;

        if(IsHidden || IsEmpty)
            GetComponent<Renderer>().material.SetColor("_BorderColor", new Color(0,0,0,0));
        else
            GetComponent<Renderer>().material.SetColor("_BorderColor", BorderColor.color);
        IsLit = false;
    }

    public void ShowObject()
    {
        //shows the tile completely, but doesn't activate it.
        if (!IsEmpty && IsHidden && !IsActivated)
        {
            HiddenObject = Instantiate(HiddenObject, this.transform.position, Quaternion.identity) as GameObject;
            HiddenObject.transform.parent = this.transform;
            HiddenObject.GetComponentInChildren<Renderer>().material.color -= new Color(0, 0, 0, 0.5f);
            GetComponent<Renderer>().material.SetColor("_BorderColor", BorderColor.color);
            IsHidden = false;
        }
        if (IsEmpty)
        {
            //GetComponent<Renderer>().material.color = Color.cyan;
        }

    }

    public void ActivateTile()
    {
        //activates the tile, but effects are triggered by GameManager
        if (!IsEmpty && !IsActivated)
        {
            if (IsHidden)
            {
                HiddenObject = Instantiate(HiddenObject, this.transform.position, Quaternion.identity) as GameObject;
                GetComponent<Renderer>().material.SetColor("_BorderColor", BorderColor.color);
                HiddenObject.transform.parent = this.transform;
            }
            else 
            {
                HiddenObject.GetComponentInChildren<Renderer>().material.color += new Color(0, 0, 0, 0.5f);
            }
        }

        if (IsEmpty)
        {
            //GetComponentInChildren<Renderer>().material.color = Color.cyan;
        }

        IsActivated = true;
    }




    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
