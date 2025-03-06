using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UI_Audio : MonoBehaviour
{
    [SerializeField] private UIDocument Ui_Options;
    [SerializeField] private InputActionAsset inputActions;

    private VisualElement optionsMenu;
    private VisualElement AudioMenu;

    private DropdownField resolutionDropdown;
    private DropdownField qualityDropdown;
    private Toggle fullscreenToggle;

    private AudioMixer audioMixer;

    private Slider masterVolumeSlider;
    private Slider sfxVolumeSlider;
    private Slider musicVolumeSlider;

    private Button backButton;
    private Button ButtonControl;

    private InputAction cancelAction;

    private Resolution[] resolutions;

    private void OnEnable()
    {

        var root = Ui_Options.rootVisualElement;

        optionsMenu = root.Q<VisualElement>("optionsMenu");
        optionsMenu.style.display = DisplayStyle.None;

        AudioMenu = root.Q<VisualElement>("UI_Audio");
        AudioMenu.style.display = DisplayStyle.None;

        ButtonControl = root.Q<Button>("Audio");
        ButtonControl.clicked += OpenControl;


        backButton = root.Q<Button>("BackButton");
        backButton.clicked += CloseOptions;

        var uiActionMap = inputActions.FindActionMap("UI");
        cancelAction = uiActionMap.FindAction("Cancel");
        cancelAction.performed += ctx => CloseOptions();
        cancelAction.Enable();

        resolutionDropdown = root.Q<DropdownField>("ResolutionDropdown");
        qualityDropdown = root.Q<DropdownField>("FullscreenToggle");
        fullscreenToggle = root.Q<Toggle>("QualityDropdown");

        resolutionDropdown.RegisterValueChangedCallback(evt => SetResolution(evt.newValue));
        fullscreenToggle.RegisterValueChangedCallback(evt => SetFullscreen(evt.newValue));
        qualityDropdown.RegisterValueChangedCallback(evt => SetQuality(evt.newValue));

        PopulateResolutionDropdown();
        LoadGraphicsSettings();

        // Audio
        masterVolumeSlider = root.Q<Slider>("MasterVolumeSlider");
        sfxVolumeSlider = root.Q<Slider>("SFXVolumeSlider");
        musicVolumeSlider = root.Q<Slider>("MusicVolumeSlider");

        masterVolumeSlider.RegisterValueChangedCallback(evt => SetMasterVolume(evt.newValue));
        sfxVolumeSlider.RegisterValueChangedCallback(evt => SetSFXVolume(evt.newValue));
        musicVolumeSlider.RegisterValueChangedCallback(evt => SetMusicVolume(evt.newValue));

        LoadAudioSettings();
    }

    private void OnDisable()
    {
        gameObject.SetActive(false);

        if (cancelAction != null)
        {
            cancelAction.Disable();
        }
    }

    public void OpenControl()
    {
        optionsMenu.style.display = DisplayStyle.None;
        AudioMenu.style.display = DisplayStyle.Flex;
    }

    private void CloseOptions()
    {
        AudioMenu.style.display = DisplayStyle.None;
        optionsMenu.style.display = DisplayStyle.Flex;

    }

    private void PopulateResolutionDropdown()
    {
        Resolution[] resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
                break;
            }
        }
    }

    private void SetResolution(string resolution)
    {
        string[] resParts = resolution.Split('x');
        int width = int.Parse(resParts[0].Trim());
        int height = int.Parse(resParts[1].Trim());

        Screen.SetResolution(width, height, Screen.fullScreen);
        PlayerPrefs.SetString("Resolution", resolution);
    }

    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    private void SetQuality(string quality)
    {
        QualitySettings.SetQualityLevel(qualityDropdown.choices.IndexOf(quality));
        PlayerPrefs.SetString("Quality", quality);
    }

    private void LoadGraphicsSettings()
    {
        if (PlayerPrefs.HasKey("Resolution"))
            SetResolution(PlayerPrefs.GetString("Resolution"));

        if (PlayerPrefs.HasKey("Fullscreen"))
            fullscreenToggle.value = PlayerPrefs.GetInt("Fullscreen") == 1;

        if (PlayerPrefs.HasKey("Quality"))
            qualityDropdown.value = PlayerPrefs.GetString("Quality");
    }

    // Settings Volume 
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


}
