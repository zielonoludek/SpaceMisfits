using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Pause : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private InputActionAsset inputActions;


    private UI_Settings ui_Settings;
    private UI_Pause ui_Pause;

    private InputAction clickAction;
    private InputAction pauseAction;

    private Button backButton;
    private Button settingsButton;
    private Button quitButton;

    public static bool isPaused = false;

    private void Awake()
    {
        ui_Settings = FindFirstObjectByType<UI_Settings>(FindObjectsInactive.Include);
        ui_Pause = FindFirstObjectByType<UI_Pause>(FindObjectsInactive.Include);

        pauseMenu = GameObject.Find("UI_Pause");
        backButton = GameObject.Find("BackToMain").GetComponent<Button>();
        settingsButton = GameObject.Find("Settings").GetComponent<Button>();
        quitButton = GameObject.Find("Quit").GetComponent<Button>();

        
        backButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
        settingsButton.onClick.AddListener(OpenOptions);
        quitButton.onClick.AddListener(() => Application.Quit());

        
        var uiActionMap = inputActions.FindActionMap("UI");
        clickAction = uiActionMap.FindAction("Click");
        pauseAction = uiActionMap.FindAction("Cancel");

        pauseAction.performed += ctx => Pause();

        clickAction.Enable();

        pauseMenu.SetActive(false);
    }
    public void Pause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
        pauseAction.Enable();
    }

    private void OnDisable()
    {
        clickAction.Disable();    
    }

    public void ShowPauseMenu()
    {
        pauseMenu.SetActive(true);
    }

    public void OpenOptions()
    {
        if (ui_Settings == null)
            ui_Settings = FindFirstObjectByType<UI_Settings>(FindObjectsInactive.Include);

        ui_Settings.OpenFromPauseMenu();
        pauseMenu.SetActive(false);
        if (isPaused)
        {
            pauseAction.Disable();
        }
    }

    public void PauseAction()
    {
        pauseAction.Enable();
    }
}
