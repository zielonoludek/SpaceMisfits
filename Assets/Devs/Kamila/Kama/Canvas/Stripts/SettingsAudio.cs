using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsAudio : MonoBehaviour
{
    [SerializeField] private GameObject settingsAudio;
    [SerializeField] private AudioMixer audioMixer;

    private Slider masterVolumeSlider;
    private Slider sfxVolumeSlider;
    private Slider musicVolumeSlider;

    private void Awake()
    {

        masterVolumeSlider = GameObject.Find("MasterVolumeSlider").GetComponent<Slider>();
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);

        sfxVolumeSlider = GameObject.Find("SFXVolumeSlider").GetComponent<Slider>();
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

        musicVolumeSlider = GameObject.Find("MusicVolumeSlider").GetComponent<Slider>();
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);

        LoadAudioSettings();
    }

    private void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }
    private void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    private void LoadAudioSettings()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
            masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");

        if (PlayerPrefs.HasKey("SFXVolume"))
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume");

        if (PlayerPrefs.HasKey("MusicVolume"))
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
    }

    public void OpenAudio()
    {
        settingsAudio.SetActive(true);
    }

    public void CloseAudio()
    {
        settingsAudio.SetActive(false);
    }
}
