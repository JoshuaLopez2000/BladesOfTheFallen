using UnityEngine;

public class PauseFade : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadePauseCanvas;
    [SerializeField] private CanvasGroup fadeHudCanvas;
    [SerializeField, Min(0f)] private float fadeSpeed = 2f;

    private bool isPaused;
    private float targetPauseAlpha;
    private float targetHudAlpha = 1f;

    private void Start()
    {
        fadePauseCanvas.alpha = 0f;
        fadePauseCanvas.blocksRaycasts = false;
    }

    private void Update()
    {
        fadePauseCanvas.alpha = Mathf.MoveTowards(fadePauseCanvas.alpha, targetPauseAlpha, fadeSpeed * Time.unscaledDeltaTime);
        fadeHudCanvas.alpha = Mathf.MoveTowards(fadeHudCanvas.alpha, targetHudAlpha, fadeSpeed * Time.unscaledDeltaTime);

        fadePauseCanvas.blocksRaycasts = fadePauseCanvas.alpha > 0.01f;
        fadeHudCanvas.blocksRaycasts = fadeHudCanvas.alpha > 0.01f;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        targetPauseAlpha = isPaused ? 1f : 0f;
        targetHudAlpha = isPaused ? 0f : 1f;
    }
}
