using System.IO;
using BladesOfTheFallen.Core;
using BladesOfTheFallen.Infrastructure.Persistence;
using TMPro;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    private const string FileName = "highscore.json";

    [SerializeField] private GameManagerSO gameManager;
    [SerializeField] private TMP_Text highScoreText;

    private HighScoreService highScores;
    private int currentScore;

    private void Awake()
    {
        if (gameManager == null)
        {
            Debug.LogError("HighScoreManager requires a GameManagerSO reference.", this);
            enabled = false;
            return;
        }

        string filePath = Path.Combine(Application.persistentDataPath, FileName);
        JsonHighScoreRepository repository = new(filePath, message => Debug.LogWarning(message, this));
        highScores = new HighScoreService(repository);
        currentScore = gameManager.PlayerScore;
        UpdateHighScoreUI();
    }

    private void OnEnable()
    {
        if (gameManager == null)
        {
            Debug.LogError("HighScoreManager requires a GameManagerSO reference.", this);
            enabled = false;
            return;
        }

        gameManager.OnGameOver += HandleGameOver;
        gameManager.OnScoreChanged += UpdateCurrentScore;
    }

    private void OnDisable()
    {
        if (gameManager == null)
        {
            return;
        }

        gameManager.OnGameOver -= HandleGameOver;
        gameManager.OnScoreChanged -= UpdateCurrentScore;
    }

    private void UpdateCurrentScore(int score)
    {
        currentScore = score;
    }

    private void HandleGameOver()
    {
        if (highScores.TryRegister(currentScore))
        {
            UpdateHighScoreUI();
        }
    }

    private void UpdateHighScoreUI()
    {
        if (highScoreText != null)
        {
            highScoreText.SetText("Record: {0}", highScores.HighScore);
        }
    }
}
