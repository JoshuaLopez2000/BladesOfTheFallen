using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class GameManagerMono : MonoBehaviour
{
    [SerializeField] private GameManagerSO gameManager;

    private Coroutine timeScaleCoroutine;

    public static GameManagerMono Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        switch (gameManager.CurrentState)
        {
            case GameManagerSO.GameState.Init:
                break;
            case GameManagerSO.GameState.Playing:
                Resume(1f);
                break;
            case GameManagerSO.GameState.Paused:
                break;
            case GameManagerSO.GameState.GameOver:
                break;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void Pause(float duration, System.Action<float> callback = null)
    {
        StartTimeScaleTransition(ExponentialPauseCoroutine(duration, callback));
    }

    public void Resume(float duration, System.Action<float> callback = null)
    {
        StartTimeScaleTransition(ExponentialResumeCoroutine(duration, callback));
    }

    public void HitStop(float slowFactor, float duration, System.Action<float> callback = null)
    {
        StartTimeScaleTransition(HitTimeCoroutine(slowFactor, duration, callback));
    }

    private void StartTimeScaleTransition(IEnumerator transition)
    {
        if (timeScaleCoroutine != null)
        {
            StopCoroutine(timeScaleCoroutine);
        }

        timeScaleCoroutine = StartCoroutine(transition);
    }

    private IEnumerator ExponentialPauseCoroutine(float duration, System.Action<float> callback)
    {
        if (duration <= 0f)
        {
            SetTimeScale(0f, callback);
            yield break;
        }

        float t = 0f;
        float startScale = Time.timeScale;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(t / duration);
            SetTimeScale(startScale * Mathf.Exp(-5f * progress), callback);
            yield return null;
        }

        SetTimeScale(0f, callback);
        timeScaleCoroutine = null;
    }

    private IEnumerator ExponentialResumeCoroutine(float duration, System.Action<float> callback)
    {
        if (duration <= 0f)
        {
            SetTimeScale(1f, callback);
            yield break;
        }

        float t = 0f;
        float startScale = Time.timeScale;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(t / duration);
            float easedProgress = 1f - Mathf.Exp(-5f * progress);
            SetTimeScale(Mathf.Lerp(startScale, 1f, easedProgress), callback);
            yield return null;
        }

        SetTimeScale(1f, callback);
        timeScaleCoroutine = null;
    }

    private IEnumerator HitTimeCoroutine(float slowFactor, float duration, System.Action<float> callback)
    {
        if (duration <= 0f)
        {
            SetTimeScale(1f, callback);
            yield break;
        }

        slowFactor = Mathf.Clamp01(slowFactor);
        float halfDuration = duration / 2f;
        float t = 0f;

        while (t < halfDuration)
        {
            t += Time.unscaledDeltaTime;
            SetTimeScale(Mathf.Lerp(1f, slowFactor, Mathf.Clamp01(t / halfDuration)), callback);
            yield return null;
        }

        t = 0f;
        while (t < halfDuration)
        {
            t += Time.unscaledDeltaTime;
            SetTimeScale(Mathf.Lerp(slowFactor, 1f, Mathf.Clamp01(t / halfDuration)), callback);
            yield return null;
        }

        SetTimeScale(1f, callback);
        timeScaleCoroutine = null;
    }

    private static void SetTimeScale(float value, System.Action<float> callback)
    {
        Time.timeScale = value;
        callback?.Invoke(value);
    }

    public void ResetGame()
    {
        gameManager.ResetGame();
    }

    public void GoToMainMenu()
    {
        gameManager.ResetSession();
        SceneManager.LoadSceneAsync("MainScreen", LoadSceneMode.Single);
    }
}
