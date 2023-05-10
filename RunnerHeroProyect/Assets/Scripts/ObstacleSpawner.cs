using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public float spawnInterval = 2f;
    public GameObject[] obstacleSets;

    private Coroutine spawnCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        PlayerController.OnPlayerDead += StopSpawning;
        spawnCoroutine = StartCoroutine(SpawnObstacles());
    }

    private void StopSpawning()
    {
        StopCoroutine(spawnCoroutine);
    }

    private IEnumerator SpawnObstacles()
    {
        while (true)
        {

            // Randomly select an obstacle set
            int index = Random.Range(0, obstacleSets.Length);
            GameObject obstacleSet = obstacleSets[index];

            // Spawn the obstacle set at the spawn point
            Instantiate(obstacleSet, spawnPoint.position, Quaternion.identity);

            // Wait for the specified interval before spawning the next set
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void OnDestroy()
    {
        PlayerController.OnPlayerDead -= StopSpawning;
    }

}
