using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private InputActionAsset inputActions;

    private UI_MainMenu ui_mainMenu;
    private UI_Settings ui_Settings;

    private InputAction clickAction;
    private AudioSource audioSource;

    private Button startButton;
    private Button settingsButton;
    private Button quitButton;
    private List<Button> menuButtons = new List<Button>();

    private void Awake()
    {
        ui_Settings = FindFirstObjectByType<UI_Settings>(FindObjectsInactive.Include);
        ui_mainMenu = FindFirstObjectByType<UI_MainMenu>(FindObjectsInactive.Include);

        mainMenu = GameObject.Find("UI_Main");

        startButton = GameObject.Find("NewGame").GetComponent<Button>();
        settingsButton = GameObject.Find("Settings").GetComponent<Button>();
        quitButton = GameObject.Find("Quit").GetComponent<Button>();


        audioSource = GetComponent<AudioSource>();

        startButton.onClick.AddListener(() => SceneManager.LoadScene("SampleScene"));
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
        clickAction.performed += ctx => OnClickUI();
        clickAction.Enable();

        mainMenu.SetActive(true);
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

    private void OnClickUI()
    {
        Debug.Log("Click");
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
    }

    public void OpenOptions()
    {
        ui_Settings = FindFirstObjectByType<UI_Settings>(FindObjectsInactive.Include);
        ui_Settings.OpenFromMainMenu();
        mainMenu.SetActive(false);
    }
}
