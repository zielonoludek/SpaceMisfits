using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using static UnityEngine.Timeline.DirectorControlPlayable;

public class MainMenuEvents : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] private UIDocument document;
    private UI_Options ui_Options;

    private InputAction clickAction;
    private AudioSource audioSource;

    private VisualElement mainMenu;
    private VisualElement optionsMenu;


    private List<Button> menuButtons = new List<Button>();

    private void Awake()
    {
        GameObject.Find("MainMenuEvents")?.SetActive(true);
        ui_Options = FindFirstObjectByType<UI_Options>(FindObjectsInactive.Include);
    }

    private void OnEnable()
    {
        var root = document.rootVisualElement;

        mainMenu = root.Q<VisualElement>("MainMenu");

        var UiActionMap = inputActions.FindActionMap("UI");
        clickAction = UiActionMap.FindAction("Click");
        clickAction.performed += ctx => OnClickUI();

        // START GAME 
        var startbutton = root.Q<Button>("Start");
        startbutton.clicked += () => SceneManager.LoadScene("Kama"); // or SceneManager.LoadSceneAsync(0);  // There will be movement Scene no.0 (always return to Main Menu)

        // SETTINGS 
        var settingsbutton = root.Q<Button>("Settings");
        settingsbutton.clicked += OpenOptions;

        //QUIT GAME
        var Quitbutton = root.Q<Button>("Quit");
        Quitbutton.clicked += () => Application.Quit();


        // ALL BUTTON AUDIO SOURCE 
        menuButtons = root.Query<Button>().ToList();
        foreach (var button in menuButtons)
        {
            button.clicked += () => PlayClickSound();
        }

        clickAction.Enable();
    }

    private void OnDisable()
    {
        clickAction.Disable();
    }


    // For ALL BUTTON 
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
        mainMenu.style.display = DisplayStyle.Flex;
    }

    public void OpenOptions()
    {
        ui_Options = GameObject.FindFirstObjectByType<UI_Options>();
        ui_Options.OpenFromMainMenu();
        mainMenu.style.display = DisplayStyle.None;
    }
}