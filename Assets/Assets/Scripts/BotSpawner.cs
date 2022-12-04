using UnityEngine;

public class BotSpawner : MonoBehaviour
{
    public int SpawnCount;
    public GameObject BotPrefab;

    private GameObject[] _spawnPoints;

    void Start()
    {
        _spawnPoints = GameObject.FindGameObjectsWithTag("BotSpawnPoint");
    }

    void Update()
    {
        
    }
}
