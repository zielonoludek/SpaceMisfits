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
        if (pauseMenu == null) Debug.LogError("pauseMenu nie ma ");
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
    }

    private void OnDisable()
    {
        gameObject.SetActive(false);

        if (cancelAction != null)
        {
            cancelAction.Disable();
        }
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
        if(PlayerPrefs.HasKey("GameVolume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("GameVolume");
            audioMixer.SetFloat("Volume", Mathf.Log10(savedVolume) * 20);
            volumeSlider.value = savedVolume;
        }
    }
}
