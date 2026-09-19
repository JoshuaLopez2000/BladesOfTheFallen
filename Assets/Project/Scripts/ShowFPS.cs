using TMPro;
using UnityEngine;

public class ShowFPS : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField, Min(0.1f)] private float refreshInterval = 0.25f;

    private float elapsedTime;
    private int frameCount;

    private void Update()
    {
        elapsedTime += Time.unscaledDeltaTime;
        frameCount++;

        if (elapsedTime < refreshInterval)
        {
            return;
        }

        if (fpsText != null)
        {
            float framesPerSecond = frameCount / elapsedTime;
            fpsText.text = $"FPS: {framesPerSecond:0}";
        }

        elapsedTime = 0f;
        frameCount = 0;
    }
}
