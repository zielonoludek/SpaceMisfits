using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UI_CanvasOptions : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private InputActionAsset inputActions;

    private UI_MainMenu ui_mainMenu;
    private UI_Pause ui_Pause;
    private SettingsAudio settingsaudio;
    private UI_Controls settingscontrols;
    private SettingsGraphic settingsGraphic;

    private Button backButton;
    private Button audioButton;
    private Button graphicButton;
    private Button controlsButton;

    private bool openedFromPause = false;

    private void Awake()
    {
        ui_mainMenu = FindFirstObjectByType<UI_MainMenu>(FindObjectsInactive.Include);
        ui_Pause = FindFirstObjectByType<UI_Pause>(FindObjectsInactive.Include);
        settingsaudio = FindFirstObjectByType<SettingsAudio>(FindObjectsInactive.Include);
        settingscontrols = FindFirstObjectByType<UI_Controls>(FindObjectsInactive.Include);
        settingsGraphic = FindFirstObjectByType<SettingsGraphic>(FindObjectsInactive.Include);

        backButton = GameObject.Find("Back").GetComponent<Button>();
        audioButton = GameObject.Find("Audio").GetComponent<Button>();
        graphicButton = GameObject.Find("Graphic").GetComponent<Button>();
        controlsButton = GameObject.Find("Controls").GetComponent<Button>();

        backButton.onClick.AddListener(CloseOptions);
        audioButton.onClick.AddListener(OpenAudio);
        graphicButton.onClick.AddListener(OpenGraphic);
        controlsButton.onClick.AddListener(OpenControls);
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

    public void CloseOptions()
    {
        settingsMenu.SetActive(false);
        if (openedFromPause && ui_Pause)
        {
            ui_Pause.ShowPauseMenu();
        }
        else if(ui_mainMenu)
        {
            ui_mainMenu.ShowMainMenu();
        }
    }

    public bool IsOpen()
    {
        return settingsMenu.activeSelf;
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
