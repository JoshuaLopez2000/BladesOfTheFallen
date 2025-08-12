using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

public class MainScreenUIManager : MonoBehaviour
{
    public GameObject configUI, creditsUI;
    public Button configButton, creditsButton, playButton;
    public List<Button> backButtons;
    public String mainScene;

    void Start()
    {
        configButton.onClick.AddListener(ShowConfig);
        creditsButton.onClick.AddListener(ShowCredits);
        playButton.onClick.AddListener(ChangeScreen);

        foreach (Button backButton in backButtons)
        {
            backButton.onClick.AddListener(ShowMain);
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

    void ChangeScreen()
    {
        SceneManager.LoadScene(mainScene, LoadSceneMode.Single);
    }
}
