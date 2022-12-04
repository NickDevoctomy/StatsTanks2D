using System.Collections.Generic;
using UnityEngine;

public class BotSpawner : MonoBehaviour
{
    public static BotSpawner Instance => _instance;

    public int SpawnCount;
    public GameObject BotPrefab;
    public List<GameObject> Bots { get; } = new List<GameObject>();

    private static BotSpawner _instance;
    private GameObject[] _spawnPoints;

    void Start()
    {
        _instance = this;
    }

    //void Update()
    //{
    //}

    public void Spawn()
    {
        _spawnPoints = GameObject.FindGameObjectsWithTag("BotSpawnPoint");
        // Pick a random spawn point here, for now just pick 0 as we only have 1 for testing
        var randomSpawnPointIndex = 0;
        var botObject = GameObject.Instantiate(
            BotPrefab,
            _spawnPoints[randomSpawnPointIndex].transform.position,
            Quaternion.identity);
        Bots.Add(botObject);
    }
}
