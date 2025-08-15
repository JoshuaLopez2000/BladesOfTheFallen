using UnityEngine;
using UnityEngine.UI;

public class MenuPauseManager : MonoBehaviour
{
    public GameManagerSO gameManager;
    public Button resumeButton;
    public Button restartButton;
    public Button quitButton;

    public Button pauseIcon, pauseIconSecundary;

    void Start()
    {
        resumeButton.onClick.AddListener(OnResumeButtonClicked);
        restartButton.onClick.AddListener(OnRestartButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
        pauseIcon.onClick.AddListener(OnResumeButtonClicked);
        pauseIconSecundary.onClick.AddListener(OnResumeButtonClicked);
    }

    private void OnResumeButtonClicked()
    {
        if (gameManager.gameState == GameManagerSO.GameState.PAUSE)
        {
            gameManager.ChangeState(GameManagerSO.GameState.PLAYING);
            gameManager.ExponentialResume(2f);
        }
        else if (gameManager.gameState == GameManagerSO.GameState.PLAYING)
        {
            gameManager.ChangeState(GameManagerSO.GameState.PAUSE);
            gameManager.ExponentialPause(2f);
        }
    }

    private void OnRestartButtonClicked()
    {
        gameManager.ResetGame();
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
