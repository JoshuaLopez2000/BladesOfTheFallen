using UnityEngine;

public class PauseFade : MonoBehaviour
{
    public CanvasGroup fadePauseCanvas, fadeHudCanvas;
    public float fadeSpeed = 2f;

    private bool isPaused = false;
    private float targetPauseAlpha = 0f, targetHudAlpha = 1f;

    void Start()
    {
        fadePauseCanvas.alpha = 0f;
        fadePauseCanvas.blocksRaycasts = false;
    }

    void Update()
    {
        fadePauseCanvas.alpha = Mathf.MoveTowards(fadePauseCanvas.alpha, targetPauseAlpha, fadeSpeed * Time.unscaledDeltaTime);
        fadeHudCanvas.alpha = Mathf.MoveTowards(fadeHudCanvas.alpha, targetHudAlpha, fadeSpeed * Time.unscaledDeltaTime);

        fadePauseCanvas.blocksRaycasts = fadePauseCanvas.alpha > 0.01f;
        fadeHudCanvas.blocksRaycasts = fadeHudCanvas.alpha > 0.01f;
    }

    public void TogglePause()
    {
        Debug.Log("Toggling Pause");
        isPaused = !isPaused;
        targetPauseAlpha = isPaused ? 1f : 0f;
        targetHudAlpha = isPaused ? 0f : 1f;
        // Time.timeScale = isPaused ? 0f : 1f;
    }
}
