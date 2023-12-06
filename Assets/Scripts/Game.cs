using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject Map;
    public GameObject PlayerPrefab;

    private GameObject[] _spawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        _spawnPoints = GameObject.FindGameObjectsWithTag("PlayerSpawnPoint");
        SpawnPlayerAtRandomSpawnPoint();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnPlayerAtRandomSpawnPoint()
    {
        var randomSpawnPointIndex = Random.Range(0, _spawnPoints.Length - 1);
        var player = Instantiate(PlayerPrefab, _spawnPoints[randomSpawnPointIndex].transform.position, Quaternion.identity);
        player.name = "Player";
    }
}
