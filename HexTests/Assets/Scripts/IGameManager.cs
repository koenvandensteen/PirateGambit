using UnityEngine;
using System.Collections;

public interface IGameManager
{
    HexMap GameMap{ get; }
    GameState GameState { get; set; }
    int CurKrakenAmmount { get; set; }

}
