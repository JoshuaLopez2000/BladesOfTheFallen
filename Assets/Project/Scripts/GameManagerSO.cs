using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "GameManagerSO", menuName = "Scriptable Objects/GameManagerSO")]
public class GameManagerSO : ScriptableObject
{

    public GameState gameState = GameState.PLAYING;

    //Player
    public int playerLives = 3;
    public int _playerScore;
    public int playerScore
    {
        get => _playerScore;
        private set
        {
            _playerScore = value;
            OnScoreChanged?.Invoke(_playerScore);
        }
    }

    public event Action<int> OnScoreChanged;
    public int scorePerEnemy = 10;
    public int scorePerHit = 5;
    public int scorePerPerfectHit = 15;
    public float PlayerAttackRange = 8.0f;
    public float PlayerMaxApproachDistance = 2.0f;
    public bool hasEspecialHability = false;


    //Game
    public int level = 1;
    public int enemiesKilled = 0;
    public float spawnInterval = 3.0f;
    public float enemySpeed = 1.0f;
    public float enemySpawnDistance = 10.0f;
    public float distanceBetweenEnemies = 0.5f;

    //BasicEnemy
    public float basicEnemySpeed = 1.0f;
    public float basicEnemyAttackRange = 2.0f;
    public int maxHits = 2;
    public float timeToDestroyEnemy = 0.1f;

    private void OnEnable()
    {
        gameState = GameState.PLAYING;

        //recover from a file if exist, else reset to default values
        level = 1;
        playerLives = 3;
        playerScore = 0;
        spawnInterval = 3.0f;
        enemySpeed = 1.0f;
        enemiesKilled = 0;
    }

    public void ResetGame()
    {
        gameState = GameState.PLAYING;

        playerLives = 3;
        playerScore = 0;
        spawnInterval = 3.0f;
        enemySpeed = 1.0f;
        enemiesKilled = 0;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void IncreaseScore(int amount)
    {
        playerScore += amount;
    }

    public void DecreaseLife()
    {
        playerLives--;
    }


    public event Action<float> OnTimeScaleChanged;

    public void ExponentialPause(float duration = 1f)
    {
        GameManagerMono.Instance.StartCoroutine(GameManagerMono.Instance.ExponentialPauseCoroutine(duration, OnTimeScaleChanged));
    }

    public void ExponentialResume(float duration = 1f)
    {
        GameManagerMono.Instance.StartCoroutine(GameManagerMono.Instance.ExponentialResumeCoroutine(duration, OnTimeScaleChanged));
    }

    public void HitTimeEffect(float slowFactor = 0.2f, float duration = 0.5f)
    {
        GameManagerMono.Instance.StartCoroutine(GameManagerMono.Instance.HitTimeCoroutine(slowFactor, duration, OnTimeScaleChanged));
    }


    public void ChangeState(GameState newState)
    {
        switch (gameState)
        {
            case GameState.INIT:
                break;
            case GameState.PLAYING:
                break;
            case GameState.PAUSE:
                break;
            case GameState.GAME_OVER:
                break;
        }
        gameState = newState;
    }

    public enum GameState
    {
        INIT,
        PLAYING,
        PAUSE,
        GAME_OVER
    }
}
