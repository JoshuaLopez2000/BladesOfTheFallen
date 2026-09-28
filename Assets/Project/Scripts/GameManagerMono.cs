using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class GameManagerMono : MonoBehaviour
{
    [SerializeField] private GameManagerSO gameManager;

    [Header("Combat Time Effects")]
    [Tooltip("Maps normalized parry-effect time to game speed: 1 is normal and 0 is the configured slow factor.")]
    [SerializeField] private AnimationCurve parryTimeScaleCurve = new(
        new Keyframe(0f, 1f, 0f, -8f),
        new Keyframe(0.2f, 0f, 0f, 0f),
        new Keyframe(0.45f, 0f, 0f, 0f),
        new Keyframe(1f, 1f, 2f, 0f));

    private Coroutine timeScaleCoroutine;
    private static GameManagerMono activeInstance;

    private void Awake()
    {
        if (activeInstance != null && activeInstance != this)
        {
            enabled = false;
            Destroy(gameObject);
            return;
        }

        activeInstance = this;

        if (gameManager == null)
        {
            Debug.LogError("GameManagerMono requires a GameManagerSO reference.", this);
            enabled = false;
            return;
        }

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

    private void OnEnable()
    {
        if (gameManager == null)
        {
            Debug.LogError("GameManagerMono requires a GameManagerSO reference.", this);
            enabled = false;
            return;
        }

        gameManager.PauseRequested += HandlePauseRequested;
        gameManager.ResumeRequested += HandleResumeRequested;
        gameManager.HitStopRequested += HandleHitStopRequested;
        gameManager.ParryTimeEffectRequested += HandleParryTimeEffectRequested;
        gameManager.RestartRequested += RestartCurrentScene;
        gameManager.MainMenuRequested += GoToMainMenu;
    }

    private void OnDisable()
    {
        if (gameManager == null)
        {
            return;
        }

        gameManager.PauseRequested -= HandlePauseRequested;
        gameManager.ResumeRequested -= HandleResumeRequested;
        gameManager.HitStopRequested -= HandleHitStopRequested;
        gameManager.ParryTimeEffectRequested -= HandleParryTimeEffectRequested;
        gameManager.RestartRequested -= RestartCurrentScene;
        gameManager.MainMenuRequested -= GoToMainMenu;
    }

    private void OnDestroy()
    {
        if (activeInstance == this)
        {
            activeInstance = null;
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

    public void PlayParryTimeEffect(float slowFactor, float duration, System.Action<float> callback = null)
    {
        StartTimeScaleTransition(ParryTimeEffectCoroutine(slowFactor, duration, callback));
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

    private IEnumerator ParryTimeEffectCoroutine(float slowFactor, float duration, System.Action<float> callback)
    {
        if (duration <= 0f)
        {
            SetTimeScale(1f, callback);
            timeScaleCoroutine = null;
            yield break;
        }

        slowFactor = Mathf.Clamp01(slowFactor);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float curveValue = parryTimeScaleCurve == null || parryTimeScaleCurve.length == 0
                ? progress
                : Mathf.Clamp01(parryTimeScaleCurve.Evaluate(progress));
            SetTimeScale(Mathf.Lerp(slowFactor, 1f, curveValue), callback);
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

    private void HandlePauseRequested(float duration)
    {
        Pause(duration, gameManager.ReportTimeScale);
    }

    private void HandleResumeRequested(float duration)
    {
        Resume(duration, gameManager.ReportTimeScale);
    }

    private void HandleHitStopRequested(float slowFactor, float duration)
    {
        HitStop(slowFactor, duration, gameManager.ReportTimeScale);
    }

    private void HandleParryTimeEffectRequested(float slowFactor, float duration)
    {
        PlayParryTimeEffect(slowFactor, duration, gameManager.ReportTimeScale);
    }

    private void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void GoToMainMenu()
    {
        gameManager.ResetSession();
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync("MainScreen", LoadSceneMode.Single);
    }
}
