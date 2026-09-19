using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "GameManager", menuName = "Blades of the Fallen/Game Manager")]
public class GameManagerSO : ScriptableObject
{
    private const int DefaultPlayerLives = 3;
    private const float DefaultSpawnInterval = 3f;
    private const float DefaultEnemySpeed = 1f;
    private const float MinimumSpawnInterval = 0.75f;

    [Header("Player")]
    [SerializeField, Min(1)] private int playerLives = DefaultPlayerLives;
    [SerializeField] private int _playerScore;
    [SerializeField, Min(0)] private int scorePerEnemy = 10;
    [SerializeField, Min(0)] private int scorePerHit = 5;
    [FormerlySerializedAs("PlayerAttackRange")]
    [SerializeField, Min(0f)] private float playerAttackRange = 8f;
    [FormerlySerializedAs("PlayerMaxApproachDistance")]
    [SerializeField, Min(0f)] private float playerMaxApproachDistance = 2f;
    [FormerlySerializedAs("hasEspecialHability")]
    [SerializeField] private bool hasSpecialAbility;

    [Header("Game")]
    [SerializeField] private int _enemiesKilled;
    [SerializeField, Min(MinimumSpawnInterval)] private float spawnInterval = DefaultSpawnInterval;
    [SerializeField, Min(0f)] private float enemySpeed = DefaultEnemySpeed;
    [SerializeField, Min(0f)] private float enemySpawnDistance = 10f;
    [SerializeField, Min(0f)] private float distanceBetweenEnemies = 0.5f;

    [Header("Enemies")]
    [SerializeField, Min(0f)] private float basicEnemyAttackRange = 2f;
    [FormerlySerializedAs("mediumEnemyTPDistance")]
    [SerializeField, Min(0f)] private float mediumEnemyTeleportDistance = 4f;
    [FormerlySerializedAs("distanceAfterHitPlayer")]
    [SerializeField, Min(0f)] private float distanceAfterPlayerHit = 8f;

    public GameState CurrentState { get; private set; } = GameState.Playing;
    public int PlayerLives => playerLives;
    public int PlayerScore
    {
        get => _playerScore;
        private set
        {
            _playerScore = value;
            OnScoreChanged?.Invoke(_playerScore);
        }
    }
    public int ScorePerEnemy => scorePerEnemy;
    public int ScorePerHit => scorePerHit;
    public float PlayerAttackRange => playerAttackRange;
    public float PlayerMaxApproachDistance => playerMaxApproachDistance;
    public bool HasSpecialAbility => hasSpecialAbility;
    public int EnemiesKilled
    {
        get => _enemiesKilled;
        private set
        {
            _enemiesKilled = value;
            OnEnemiesKilledChanged?.Invoke(_enemiesKilled);
        }
    }
    public float SpawnInterval => spawnInterval;
    public float EnemySpeed => enemySpeed;
    public float EnemySpawnDistance => enemySpawnDistance;
    public float DistanceBetweenEnemies => distanceBetweenEnemies;
    public float BasicEnemyAttackRange => basicEnemyAttackRange;
    public float MediumEnemyTeleportDistance => mediumEnemyTeleportDistance;
    public float DistanceAfterPlayerHit => distanceAfterPlayerHit;

    public event Action<int> OnScoreChanged;
    public event Action<int> OnEnemiesKilledChanged;
    public event Action<int> OnPlayerLivesChanged;
    public event Action<float> OnTimeScaleChanged;
    public event Action OnGameOver;

    private void OnEnable()
    {
        ResetRuntimeState();
    }

    public void ResetGame()
    {
        ResetSession();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void ResetSession()
    {
        ResetRuntimeState();
        Time.timeScale = 1f;
    }

    private void ResetRuntimeState()
    {
        CurrentState = GameState.Playing;
        playerLives = DefaultPlayerLives;
        PlayerScore = 0;
        spawnInterval = DefaultSpawnInterval;
        enemySpeed = DefaultEnemySpeed;
        EnemiesKilled = 0;
    }

    public void IncreaseScore(int amount)
    {
        PlayerScore = Mathf.Max(0, PlayerScore + amount);
    }

    public void DecreaseLife()
    {
        if (playerLives == 0)
        {
            return;
        }

        playerLives--;
        OnPlayerLivesChanged?.Invoke(playerLives);

        if (playerLives == 0)
        {
            ChangeState(GameState.GameOver);
        }
    }

    public void RegisterEnemyKilled()
    {
        EnemiesKilled++;
    }

    public void DecreaseSpawnInterval(float amount = 0.75f)
    {
        spawnInterval = Mathf.Max(MinimumSpawnInterval, spawnInterval - Mathf.Max(0f, amount));
    }

    public void ExponentialPause(float duration = 1f)
    {
        if (GameManagerMono.Instance != null)
        {
            GameManagerMono.Instance.Pause(duration, OnTimeScaleChanged);
        }
    }

    public void ExponentialResume(float duration = 1f)
    {
        if (GameManagerMono.Instance != null)
        {
            GameManagerMono.Instance.Resume(duration, OnTimeScaleChanged);
        }
    }

    public void HitTimeEffect(float slowFactor = 0.2f, float duration = 0.5f)
    {
        if (GameManagerMono.Instance != null)
        {
            GameManagerMono.Instance.HitStop(slowFactor, duration, OnTimeScaleChanged);
        }
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;

        if (CurrentState == GameState.GameOver)
        {
            OnGameOver?.Invoke();
        }
    }


    public enum GameState
    {
        Init,
        Playing,
        Paused,
        GameOver
    }
}
