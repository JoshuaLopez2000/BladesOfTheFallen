using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject basicEnemyPrefab;
    public float distanceFromPlayer = 10f;
    private GameObject player;
    private float spawnInterval = 5f;
    private float nextSpawnTime = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        nextSpawnTime = Time.time + spawnInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }
        if (player != null)
        {
            // Position the spawner at a fixed distance from the player
            Vector3 direction = (transform.position - player.transform.position).normalized;
            transform.position = player.transform.position + direction * distanceFromPlayer;
        }
    }

    private void SpawnEnemy()
    {
        Instantiate(basicEnemyPrefab, transform.position, Quaternion.identity);
    }
}
