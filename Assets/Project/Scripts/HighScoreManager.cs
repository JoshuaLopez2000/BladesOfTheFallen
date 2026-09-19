using System;
using System.IO;
using TMPro;
using UnityEngine;

[Serializable]
internal sealed class HighScoreData
{
    public int highScore;
}

public class HighScoreManager : MonoBehaviour
{
    private const string FileName = "highscore.json";

    [SerializeField] private GameManagerSO gameManager;
    [SerializeField] private TMP_Text highScoreText;

    private int highScore;
    private int currentScore;
    private string filePath;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, FileName);
        currentScore = gameManager.PlayerScore;
        LoadHighScore();
        UpdateHighScoreUI();
    }

    private void OnEnable()
    {
        gameManager.OnGameOver += HandleGameOver;
        gameManager.OnScoreChanged += UpdateCurrentScore;
    }

    private void OnDisable()
    {
        gameManager.OnGameOver -= HandleGameOver;
        gameManager.OnScoreChanged -= UpdateCurrentScore;
    }

    private void UpdateCurrentScore(int score)
    {
        currentScore = score;
    }

    private void HandleGameOver()
    {
        CheckHighScore();
    }

    private void CheckHighScore()
    {
        if (currentScore <= highScore)
        {
            return;
        }

        highScore = currentScore;
        SaveHighScore();
        UpdateHighScoreUI();
    }

    private void UpdateHighScoreUI()
    {
        if (highScoreText != null)
        {
            highScoreText.SetText("Record: {0}", highScore);
        }
    }

    private void SaveHighScore()
    {
        try
        {
            HighScoreData data = new() { highScore = highScore };
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, json);
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Could not save the high score: {exception.Message}", this);
        }
    }

    private void LoadHighScore()
    {
        if (!File.Exists(filePath))
        {
            highScore = 0;
            return;
        }

        try
        {
            string json = File.ReadAllText(filePath);
            HighScoreData data = JsonUtility.FromJson<HighScoreData>(json);
            highScore = Mathf.Max(0, data?.highScore ?? 0);
        }
        catch (Exception exception)
        {
            highScore = 0;
            Debug.LogWarning($"Could not load the high score: {exception.Message}", this);
        }
    }
}
