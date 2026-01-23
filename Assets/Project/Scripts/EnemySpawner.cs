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
        if (totalKilled == 10)
            gameManager.DecreaseSpawnInterval();
        else if (totalKilled == 20)
            gameManager.DecreaseSpawnInterval();
        else if (totalKilled == 35)
            gameManager.DecreaseSpawnInterval();
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
        int dontSpawn = Random.Range(0, 10); // 30% chance to not spawn
        if (dontSpawn < 3) return;

        // Difficulty Calculation
        float spawnSpeed = 1.0f;
        int basicLives = 1;
        int killed = gameManager.enemiesKilled;

        if (killed < 5)
        {
            spawnSpeed = 1.5f;
            basicLives = 1;
        }
        else if (killed < 10)
        {
            spawnSpeed = 2.5f;
            basicLives = (Random.value < 0.3f) ? 2 : 1;
        }
        else
        {
            spawnSpeed = 3.5f;
            basicLives = (Random.value < 0.65f) ? 2 : 1;
        }

        // Colors
        Color redColor = new Color32(133, 28, 4, 255);
        Color yellowColor = new Color32(255, 198, 0, 255);
        Color purpleColor = new Color(0.196f, 0.059f, 0.207f);

        System.Action<GameObject> configureEnemy = (enemyObj) => {
            EnemyBase enemy = enemyObj.GetComponent<EnemyBase>();
            if (enemy is BasicEnemyController)
            {
                Color c = (basicLives > 1) ? yellowColor : redColor;
                enemy.Initialize(player.transform, spawnSpeed, basicLives, c);
            }
            else if (enemy is MediumEnemyController)
            {
                enemy.Initialize(player.transform, spawnSpeed, 3, purpleColor);
            }
        };

        switch (sideSpawn)
        {
            case 1:
                if (enemyType == 0) configureEnemy(Instantiate(basicEnemyPrefab, rightPos, Quaternion.identity));
                else configureEnemy(Instantiate(mediumEnemyPrefab, rightPos, Quaternion.identity));
                break;
            case 2:
                if (enemyType == 0) configureEnemy(Instantiate(basicEnemyPrefab, leftPos, Quaternion.identity));
                else configureEnemy(Instantiate(mediumEnemyPrefab, leftPos, Quaternion.identity));
                break;
            case 3:
                if (enemyType == 0) configureEnemy(Instantiate(basicEnemyPrefab, rightPos, Quaternion.identity));
                else configureEnemy(Instantiate(mediumEnemyPrefab, rightPos, Quaternion.identity));

                if (enemyType == 0) configureEnemy(Instantiate(basicEnemyPrefab, leftPos, Quaternion.identity));
                else configureEnemy(Instantiate(mediumEnemyPrefab, leftPos, Quaternion.identity));
                break;
        }
    }
}
