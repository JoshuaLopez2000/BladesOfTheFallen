using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameManagerSO gameManager;
    public GameObject basicEnemyPrefab, mediumEnemyPrefab, hardEnemyPrefab;
    public float distanceFromPlayer;
    public float spawnInterval;

    private GameObject player;
    private float nextSpawnTime = 0f;

    void Start()
    {
        distanceFromPlayer = gameManager.enemySpawnDistance;
        spawnInterval = gameManager.spawnInterval;

        player = GameObject.FindWithTag("Player");
        nextSpawnTime = Time.time + spawnInterval;
    }

    void Update()
    {
        if (player == null) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemies();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void SpawnEnemies()
    {
        Vector3 rightPos = new Vector3(player.transform.position.x + distanceFromPlayer,
                                       transform.position.y,
                                       transform.position.z);

        Vector3 leftPos = new Vector3(player.transform.position.x - distanceFromPlayer,
                                      transform.position.y,
                                      transform.position.z);

        int sideSpawn = Random.Range(1, 4);
        switch (sideSpawn)
        {
            case 1:
                Instantiate(basicEnemyPrefab, rightPos, Quaternion.identity);
                break;
            case 2:
                Instantiate(basicEnemyPrefab, leftPos, Quaternion.identity);
                break;
            case 3:
                Instantiate(basicEnemyPrefab, rightPos, Quaternion.identity);
                Instantiate(basicEnemyPrefab, leftPos, Quaternion.identity);
                break;
        }
    }
}
