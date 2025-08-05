using TMPro;
using UnityEngine;

public class ShowFPS : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float fps = 1.0f / Time.deltaTime;
        fpsText.text = "FPS: " + Mathf.Round(fps);
    }
}
