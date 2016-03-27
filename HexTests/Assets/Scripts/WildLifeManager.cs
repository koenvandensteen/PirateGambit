//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using System.Linq;
using UnityEngine;

public class WildLifeManager : MonoBehaviour {


    public bool DoOnlySeagulls = false;
    private enum RandomEvent {
        Idle,
        DoplphinSchool,
        Whale,
        SeaGull,
        Size
    }

    public GameObject Dolphin;
    public GameObject Whale;
    public GameObject SeaGull;

    public int DolphinSchoolSize = 5;
    public int MaxSeagullFlockSize = 5;

    public float RandomEventTimer = 5.0f;
    private float _curEventTimer = 0f;

    private RandomEvent _curEvent = RandomEvent.Idle;

    private Vector3 _spawnPos;

    public float DolphinSpawnInterval = 1.0f;

    //private float _curDolphinInterval = 0f;
    private int _dolphinCount = 0;
    private GameManager _thisManager;

    void Start() {
        _thisManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update() {
        if (_curEventTimer > RandomEventTimer) {
            _curEventTimer = 0;
            _spawnPos = new Vector3(Random.Range(-6f, 6f), 0, Random.Range(-8f, 8f));
            if (DoOnlySeagulls)
                _curEvent = RandomEvent.SeaGull;
            else
                _curEvent = (RandomEvent)Random.Range(1, (int)RandomEvent.Size);
        }

        switch (_curEvent) {
            case RandomEvent.Idle:
                _curEventTimer += Time.deltaTime;
                break;
            case RandomEvent.DoplphinSchool:
                SpawnDolphinEvent();
                break;
            case RandomEvent.Whale:
                SpawnWhaleEvent();
                break;
            case RandomEvent.SeaGull:
                SpawnSeaGullEvent();
                break;
        }

    }

    private void SpawnDolphinEvent() {
        //_curDolphinInterval += Time.deltaTime;

        //int randX = Random.Range(0, _thisManager.GameMap.Count);
        //int randY = Random.Range(0, _thisManager.GameMap[randX].Count);
        //var tile = _thisManager.GameMap[randX][randY];
        //_spawnPos = tile.transform.position;

        //var angle = Random.Range(0, 6);

        //var dolphin = Instantiate(Dolphin, _spawnPos, Quaternion.Euler(0, angle * 60, 0)) as GameObject;

        //// Destroy(dolphin, Random.Range(5f, 7f));
        //_curEvent = RandomEvent.Idle;
    }

    private void SpawnWhaleEvent() {
        _spawnPos += new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
        //var whale = Instantiate(Whale, _spawnPos, Quaternion.identity) as GameObject;

        //_curDolphinInterval = 0;
        _dolphinCount++;

        _curEvent = RandomEvent.Idle;
    }

    private void SpawnSeaGullEvent() {
        int startQuadrant = Random.Range(0, 4);

        Vector3 startPos = Vector3.zero;

        switch (startQuadrant) {
            case 0:
                startPos = new Vector3(Random.Range(-6f, -8f), 0, Random.Range(6f, 8f));
                break;
            case 1:
                startPos = new Vector3(Random.Range(-6f, -8f), 0, Random.Range(-6f, -8f));
                break;
            case 2:
                startPos = new Vector3(Random.Range(6f, 8f), 0, Random.Range(-6f, -8f));
                break;
            case 3:
                startPos = new Vector3(Random.Range(6f, 8f), 0, Random.Range(6f, 8f));
                break;
        }

        float amount = Random.Range(1, MaxSeagullFlockSize + 1);

        for (int i = 0; i < amount; i++) {
            var spawnPos = startPos + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            var seaGull = Instantiate(SeaGull, spawnPos, Quaternion.identity);
            Destroy(seaGull, 15f);
        }

        _curEvent = RandomEvent.Idle;
    }

}
