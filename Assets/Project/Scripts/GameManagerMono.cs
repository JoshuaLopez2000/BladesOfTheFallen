using System.Collections;
using UnityEngine;

public class GameManagerMono : MonoBehaviour
{
    public static GameManagerMono Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public IEnumerator ExponentialPauseCoroutine(float duration, System.Action<float> callback = null)
    {
        float t = 0f;
        float startScale = Time.timeScale;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Exp(-5f * (t / duration));
            callback?.Invoke(Time.timeScale);
            yield return null;
        }

        Time.timeScale = 0f;
        callback?.Invoke(Time.timeScale);
    }

    public IEnumerator ExponentialResumeCoroutine(float duration, System.Action<float> callback = null)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            Time.timeScale = 1f - Mathf.Exp(-5f * (t / duration));
            callback?.Invoke(Time.timeScale);
            yield return null;
        }

        Time.timeScale = 1f;
        callback?.Invoke(Time.timeScale);
    }


    public IEnumerator HitTimeCoroutine(float slowFactor, float duration, System.Action<float> callback = null)
    {
        float halfDuration = duration / 2f;
        float t = 0f;

        while (t < halfDuration)
        {
            t += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(1f, slowFactor, t / halfDuration);
            callback?.Invoke(Time.timeScale);
            yield return null;
        }

        t = 0f;
        while (t < halfDuration)
        {
            t += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(slowFactor, 1f, t / halfDuration);
            callback?.Invoke(Time.timeScale);
            yield return null;
        }

        Time.timeScale = 1f;
        callback?.Invoke(Time.timeScale);
    }
}
