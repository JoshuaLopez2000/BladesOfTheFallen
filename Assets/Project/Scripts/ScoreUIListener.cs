using UnityEngine;
using TMPro;

public class ScoreUIListener : MonoBehaviour
{
    public GameManagerSO gameManagerSO;
    public TMP_Text scoreText;

    private void OnEnable()
    {
        if (gameManagerSO != null)
        {
            gameManagerSO.OnScoreChanged += UpdateScoreText;
            UpdateScoreText(gameManagerSO.playerScore);
        }
    }

    private void OnDisable()
    {
        if (gameManagerSO != null)
        {
            gameManagerSO.OnScoreChanged -= UpdateScoreText;
        }
    }

    private void UpdateScoreText(int newScore)
    {
        if (scoreText != null)
        {
            // Formato: 000 000 XP
            string formattedScore = newScore.ToString("D6");
            formattedScore = formattedScore.Insert(3, " ");
            scoreText.text = formattedScore + " XP";
        }
    }
}
