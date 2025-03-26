using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using static UnityEngine.Timeline.DirectorControlPlayable;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private InputActionAsset inputActions;

    private UI_MainMenu ui_mainMenu;
    private UI_CanvasOptions ui_Settings;

    private InputAction clickAction;
    private InputAction cancelAction;
    private AudioSource audioSource;

    private Button startButton;
    private Button settingsButton;
    private Button quitButton;
    private List<Button> menuButtons = new List<Button>();

    private void Awake()
    {
        ui_Settings = FindFirstObjectByType<UI_CanvasOptions>(FindObjectsInactive.Include);
        ui_mainMenu = FindFirstObjectByType<UI_MainMenu>(FindObjectsInactive.Include);

        mainMenu = GameObject.Find("UI_Main");

        startButton = GameObject.Find("NewGame").GetComponent<Button>();
        settingsButton = GameObject.Find("Settings").GetComponent<Button>();
        quitButton = GameObject.Find("Quit").GetComponent<Button>();


        audioSource = GetComponent<AudioSource>();

        startButton.onClick.AddListener(StartGame);
        settingsButton.onClick.AddListener(OpenOptions);
        quitButton.onClick.AddListener(() => Application.Quit());

        menuButtons.Add(startButton);
        menuButtons.Add(settingsButton);
        menuButtons.Add(quitButton);

        foreach (var button in menuButtons)
        {
            button.onClick.AddListener(PlayClickSound);
        }

        var uiActionMap = inputActions.FindActionMap("UI");
        clickAction = uiActionMap.FindAction("Click");
        cancelAction = uiActionMap.FindAction("Cancel");

        cancelAction.performed += ctx => Cancel();

        clickAction.Enable();
        cancelAction.Enable();

        mainMenu.SetActive(true);
    }

    private void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    private void Cancel()
    {
        if (ui_Settings != null && ui_Settings.IsOpen())
        {
            ui_Settings.CloseOptions();
            ShowMainMenu();
        }

    }

    private void OnDisable()
    {
        clickAction.Disable();
    }

    private void PlayClickSound()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
    }

    public void OpenOptions()
    {
        ui_Settings = FindFirstObjectByType<UI_CanvasOptions>(FindObjectsInactive.Include);
        ui_Settings.OpenFromMainMenu();
        mainMenu.SetActive(false);
    }
}
