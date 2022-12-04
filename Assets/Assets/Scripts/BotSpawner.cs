using UnityEngine;

public class BotSpawner : MonoBehaviour
{
    public int SpawnCount;
    public GameObject BotPrefab;

    private GameObject[] _spawnPoints;

    void Start()
    {
        _spawnPoints = GameObject.FindGameObjectsWithTag("BotSpawnPoint");
        var bot = GameObject.Instantiate(BotPrefab, transform);
        // Pick a random spawn point here, for now just pick 0 as we only have 1 for testing
        var randomSpawnPointIndex = 0;
        bot.transform.position = _spawnPoints[randomSpawnPointIndex].transform.position;
    }

    void Update()
    {
        
    }
}
