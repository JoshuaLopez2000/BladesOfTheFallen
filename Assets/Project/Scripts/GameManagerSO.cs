using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameManagerSO", menuName = "Scriptable Objects/GameManagerSO")]
public class GameManagerSO : ScriptableObject
{
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
    public int maxEnemies = 5;
    public int enemiesKilled = 0;
    public float spawnInterval = 3.0f;
    public float enemySpeed = 1.0f;
    public float enemySpawnDistance = 10.0f;
    public float distanceBetweenEnemies = 0.5f;

    //BasicEnemy
    public float basicEnemySpeed = 1.0f;
    public float basicEnemyAttackRange = 2.0f;
    public int maxHits = 2;

    private void OnEnable()
    {
        //recover from a file if exist, else reset to default values
        playerLives = 3;
        playerScore = 0;
        maxEnemies = 5;
        spawnInterval = 3.0f;
        enemySpeed = 1.0f;
    }

    public void ResetGame()
    {
        playerLives = 3;
        playerScore = 0;
        maxEnemies = 5;
        spawnInterval = 3.0f;
        enemySpeed = 1.0f;
    }

    public void IncreaseScore(int amount)
    {
        playerScore += amount;
    }

    public void DecreaseLife()
    {
        playerLives--;
    }

    public bool PauseGame()
    {
        // Decreasing game speed and show pause menu
        Time.timeScale = 0.5f;
        // Show pause menu UI
        return true;
    }
}
