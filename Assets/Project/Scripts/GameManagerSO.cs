using System;
using BladesOfTheFallen.Core;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "GameManager", menuName = "Blades of the Fallen/Game Manager")]
public class GameManagerSO : ScriptableObject
{
    private const int DefaultPlayerLives = 3;
    private const float DefaultSpawnInterval = 3f;
    private const float DefaultEnemySpeed = 1f;
    private const float MinimumSpawnInterval = 0.75f;

    [Header("Session Defaults")]
    [FormerlySerializedAs("playerLives")]
    [SerializeField, Min(1)] private int initialPlayerLives = DefaultPlayerLives;
    [FormerlySerializedAs("spawnInterval")]
    [SerializeField, Min(MinimumSpawnInterval)] private float initialSpawnInterval = DefaultSpawnInterval;
    [FormerlySerializedAs("enemySpeed")]
    [SerializeField, Min(0f)] private float initialEnemySpeed = DefaultEnemySpeed;

    [Header("Scoring")]
    [SerializeField, Min(0)] private int scorePerEnemy = 10;
    [SerializeField, Min(0)] private int scorePerHit = 5;

    [Header("Player")]
    [FormerlySerializedAs("PlayerAttackRange")]
    [SerializeField, Min(0f)] private float playerAttackRange = 8f;
    [FormerlySerializedAs("PlayerMaxApproachDistance")]
    [SerializeField, Min(0f)] private float playerMaxApproachDistance = 2f;
    [FormerlySerializedAs("hasEspecialHability")]
    [SerializeField] private bool hasSpecialAbility;

    [Header("Spawning")]
    [SerializeField, Min(0f)] private float enemySpawnDistance = 10f;
    [SerializeField, Min(0f)] private float distanceBetweenEnemies = 0.5f;

    [Header("Enemies")]
    [SerializeField, Min(0f)] private float basicEnemyAttackRange = 2f;
    [SerializeField, Min(0f)] private float mediumEnemyTeleportRangeInset = 0.75f;
    [FormerlySerializedAs("distanceAfterHitPlayer")]
    [SerializeField, Min(0f)] private float distanceAfterPlayerHit = 8f;

    [NonSerialized] private GameSession session;

    public GameState CurrentState => (GameState)Session.State;
    public int PlayerLives => Session.Lives;
    public int PlayerScore => Session.Score;
    public int ScorePerEnemy => scorePerEnemy;
    public int ScorePerHit => scorePerHit;
    public float PlayerAttackRange => playerAttackRange;
    public float PlayerMaxApproachDistance => playerMaxApproachDistance;
    public bool HasSpecialAbility => hasSpecialAbility;
    public int EnemiesKilled => Session.EnemiesKilled;
    public float SpawnInterval => Session.SpawnInterval;
    public float EnemySpeed => Session.EnemySpeed;
    public float EnemySpawnDistance => enemySpawnDistance;
    public float DistanceBetweenEnemies => distanceBetweenEnemies;
    public float BasicEnemyAttackRange => basicEnemyAttackRange;
    public float MediumEnemyTeleportRangeInset => mediumEnemyTeleportRangeInset;
    public float DistanceAfterPlayerHit => distanceAfterPlayerHit;

    public event Action<int> OnScoreChanged;
    public event Action<int> OnEnemiesKilledChanged;
    public event Action<int> OnPlayerLivesChanged;
    public event Action<float> OnTimeScaleChanged;
    public event Action OnGameOver;

    // Requests keep the state asset independent from scenes and coroutines.
    public event Action<float> PauseRequested;
    public event Action<float> ResumeRequested;
    public event Action<float, float> HitStopRequested;
    public event Action<float, float> ParryTimeEffectRequested;
    public event Action RestartRequested;
    public event Action MainMenuRequested;

    private GameSession Session
    {
        get
        {
            if (session == null)
            {
                CreateSession();
            }

            return session;
        }
    }

    private void OnEnable()
    {
        CreateSession();
    }

    private void OnDisable()
    {
        DetachSessionEvents();
    }

    public void ResetGame()
    {
        ResetSession();
        RestartRequested?.Invoke();
    }

    public void ResetSession()
    {
        Session.Reset();
    }

    public void ReturnToMainMenu()
    {
        MainMenuRequested?.Invoke();
    }

    public void IncreaseScore(int amount) => Session.AddScore(amount);
    public void DecreaseLife() => Session.RemoveLife();
    public void RegisterEnemyKilled() => Session.RegisterEnemyKilled();
    public void DecreaseSpawnInterval(float amount = 0.75f) => Session.ReduceSpawnInterval(amount);

    public void ExponentialPause(float duration = 1f)
    {
        PauseRequested?.Invoke(Mathf.Max(0f, duration));
    }

    public void ExponentialResume(float duration = 1f)
    {
        ResumeRequested?.Invoke(Mathf.Max(0f, duration));
    }

    public void HitTimeEffect(float slowFactor = 0.2f, float duration = 0.5f)
    {
        HitStopRequested?.Invoke(Mathf.Clamp01(slowFactor), Mathf.Max(0f, duration));
    }

    public void ParryTimeEffect(float slowFactor = 0.1f, float duration = 0.45f)
    {
        ParryTimeEffectRequested?.Invoke(Mathf.Clamp01(slowFactor), Mathf.Max(0f, duration));
    }

    public void ChangeState(GameState newState)
    {
        Session.ChangeState((SessionState)newState);
    }

    internal void ReportTimeScale(float value)
    {
        OnTimeScaleChanged?.Invoke(value);
    }

    private void CreateSession()
    {
        DetachSessionEvents();
        session = new GameSession(
            initialPlayerLives,
            initialSpawnInterval,
            MinimumSpawnInterval,
            initialEnemySpeed);
        AttachSessionEvents();
    }

    private void AttachSessionEvents()
    {
        session.ScoreChanged += HandleScoreChanged;
        session.EnemiesKilledChanged += HandleEnemiesKilledChanged;
        session.LivesChanged += HandleLivesChanged;
        session.GameOver += HandleGameOver;
    }

    private void DetachSessionEvents()
    {
        if (session == null)
        {
            return;
        }

        session.ScoreChanged -= HandleScoreChanged;
        session.EnemiesKilledChanged -= HandleEnemiesKilledChanged;
        session.LivesChanged -= HandleLivesChanged;
        session.GameOver -= HandleGameOver;
    }

    private void HandleScoreChanged(int score) => OnScoreChanged?.Invoke(score);
    private void HandleEnemiesKilledChanged(int killed) => OnEnemiesKilledChanged?.Invoke(killed);
    private void HandleLivesChanged(int lives) => OnPlayerLivesChanged?.Invoke(lives);
    private void HandleGameOver() => OnGameOver?.Invoke();

    public enum GameState
    {
        Init,
        Playing,
        Paused,
        GameOver
    }
}
