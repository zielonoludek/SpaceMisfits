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

    private VisualElement optionsMenu;

    private Button backButton;
    private bool openedFromPause = false;

    private InputAction cancelAction;

    private AudioMixer audioMixer;
    private Slider volumeSlider;



    // [SerializeField] private GameObject OptionsGameObject;

    private void Awake()
    {

        //GameObject.FindFirstObjectByType<UI_Options>();
        //GameObject.Find("UI_Options")?.SetActive(true);
        // OptionsGameObject = GetComponentInChildren<UIDocument>(true)?.gameObject;
        mainMenu = FindFirstObjectByType<MainMenuEvents>(FindObjectsInactive.Include);
        pauseMenu = FindFirstObjectByType<PauseManager>(FindObjectsInactive.Include);
    }

    private void OnEnable()
    {

        var root = Ui_Options.rootVisualElement;

        mainMenu = GameObject.FindFirstObjectByType<MainMenuEvents>();
        pauseMenu = GameObject.FindFirstObjectByType<PauseManager>();


        optionsMenu = root.Q<VisualElement>("optionsMenu");

        //if (mainMenu == null) Debug.LogError("MainMenuu nie ma ");
        //if (pauseMenu == null) Debug.LogError("pauseMenu nie ma ");
        //if (optionsMenu == null) Debug.LogError("optionsMenu nie ma ");


        optionsMenu.style.display = DisplayStyle.None;


        backButton = root.Q<Button>("BackButton");
        backButton.clicked += CloseOptions;

        var uiActionMap = inputActions.FindActionMap("UI");
        cancelAction = uiActionMap.FindAction("Cancel");
        cancelAction.performed += ctx => CloseOptions();
        cancelAction.Enable();

        // VOLUME SLIDER AUDIO 
        if (volumeSlider != null)
        {
            volumeSlider = root.Q<Slider>("VolumeSlider");
            volumeSlider.RegisterValueChangedCallback(evt => SetVolume(evt.newValue));
            LoadVolume();
        }

        var resetButton = root.Q<Button>("ResetButton");
        resetButton.clicked += ResetSelectedRebinds;

        var allresetButton = root.Q<Button>("AllResetButton");
        allresetButton.clicked += AllResetRebinds;

        rebindButton = root.Q<Button> ("RebindButton");
        rebindLabel = root.Q<Label>("RebindLabel");
        rebindButton.clicked += StartRebinding;
        LoadRebinds();
    }

    private void OnDisable()
    {
        gameObject.SetActive(false);

        if (cancelAction != null)
        {
            cancelAction.Disable();
        }

        rebindButton.clicked -= StartRebinding;
        rebindingOperation?.Dispose();
    }

    public void OpenFromMainMenu()
    {
        //OptionsGameObject.SetActive(true);
        optionsMenu.style.display = DisplayStyle.Flex;
        openedFromPause = false;
    }

    public void OpenFromPauseMenu()
    {
        // OptionsGameObject.SetActive(true);
        optionsMenu.style.display = DisplayStyle.Flex;
        openedFromPause = true;
    }

    private void CloseOptions()
    {
        optionsMenu.style.display = DisplayStyle.None;

        //OptionsGameObject.SetActive(false);

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

    private void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("GameVolume", volume);
    }

    private void LoadVolume()
    {
        if (PlayerPrefs.HasKey("GameVolume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("GameVolume");
            audioMixer.SetFloat("Volume", Mathf.Log10(savedVolume) * 20);
            volumeSlider.value = savedVolume;
        }
    }

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
    
}