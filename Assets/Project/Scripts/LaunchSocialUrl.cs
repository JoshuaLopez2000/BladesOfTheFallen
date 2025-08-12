using UnityEngine;
using UnityEngine.UI;

public class LaunchSocialUrl : MonoBehaviour
{
    public Button linkedinButton, githubButton;
    private string linkedinUrl = "https://www.linkedin.com/in/joshua-iv%C3%A1n-l%C3%B3pez-nava-77311721a/";
    private string githubUrl = "https://github.com/JoshuaLopez2000";

    void Start()
    {
        linkedinButton.onClick.AddListener(OpenLinkedInProfile);
        githubButton.onClick.AddListener(OpenGitHubProfile);
    }
    public void OpenLinkedInProfile()
    {
        Application.OpenURL(linkedinUrl);
    }

    public void OpenGitHubProfile()
    {
        Application.OpenURL(githubUrl);
    }
}
