using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.Timeline.DirectorControlPlayable;
using UnityEditor.Timeline.Actions;

public class UI_Settings : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private InputActionReference moveActionReference;
    [SerializeField] private AudioMixer audioMixer;

    private UI_Settings ui_Settings;
    private UI_MainMenu ui_mainMenu;
    private UI_Pause ui_Pause;

    //private Button rebindButton;
    //private Text rebindLabel;
    //private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private Button backButton;
    //private Button resetButton;
    //private Button allResetButton;

    private InputAction clickAction;
    private InputAction backAction;

    //private Dropdown resolutionDropdown;
    //private Dropdown qualityDropdown;
    //private Toggle fullscreenToggle;

    //private Slider masterVolumeSlider;
    //private Slider sfxVolumeSlider;
    //private Slider musicVolumeSlider;

    //private Resolution[] resolutions;
    private bool openedFromPause = false;

    private void Awake()
    {
        ui_mainMenu = FindFirstObjectByType<UI_MainMenu>(FindObjectsInactive.Include);
        ui_Pause = FindFirstObjectByType<UI_Pause>(FindObjectsInactive.Include);
        // Pobierz obiekty UI
        backButton = GameObject.Find("Back").GetComponent<Button>();
        //resetButton = GameObject.Find("ResetButton").GetComponent<Button>();
        //allResetButton = GameObject.Find("AllResetButton").GetComponent<Button>();

        //rebindButton = GameObject.Find("RebindButton").GetComponent<Button>();
        //rebindLabel = GameObject.Find("RebindLabel").GetComponent<Text>();

        //resolutionDropdown = GameObject.Find("ResolutionDropdown").GetComponent<Dropdown>();
        //qualityDropdown = GameObject.Find("QualityDropdown").GetComponent<Dropdown>();
        //fullscreenToggle = GameObject.Find("FullscreenToggle").GetComponent<Toggle>();

        //masterVolumeSlider = GameObject.Find("MasterVolumeSlider").GetComponent<Slider>();
        //sfxVolumeSlider = GameObject.Find("SFXVolumeSlider").GetComponent<Slider>();
        //musicVolumeSlider = GameObject.Find("MusicVolumeSlider").GetComponent<Slider>();

        // Ustawienia przycisków
        backButton.onClick.AddListener(CloseOptions);
        //resetButton.onClick.AddListener(ResetSelectedRebinds);
        //allResetButton.onClick.AddListener(AllResetRebinds);
        //rebindButton.onClick.AddListener(StartRebinding);

        //resolutionDropdown.onValueChanged.AddListener(delegate { SetResolution(resolutionDropdown.options[resolutionDropdown.value].text); });
        //fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        //qualityDropdown.onValueChanged.AddListener(delegate { SetQuality(qualityDropdown.options[qualityDropdown.value].text); });

        //masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        //sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        //musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);

        /*var uiActionMap = inputActions.FindActionMap("UI");
        clickAction = uiActionMap.FindAction("Click");
        backAction = uiActionMap.FindAction("Cancel");

        backAction.performed += ctx => CloseOptions();

        inputActions.Enable();
        clickAction.Enable();
        backAction.Enable();
        // Wczytanie ustawień
        /*LoadAudioSettings();
        PopulateResolutionDropdown();
        PopulateQualityDropdown();
        LoadGraphicsSettings();
        */
    }
    public void OpenFromMainMenu()
    {
        settingsMenu.SetActive(true);
        openedFromPause = false;
    }

    public void OpenFromPauseMenu()
    {
        settingsMenu.SetActive(true);
        openedFromPause = true;
    }

    private void CloseOptions()
    {
        if(openedFromPause)
        {
            ui_Pause.PauseAction();
            ui_Pause.ShowPauseMenu();
            settingsMenu.SetActive(false);
        }
        else
        {
            ui_mainMenu.ShowMainMenu();
            settingsMenu.SetActive(false);
        }
    }

    /*private void StartRebinding()
    {
        var action = moveActionReference.action;
        if (action == null) return;

        action.Disable();
        rebindLabel.text = "Press a key...";

        rebindingOperation = action.PerformInteractiveRebinding()
            .WithControlsExcluding("Move")
            .OnMatchWaitForAnother(0.1f)
            .OnCancel(operation =>
            {
                Debug.LogWarning("Fail");
                operation.Dispose();
                action.Enable();
                UpdateRebindLabel();
            })
            .OnComplete(operation =>
            {
                Debug.Log("Success");
                operation.Dispose();
                SaveRebinds();
                UpdateRebindLabel();
                action.Enable();
            })
            .Start();
    }

    private void SaveRebinds()
    {
        var action = moveActionReference.action;
        string rebindData = action.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebind_" + action.id, rebindData);
        PlayerPrefs.Save();
    }

    private void LoadRebinds()
    {
        var action = moveActionReference.action;
        string rebindData = PlayerPrefs.GetString("rebind_" + action.id);
        action.LoadBindingOverridesFromJson(rebindData);
        UpdateRebindLabel();
    }

    private void UpdateRebindLabel()
    {
        var action = moveActionReference.action;
        rebindLabel.text = action.GetBindingDisplayString();
    }

    private void ResetSelectedRebinds()
    {
        var action = moveActionReference.action;
        action.RemoveAllBindingOverrides();
        SaveRebinds();
        UpdateRebindLabel();
    }

    private void AllResetRebinds()
    {
        var action = moveActionReference.action;
        action.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey("rebind_" + action.id);
        PlayerPrefs.Save();
        UpdateRebindLabel();
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
    */

    public void MainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
