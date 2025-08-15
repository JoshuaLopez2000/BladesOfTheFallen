using UnityEngine;
using UnityEngine.UI;

public class MenuPauseManager : MonoBehaviour
{
    public GameManagerSO gameManager;
    public Button resumeButton;
    public Button restartButton;
    public Button quitButton;
    public GameObject endMenu;

    public Button pauseIcon, pauseIconSecundary;

    private void OnEnable()
    {
        gameManager.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        gameManager.OnGameOver -= HandleGameOver;
    }

    void Start()
    {
        resumeButton.onClick.AddListener(OnResumeButtonClicked);
        restartButton.onClick.AddListener(OnRestartButtonClicked);
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

    private void HandleGameOver()
    {
        gameManager.ExponentialPause(2f);
        endMenu.SetActive(true);
    }
}
