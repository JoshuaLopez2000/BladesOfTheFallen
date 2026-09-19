using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MainScreenUIManager : MonoBehaviour
{
    [SerializeField] private GameObject configUI;
    [SerializeField] private GameObject creditsUI;
    [SerializeField] private Button configButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button playButton;
    [SerializeField] private List<Button> backButtons = new();
    [SerializeField] private string mainScene;

    private void Start()
    {
        configButton.onClick.AddListener(ShowConfig);
        creditsButton.onClick.AddListener(ShowCredits);
        playButton.onClick.AddListener(ChangeScreen);

        foreach (Button backButton in backButtons)
        {
            backButton.onClick.AddListener(ShowMain);
        }
    }

    private void OnDestroy()
    {
        configButton.onClick.RemoveListener(ShowConfig);
        creditsButton.onClick.RemoveListener(ShowCredits);
        playButton.onClick.RemoveListener(ChangeScreen);

        foreach (Button backButton in backButtons)
        {
            backButton.onClick.RemoveListener(ShowMain);
        }
    }

    private void ShowConfig()
    {
        configUI.SetActive(true);
        creditsUI.SetActive(false);
    }

    private void ShowCredits()
    {
        configUI.SetActive(false);
        creditsUI.SetActive(true);
    }

    private void ShowMain()
    {
        configUI.SetActive(false);
        creditsUI.SetActive(false);
    }

    private void ChangeScreen()
    {
        SceneManager.LoadSceneAsync(mainScene, LoadSceneMode.Single);
    }
}
