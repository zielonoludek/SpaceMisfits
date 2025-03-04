using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class UI_Options : MonoBehaviour
{
    [SerializeField] private UIDocument Ui_Options;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private InputActionReference moveActionReference;

    private Button rebindButton;
    private Label rebindLabel;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private MainMenuEvents mainMenu;
    private PauseManager pauseMenu;
    private UI_Control ui_Control;
    private UI_Audio ui_Audio;

    private VisualElement optionsMenu;
    private VisualElement ControlMenu;

    private Button backButton;
    private Button ButtonControl;
    private bool openedFromPause = false;

    private InputAction cancelAction;

    private DropdownField resolutionDropdown;
    private DropdownField qualityDropdown;
    private Toggle fullscreenToggle;

    private AudioMixer audioMixer;

    private Slider masterVolumeSlider;
    private Slider sfxVolumeSlider;
    private Slider musicVolumeSlider;

    private Resolution[] resolutions;



    private void Awake()
    {
        mainMenu = FindFirstObjectByType<MainMenuEvents>(FindObjectsInactive.Include);
        pauseMenu = FindFirstObjectByType<PauseManager>(FindObjectsInactive.Include);
        ui_Control = FindFirstObjectByType<UI_Control>(FindObjectsInactive.Include);
        ui_Audio = FindFirstObjectByType<UI_Audio>(FindObjectsInactive.Include);
    }

    private void OnEnable()
    {

        var root = Ui_Options.rootVisualElement;

        mainMenu = GameObject.FindFirstObjectByType<MainMenuEvents>();
        pauseMenu = GameObject.FindFirstObjectByType<PauseManager>();


        optionsMenu = root.Q<VisualElement>("optionsMenu");
        optionsMenu.style.display = DisplayStyle.None;

        ControlMenu = root.Q<VisualElement>("Control");
        ControlMenu.style.display = DisplayStyle.None;

        ButtonControl = root.Q<Button>("Rebind");
        ButtonControl.clicked +=  OpenControl;


        backButton = root.Q<Button>("BackButton");
        backButton.clicked += CloseOptions;

        var uiActionMap = inputActions.FindActionMap("UI");
        cancelAction = uiActionMap.FindAction("Cancel");
        cancelAction.performed += ctx => CloseOptions();
        cancelAction.Enable();

       /* var resetButton = root.Q<Button>("ResetButton");
        resetButton.clicked += ResetSelectedRebinds;

        var allresetButton = root.Q<Button>("AllResetButton");
        allresetButton.clicked += AllResetRebinds;

        rebindButton = root.Q<Button>("RebindButton");
        rebindLabel = root.Q<Label>("RebindLabel");
        rebindButton.clicked += StartRebinding;

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
       */
    }

    private void OnDisable()
    {
        gameObject.SetActive(false);

        if (cancelAction != null)
        {
            cancelAction.Disable();
        }

      //  rebindButton.clicked -= StartRebinding;
        rebindingOperation?.Dispose();
    }

    public void OpenControl()
    {
        ui_Control = GameObject.FindFirstObjectByType<UI_Control>();
        optionsMenu.style.display = DisplayStyle.None;
        ControlMenu.style.display = DisplayStyle.Flex;
    }
    public void CloseControl()
    {
        optionsMenu.style.display = DisplayStyle.Flex;
        ControlMenu.style.display = DisplayStyle.None;
    }

    public void OpenFromMainMenu()
    {
        optionsMenu.style.display = DisplayStyle.Flex;
        openedFromPause = false;
    }

    public void OpenFromPauseMenu()
    {
        optionsMenu.style.display = DisplayStyle.Flex;
        openedFromPause = true;
    }


    private void CloseOptions()
    {
        optionsMenu.style.display = DisplayStyle.None;


        if (openedFromPause)
        {
            if (pauseMenu != null)
            {
                pauseMenu.ShowPauseMenu();
            }
        }
        else
        {
            if (mainMenu != null)
            {
                mainMenu.ShowMainMenu();
            }
        }

    }
    /*
    private void StartRebinding()
    {
        if(Application.platform != RuntimePlatform.WindowsPlayer &&
           Application.platform != RuntimePlatform.WindowsEditor &&
           Application.platform != RuntimePlatform.LinuxPlayer &&
           Application.platform != RuntimePlatform.OSXPlayer &&
           Application.platform != RuntimePlatform.OSXEditor)
        {
            return;
        }


        var action = moveActionReference.action;
        if (action == null) return;

        action.Disable();

        rebindLabel.text = "Press a key";


        rebindingOperation = action.PerformInteractiveRebinding()
            .WithControlsExcluding("Move")
            .OnMatchWaitForAnother(0.1f).
            OnCancel(operaton => 
            {
                Debug.LogWarning("Fail");
                operaton.Dispose();
                action.Enable();
                UpdateRebindLabel();
            }).
            OnComplete(operation =>
            {
                Debug.Log("Sukcess");
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
        if (action == null) return;

        string pcBinding = "";

        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (!action.bindings[i].isPartOfComposite && action.bindings[i].path.Contains("Keyboard"))
            {
                pcBinding = action.GetBindingDisplayString(i, InputBinding.DisplayStringOptions.DontIncludeInteractions);
                break;
            }
        }
        // rebindLabel.text = action.GetBindingDisplayString();
        rebindLabel.text = pcBinding;
    }

    private void ResetSelectedRebinds()
    {
        var action = moveActionReference.action;
        if (action == null) return;

        int bindingIndex = action.GetBindingIndexForControl(action.controls[0]);
        if(bindingIndex >= 0)
        {
            action.ApplyBindingOverride(bindingIndex, action.bindings[bindingIndex].path);
        }

        PlayerPrefs.SetString("rebind_" + action.id, action.SaveBindingOverridesAsJson());
        PlayerPrefs.Save();

        UpdateRebindLabel();
    }
     private void AllResetRebinds()
    {
        var action = moveActionReference.action;
        if(action == null) return;

        action.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey("rebind_" + action.id);
        PlayerPrefs.Save();
        //LoadRebinds();
        UpdateRebindLabel();
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
    */

}