using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameManagerSO gameManager;
    public GameObject basicEnemyPrefab, mediumEnemyPrefab, hardEnemyPrefab;
    public float distanceFromPlayer;
    private GameObject player;
    private float nextSpawnTime = 0f;

    void Start()
    {
        distanceFromPlayer = gameManager.enemySpawnDistance;

        player = GameObject.FindWithTag("Player");
        nextSpawnTime = Time.time + gameManager.spawnInterval;
    }

    void Update()
    {
        if (player == null) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemies();
            nextSpawnTime = Time.time + gameManager.spawnInterval;
        }
    }


    private void OnEnable()
    {
        gameManager.OnEnemiesKilledChanged += UpdateSpawnInterval;
    }

    private void OnDisable()
    {
        gameManager.OnEnemiesKilledChanged -= UpdateSpawnInterval;
    }

    private void UpdateSpawnInterval(int totalKilled)
    {
        if (totalKilled < 10)
            gameManager.spawnInterval = 2.0f;
        else if (totalKilled < 30)
            gameManager.spawnInterval = 1.0f;
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
        int enemyType = Random.Range(0, 2); // 0: Basic, 1: Medium
        switch (sideSpawn)
        {
            case 1:
                if (enemyType == 0)
                    Instantiate(basicEnemyPrefab, rightPos, Quaternion.identity);
                else
                    Instantiate(mediumEnemyPrefab, rightPos, Quaternion.identity);
                break;
            case 2:
                if (enemyType == 0)
                    Instantiate(basicEnemyPrefab, leftPos, Quaternion.identity);
                else
                    Instantiate(mediumEnemyPrefab, leftPos, Quaternion.identity);
                break;
            case 3:
                if (enemyType == 0)
                    Instantiate(basicEnemyPrefab, rightPos, Quaternion.identity);
                else
                    Instantiate(mediumEnemyPrefab, rightPos, Quaternion.identity);

                if (enemyType == 0)
                    Instantiate(basicEnemyPrefab, leftPos, Quaternion.identity);
                else
                    Instantiate(mediumEnemyPrefab, leftPos, Quaternion.identity);
                break;
        }
    }
}
