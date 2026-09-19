using UnityEngine;
using UnityEngine.UI;

public class LaunchSocialUrl : MonoBehaviour
{
    private const string LinkedInUrl = "https://www.linkedin.com/in/joshua-iv%C3%A1n-l%C3%B3pez-nava-77311721a/";
    private const string GitHubUrl = "https://github.com/JoshuaLopez2000";

    [SerializeField] private Button linkedinButton;
    [SerializeField] private Button githubButton;

    private void Start()
    {
        linkedinButton.onClick.AddListener(OpenLinkedInProfile);
        githubButton.onClick.AddListener(OpenGitHubProfile);
    }

    private void OnDestroy()
    {
        linkedinButton.onClick.RemoveListener(OpenLinkedInProfile);
        githubButton.onClick.RemoveListener(OpenGitHubProfile);
    }

    private static void OpenLinkedInProfile()
    {
        Application.OpenURL(LinkedInUrl);
    }

    private static void OpenGitHubProfile()
    {
        Application.OpenURL(GitHubUrl);
    }
}
