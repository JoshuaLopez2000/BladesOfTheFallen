using UnityEngine;

public class GameConfig : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        QualitySettings.vSyncCount = 0; // Desactiva VSync
        Application.targetFrameRate = 60; // Establece el objetivo de FPS
    }

}
