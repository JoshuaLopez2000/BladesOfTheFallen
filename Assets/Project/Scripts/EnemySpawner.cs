using BladesOfTheFallen.Core;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private const float SkipSpawnChance = 0.3f;

    [SerializeField] private GameManagerSO gameManager;
    [SerializeField] private GameObject basicEnemyPrefab;
    [SerializeField] private GameObject mediumEnemyPrefab;

    private GameObject player;
    private float nextSpawnTime;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        nextSpawnTime = Time.time + gameManager.SpawnInterval;

        if (player == null)
        {
            Debug.LogError("EnemySpawner requires a GameObject tagged 'Player'.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (gameManager.CurrentState != GameManagerSO.GameState.Playing)
        {
            return;
        }

        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemies();
            nextSpawnTime = Time.time + gameManager.SpawnInterval;
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
        if (EnemyDifficultyPolicy.ShouldReduceSpawnInterval(totalKilled))
        {
            gameManager.DecreaseSpawnInterval();
        }
    }

    private void SpawnEnemies()
    {
        if (Random.value < SkipSpawnChance)
        {
            return;
        }

        float distanceFromPlayer = gameManager.EnemySpawnDistance;
        Vector3 spawnOrigin = new(player.transform.position.x, transform.position.y, transform.position.z);
        Vector3 rightPosition = spawnOrigin + Vector3.right * distanceFromPlayer;
        Vector3 leftPosition = spawnOrigin + Vector3.left * distanceFromPlayer;

        SpawnSide spawnSide = (SpawnSide)Random.Range(0, 3);
        bool spawnMediumEnemy = Random.value >= 0.5f;
        int killed = gameManager.EnemiesKilled;
        EnemySpawnProfile profile = EnemyDifficultyPolicy.Evaluate(killed, Random.value);

        switch (spawnSide)
        {
            case SpawnSide.Right:
                SpawnEnemy(rightPosition, spawnMediumEnemy, profile.Speed, profile.BasicEnemyLives);
                break;
            case SpawnSide.Left:
                SpawnEnemy(leftPosition, spawnMediumEnemy, profile.Speed, profile.BasicEnemyLives);
                break;
            case SpawnSide.Both:
                SpawnEnemy(rightPosition, spawnMediumEnemy, profile.Speed, profile.BasicEnemyLives);
                SpawnEnemy(leftPosition, spawnMediumEnemy, profile.Speed, profile.BasicEnemyLives);
                break;
        }
    }

    private void SpawnEnemy(Vector3 position, bool spawnMediumEnemy, float speed, int basicLives)
    {
        GameObject prefab = spawnMediumEnemy ? mediumEnemyPrefab : basicEnemyPrefab;
        GameObject enemyObject = Instantiate(prefab, position, Quaternion.identity);

        if (!enemyObject.TryGetComponent(out EnemyBase enemy))
        {
            Debug.LogError($"Enemy prefab '{prefab.name}' is missing an EnemyBase component.", prefab);
            Destroy(enemyObject);
            return;
        }

        Color initialColor = spawnMediumEnemy
            ? new Color(0.196f, 0.059f, 0.207f)
            : basicLives > 1
                ? new Color32(255, 198, 0, 255)
                : new Color32(133, 28, 4, 255);
        int lives = spawnMediumEnemy ? 3 : basicLives;
        enemy.Initialize(player.transform, speed, lives, initialColor);
    }

    private enum SpawnSide
    {
        Right,
        Left,
        Both
    }
}
