using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuPauseManager : MonoBehaviour
{
    [SerializeField] private GameManagerSO gameManager;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameObject endMenu;
    [SerializeField] private Button pauseIcon;
    [FormerlySerializedAs("pauseIconSecundary")]
    [SerializeField] private Button secondaryPauseIcon;

    private void OnEnable()
    {
        gameManager.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        gameManager.OnGameOver -= HandleGameOver;
    }

    private void Start()
    {
        resumeButton.onClick.AddListener(OnResumeButtonClicked);
        restartButton.onClick.AddListener(OnRestartButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
        pauseIcon.onClick.AddListener(OnResumeButtonClicked);
        secondaryPauseIcon.onClick.AddListener(OnResumeButtonClicked);
    }

    private void OnDestroy()
    {
        resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
        restartButton.onClick.RemoveListener(OnRestartButtonClicked);
        quitButton.onClick.RemoveListener(OnQuitButtonClicked);
        pauseIcon.onClick.RemoveListener(OnResumeButtonClicked);
        secondaryPauseIcon.onClick.RemoveListener(OnResumeButtonClicked);
    }

    private void OnResumeButtonClicked()
    {
        if (gameManager.CurrentState == GameManagerSO.GameState.Paused)
        {
            gameManager.ChangeState(GameManagerSO.GameState.Playing);
            gameManager.ExponentialResume(0.5f);
        }
        else if (gameManager.CurrentState == GameManagerSO.GameState.Playing)
        {
            gameManager.ChangeState(GameManagerSO.GameState.Paused);
            gameManager.ExponentialPause(0.5f);
        }
    }

    private void OnRestartButtonClicked()
    {
        gameManager.ResetGame();
    }

    private void OnQuitButtonClicked()
    {
        if (GameManagerMono.Instance != null)
        {
            GameManagerMono.Instance.GoToMainMenu();
        }
    }

    private void HandleGameOver()
    {
        gameManager.ExponentialPause(2f);
        endMenu.SetActive(true);
    }
}
