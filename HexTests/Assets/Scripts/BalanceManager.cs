using UnityEngine;
using System;
using System.Collections.Generic;
using System.Xml;



public class WorldElement
{
    public int WorldSize { get; set; }
    public int Krakens { get; set; }
    public int Imunity { get; set; }
    public int Treasure { get; set; }
    public int kegs { get; set; }
    public int Map { get; set; }
}

public class BalanceManager : MonoBehaviour {



    private static BalanceManager _instance;
    public static BalanceManager Instance
    {
        get { return _instance; }
        private set { _instance = value; }
    }
    
    private Dictionary<string, WorldElement> _worldVariablesDictionary;

    // Use this for initialization
    void Start () {
        _worldVariablesDictionary = new Dictionary<string, WorldElement>();
        InitalizeWorldDicitionary("WorldVariables");
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private void InitalizeWorldDicitionary(string fileName)
    {
        string filePath = "BalancingVariables/" + fileName;
        TextAsset targetfile = Resources.Load<TextAsset>(filePath);
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(targetfile.text);
        var worldList = xmlDoc.GetElementsByTagName("WorldElement");
        
        foreach(XmlNode worldElement in worldList)
        {
            _worldVariablesDictionary.Add(worldElement["name"].InnerText,
                new WorldElement
                {
                    WorldSize = Convert.ToInt32(worldElement["size"].InnerText),
                    Krakens = Convert.ToInt32(worldElement["krakens"].InnerText),
                    Imunity = Convert.ToInt32(worldElement["imunity"].InnerText),
                    Treasure = Convert.ToInt32(worldElement["treasure"].InnerText),
                    kegs = Convert.ToInt32(worldElement["kegs"].InnerText),
                    Map = Convert.ToInt32(worldElement["map"].InnerText),
                });
        }
    }

    public WorldElement GetWorldElement(string worldName)
    {
        return _worldVariablesDictionary["worldName"];
    }


}
