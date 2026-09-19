using UnityEngine;
using TMPro;

public class ScoreUIListener : MonoBehaviour
{
    [SerializeField] private GameManagerSO gameManagerSO;
    [SerializeField] private TMP_Text scoreText;

    private void OnEnable()
    {
        if (gameManagerSO != null)
        {
            gameManagerSO.OnScoreChanged += UpdateScoreText;
            UpdateScoreText(gameManagerSO.PlayerScore);
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
            string formattedScore = newScore.ToString("D6");
            formattedScore = formattedScore.Insert(3, " ");
            scoreText.text = $"{formattedScore} XP";
        }
    }
}
