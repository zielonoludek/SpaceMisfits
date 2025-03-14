using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class UI_CanvasOptions : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private InputActionAsset inputActions;

    private UI_Settings ui_Settings;
    private UI_MainMenu ui_mainMenu;
    private UI_Pause ui_Pause;
    private SettingsAudio settingsaudio;
    private SettingsControls settingscontrols;
    private SettingsGraphic settingsGraphic;

    private Button backButton;
    private Button audioButton;
    private Button graphicButton;
    private Button controlsButton;

    private InputAction clickAction;
    private InputAction backAction;

    private bool openedFromPause = false;

    private void Awake()
    {
        ui_mainMenu = FindFirstObjectByType<UI_MainMenu>(FindObjectsInactive.Include);
        ui_Pause = FindFirstObjectByType<UI_Pause>(FindObjectsInactive.Include);
        settingsaudio = FindFirstObjectByType<SettingsAudio>(FindObjectsInactive.Include);
        settingscontrols = FindFirstObjectByType<SettingsControls>(FindObjectsInactive.Include);
        settingsGraphic = FindFirstObjectByType<SettingsGraphic>(FindObjectsInactive.Include);

        backButton = GameObject.Find("Back").GetComponent<Button>();
        audioButton = GameObject.Find("Audio").GetComponent<Button>();
        graphicButton = GameObject.Find("Graphic").GetComponent<Button>();
        controlsButton = GameObject.Find("Controls").GetComponent<Button>();

        backButton.onClick.AddListener(CloseOptions);
        audioButton.onClick.AddListener(OpenAudio);
        graphicButton.onClick.AddListener(OpenGraphic);
        controlsButton.onClick.AddListener(OpenControls);

        var uiActionMap = inputActions.FindActionMap("UI");
        clickAction = uiActionMap.FindAction("Click");
        backAction = uiActionMap.FindAction("Cancel");

        backAction.performed += ctx => CloseOptions();
        backAction.performed += ctx => OpenAudio();
        backAction.performed += ctx => OpenGraphic();
        backAction.performed += ctx => OpenControls();

        clickAction.Enable();
        backAction.Enable();

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
        if (openedFromPause)
        {
            ui_Pause.ShowPauseMenu();
            settingsMenu.SetActive(false);
        }
        else
        {
            ui_mainMenu.ShowMainMenu();
            settingsMenu.SetActive(false);
        }
    }

    private void OpenAudio()
    {
        settingsaudio.OpenAudio();
        settingsGraphic.CloseGraphic();
        settingscontrols.CloseControls();
    }

    private void OpenGraphic()
    {
        settingsaudio.CloseAudio();
        settingsGraphic.OpenGraphic();
        settingscontrols.CloseControls();
    }

    private void OpenControls()
    {
        settingsaudio.CloseAudio();
        settingsGraphic.CloseGraphic();
        settingscontrols.OpenControls();
    }
}
