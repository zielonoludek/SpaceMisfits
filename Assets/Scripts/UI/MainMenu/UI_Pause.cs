using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Pause : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private InputActionAsset inputActions;

    private UI_CanvasOptions ui_Settings;
    private InputAction clickAction;
    private InputAction pauseAction;

    private Button backButton;
    private Button settingsButton;
    private Button quitButton;

    public static bool isPaused = false;

    private void Awake()
    {

        ui_Settings = FindFirstObjectByType<UI_CanvasOptions>(FindObjectsInactive.Include);

        pauseMenu = GameObject.Find("UI_Pause");
        backButton = GameObject.Find("BackToMain").GetComponent<Button>();
        settingsButton = GameObject.Find("Settings").GetComponent<Button>();
        quitButton = GameObject.Find("Quit").GetComponent<Button>();

        backButton.onClick.AddListener(ReturnToMainMenu);
        settingsButton.onClick.AddListener(OpenOptions);
        quitButton.onClick.AddListener(() => Application.Quit());

        var uiActionMap = inputActions.FindActionMap("UI");
        clickAction = uiActionMap.FindAction("Click");
        pauseAction = uiActionMap.FindAction("Cancel");

        pauseAction.performed += ctx => Cancel();

        clickAction.Enable();
        pauseAction.Enable();

        pauseMenu?.SetActive(false);
        isPaused = false;
    }

    private void ReturnToMainMenu()
    {
        clickAction.Disable();
        pauseAction.Disable();
        SceneManager.LoadScene("MainMenu");
    }

    private void Cancel()
    {
        if (ui_Settings != null && ui_Settings.IsOpen())
        {
            ui_Settings.CloseOptions();
            ShowPauseMenu();
        }
        else if (isPaused)
        {
            Pause();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        if (ui_Settings != null && ui_Settings.IsOpen()) return;
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void ShowPauseMenu()
    {
        pauseMenu.SetActive(true);
    }

    public void OpenOptions()
    {
        ui_Settings = FindFirstObjectByType<UI_CanvasOptions>(FindObjectsInactive.Include);
        ui_Settings.OpenFromPauseMenu();
        pauseMenu.SetActive(false);
    }
    private void OnDisable()
    {
        clickAction.Disable();
    }
}
