using System;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    private const string FileName = "audioSettings.json";

    [SerializeField] private Slider mainVolume;
    [SerializeField] private Slider musicVolume;
    [FormerlySerializedAs("SFxVolume")]
    [SerializeField] private Slider sfxVolume;
    [SerializeField] private AudioMixer globalAudioMixer;

    private AudioVolumes volumes = new();

    private void Start()
    {
        LoadVolumes();
    }

    private void OnEnable()
    {
        mainVolume.onValueChanged.AddListener(OnMainVolumeChange);
        musicVolume.onValueChanged.AddListener(OnMusicVolumeChange);
        sfxVolume.onValueChanged.AddListener(OnSfxVolumeChange);
    }

    private void OnDisable()
    {
        mainVolume.onValueChanged.RemoveListener(OnMainVolumeChange);
        musicVolume.onValueChanged.RemoveListener(OnMusicVolumeChange);
        sfxVolume.onValueChanged.RemoveListener(OnSfxVolumeChange);
    }

    private void OnMainVolumeChange(float volume)
    {
        SetMixerVolume("MasterVolume", volume);
        volumes.main = volume;
        SaveVolumes();
    }

    private void OnMusicVolumeChange(float volume)
    {
        SetMixerVolume("MusicVolume", volume);
        volumes.music = volume;
        SaveVolumes();
    }

    private void OnSfxVolumeChange(float volume)
    {
        SetMixerVolume("SFxVolume", volume);
        volumes.SFx = volume;
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
        string fullPath = Path.Combine(Application.persistentDataPath, FileName);

        try
        {
            string json = JsonUtility.ToJson(volumes);
            File.WriteAllText(fullPath, json);
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Could not save audio settings: {exception.Message}", this);
        }
    }

    private void LoadVolumes()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, FileName);

        if (File.Exists(fullPath))
        {
            try
            {
                string volumeContent = File.ReadAllText(fullPath);
                volumes = JsonUtility.FromJson<AudioVolumes>(volumeContent) ?? new AudioVolumes();
            }
            catch (Exception exception)
            {
                volumes = new AudioVolumes();
                Debug.LogWarning($"Could not load audio settings: {exception.Message}", this);
            }
        }

        mainVolume.SetValueWithoutNotify(volumes.main);
        SetMixerVolume("MasterVolume", volumes.main);

        musicVolume.SetValueWithoutNotify(volumes.music);
        SetMixerVolume("MusicVolume", volumes.music);

        sfxVolume.SetValueWithoutNotify(volumes.SFx);
        SetMixerVolume("SFxVolume", volumes.SFx);
    }
}

[Serializable]
internal sealed class AudioVolumes
{
    public float main, music, SFx;

    public AudioVolumes()
    {
        main = 0.75f;
        music = 0.5f;
        SFx = 0.75f;
    }
}
