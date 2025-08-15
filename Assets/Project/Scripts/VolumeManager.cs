using System;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    private string fileName = "audioSettings.json";
    public Slider mainVolume;
    public Slider musicVolume;
    public Slider SFxVolume;
    public AudioMixer globalAudioMixer;

    private AudioVolumes tmpVolumes;

    void Start()
    {
        LoadVolumes();
    }

    private void OnEnable()
    {
        mainVolume.onValueChanged.AddListener(OnMainVolumeChange);
        musicVolume.onValueChanged.AddListener(OnMusicVolumeChange);
        SFxVolume.onValueChanged.AddListener(OnSFxVolumeChange);
    }

    private void OnDisable()
    {
        mainVolume.onValueChanged.RemoveListener(OnMainVolumeChange);
        musicVolume.onValueChanged.RemoveListener(OnMusicVolumeChange);
        SFxVolume.onValueChanged.RemoveListener(OnSFxVolumeChange);
    }

    public void OnMainVolumeChange(float volume)
    {
        SetMixerVolume("MasterVolume", volume);
        tmpVolumes.main = volume;
    }

    public void OnMusicVolumeChange(float volume)
    {
        SetMixerVolume("MusicVolume", volume);
        tmpVolumes.music = volume;
        SaveVolumes();
    }

    public void OnSFxVolumeChange(float volume)
    {
        SetMixerVolume("SFxVolume", volume);
        tmpVolumes.SFx = volume;
        SaveVolumes();
    }

    private void SetMixerVolume(string parameterName, float sliderValue)
    {
        float safeValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        float dB = Mathf.Log10(safeValue) * 20f;
        globalAudioMixer.SetFloat(parameterName, dB);
    }

    private void SaveVolumes()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, fileName);
        string tmpJson = JsonUtility.ToJson(tmpVolumes);
        File.WriteAllText(fullPath, tmpJson);
    }

    private void LoadVolumes()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, fileName);

        if (File.Exists(fullPath))
        {
            string volumeContent = File.ReadAllText(fullPath);
            tmpVolumes = JsonUtility.FromJson<AudioVolumes>(volumeContent);
        }
        else
        {
            tmpVolumes = new AudioVolumes();
        }

        mainVolume.value = tmpVolumes.main;
        SetMixerVolume("MasterVolume", tmpVolumes.main);

        musicVolume.value = tmpVolumes.music;
        SetMixerVolume("MusicVolume", tmpVolumes.music);

        SFxVolume.value = tmpVolumes.SFx;
        SetMixerVolume("SFxVolume", tmpVolumes.SFx);
    }
}

[Serializable]
public class AudioVolumes
{
    public float main, music, SFx;

    public AudioVolumes()
    {
        main = 0.75f;
        music = 0.75f;
        SFx = 0.75f;
    }
}
