using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsGraphic : MonoBehaviour
{
    [SerializeField] GameObject settingsGraphic;

    private TMP_Dropdown resolutionDropdown;
    private TMP_Dropdown qualityDropdown;
    private Toggle fullscreenToggle;

    private Resolution[] resolutions;

    private void Awake()
    {
        resolutionDropdown = GameObject.Find("ResolutionDropdown").GetComponent<TMP_Dropdown>();
        qualityDropdown = GameObject.Find("QualityDropdown").GetComponent<TMP_Dropdown>();
        fullscreenToggle = GameObject.Find("FullscreenToggle").GetComponent<Toggle>();

        resolutionDropdown.onValueChanged.AddListener(delegate { SetResolution(resolutionDropdown.options[resolutionDropdown.value].text); });
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        qualityDropdown.onValueChanged.AddListener(delegate { SetQuality(qualityDropdown.options[qualityDropdown.value].text); });


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
        resolutionDropdown.value = currentResolutionIndex;
    }

    private void PopulateQualityDropdown()
    {
        qualityDropdown.ClearOptions();
        List<string> qualityOptions = new List<string>(QualitySettings.names);
        qualityDropdown.AddOptions(qualityOptions);

        int currentQualityIndex = QualitySettings.GetQualityLevel();
        qualityDropdown.value = currentQualityIndex;
    }

    private void SetResolution(string resolution)
    {
        string[] resParts = resolution.Split('x');
        if (resParts.Length != 2) return;

        int width = int.Parse(resParts[0].Trim());
        int height = int.Parse(resParts[1].Trim());

        Screen.SetResolution(width, height, Screen.fullScreen);
        PlayerPrefs.SetString("Resolution", resolution);
        PlayerPrefs.Save();
    }

    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void SetQuality(string quality)
    {
        int qualityIndex = qualityDropdown.options.FindIndex(option => option.text == quality);
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetString("Quality", quality);
        PlayerPrefs.Save();
    }

    private void LoadGraphicsSettings()
    {
        if (PlayerPrefs.HasKey("Resolution"))
            SetResolution(PlayerPrefs.GetString("Resolution"));

        if (PlayerPrefs.HasKey("Fullscreen"))
            fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen") == 1;

        if (PlayerPrefs.HasKey("Quality"))
            qualityDropdown.value = qualityDropdown.options.FindIndex(option => option.text == PlayerPrefs.GetString("Quality"));
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
