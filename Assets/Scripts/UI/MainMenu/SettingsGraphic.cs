using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsGraphic : MonoBehaviour
{
    [SerializeField] private GameObject settingsGraphic;

    private Dropdown resolutionDropdown;
    private Dropdown qualityDropdown;
    private Toggle fullscreenToggle;

    private Resolution[] resolutions;

    private void Awake()
    {
        resolutionDropdown = GameObject.Find("ResolutionDropdown").GetComponent<Dropdown>();
        qualityDropdown = GameObject.Find("QualityDropdown").GetComponent<Dropdown>();
        fullscreenToggle = GameObject.Find("FullscreenToggle").GetComponent<Toggle>();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        qualityDropdown.onValueChanged.AddListener(SetQuality);

        PopulateResolutionDropdown();
        PopulateQualityDropdown();
        LoadGraphicsSettings();
    }

    private void PopulateResolutionDropdown()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> resolutionOptions = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string resolution = resolutions[i].width + "x" + resolutions[i].height;
            resolutionOptions.Add(resolution);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(resolutionOptions);

        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionIndex");
        }
        else
        {
            resolutionDropdown.value = currentResolutionIndex;
        }

        resolutionDropdown.RefreshShownValue();
    }

    private void PopulateQualityDropdown()
    {
        qualityDropdown.ClearOptions();
        List<string> qualityOptions = new List<string>(QualitySettings.names);
        qualityDropdown.AddOptions(qualityOptions);

        int currentQualityIndex = QualitySettings.GetQualityLevel();
        qualityDropdown.value = PlayerPrefs.GetInt("QualityIndex", currentQualityIndex);
    }

    private void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.SetString("Resolution", resolution.width + "x" + resolution.height);
        PlayerPrefs.Save();
    }

    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityIndex", qualityIndex);
        PlayerPrefs.Save();
    }

    private void LoadGraphicsSettings()
    {
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionIndex");
            resolutionDropdown.RefreshShownValue();
        }

        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen") == 1;
        }

        if (PlayerPrefs.HasKey("QualityIndex"))
        {
            qualityDropdown.value = PlayerPrefs.GetInt("QualityIndex");
        }
    }

    public void OpenGraphic()
    {
        settingsGraphic.SetActive(true);
    }

    public void CloseGraphic()
    {
        settingsGraphic.SetActive(false);
    }
}
