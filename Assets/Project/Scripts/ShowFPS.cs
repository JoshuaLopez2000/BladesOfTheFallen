using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowFPS : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    private List<float> fpsHistory = new List<float>();
    // Update is called once per frame
    void Update()
    {
        //average frames per second
        float fps = 1.0f / Time.deltaTime;
        fpsHistory.Add(fps);
        if (fpsHistory.Count > 100)
        {
            fpsHistory.RemoveAt(0);
        }
        float averageFps = 0;
        foreach (float f in fpsHistory)
        {
            averageFps += f;
        }
        averageFps /= fpsHistory.Count;
        fpsText.text = "FPS: " + Mathf.Round(averageFps);
    }
}
