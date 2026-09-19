using UnityEngine;

public class GameConfig : MonoBehaviour
{
    [SerializeField, Min(-1)] private int targetFrameRate = 60;
    [SerializeField] private bool disableVSync = true;

    private void Awake()
    {
        if (disableVSync)
        {
            QualitySettings.vSyncCount = 0;
        }

        Application.targetFrameRate = targetFrameRate;
    }
}
