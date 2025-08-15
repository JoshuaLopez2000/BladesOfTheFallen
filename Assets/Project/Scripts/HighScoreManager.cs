using System.IO;
using UnityEngine;
using TMPro;

[System.Serializable]
public class HighScoreData
{
    public int highScore;
}

public class HighScoreManager : MonoBehaviour
{
    public GameManagerSO gameManager;
    public TMP_Text highScoreText;

    private int highScore = 0;
    private int currentScore = 0;
    private string filePath;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "highscore.json");
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

    public void CheckHighScore()
    {
        Debug.Log("Current Score:" + currentScore);
        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveHighScore();
            UpdateHighScoreUI();
        }
    }

    private void UpdateHighScoreUI()
    {
        highScoreText.text = "Record: " + highScore.ToString();
    }

    private void SaveHighScore()
    {
        HighScoreData data = new HighScoreData { highScore = highScore };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
    }

    private void LoadHighScore()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            HighScoreData data = JsonUtility.FromJson<HighScoreData>(json);
            highScore = data.highScore;
        }
        else
        {
            highScore = 0;
        }
    }
}
