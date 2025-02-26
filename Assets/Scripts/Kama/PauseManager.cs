using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private UIDocument PauseDocument;
    [SerializeField] private InputActionAsset inputActions;

    private InputAction clickAction;
    private InputAction pauseAction;

    private VisualElement pauseEl;


    private Button backButton;
    private Button settingsButton;
    private Button quitButton;


    public static bool isPaused = false;


    private void OnEnable()
    {
        var root = PauseDocument.rootVisualElement;
         pauseEl = root.Q<VisualElement>("pause");

        backButton = root.Q<Button>("BackToMain");
        backButton.clicked += () => SceneManager.LoadScene("MainMenu"); // or SceneManager.LoadSceneAsync(0);  // There will be movement Scene no.0 (always return to Main Menu)

        settingsButton = root.Q<Button>("Settings");
        settingsButton.clicked += () => Debug.Log("Settings");

        quitButton = root.Q<Button>("Quit");
        quitButton.clicked += () => Application.Quit();


        // Downloading actions with the input system
        var UiActionMap = inputActions.FindActionMap("UI");
        clickAction = UiActionMap.FindAction("Click");
        pauseAction = UiActionMap.FindAction("Cancel");

        clickAction.performed += ctx => OnClick();
        pauseAction.performed += ctx => Pause();

        clickAction.Enable();
        pauseAction.Enable();
    }

    private void Pause()
    {
        isPaused = !isPaused;
        pauseEl.style.display = isPaused ? DisplayStyle.Flex : DisplayStyle.None;
        Time.timeScale = isPaused ? 0 : 1;
    }


    private void OnDisable()
    {
        clickAction.Disable();
        pauseAction.Disable();

    }

    private void OnClick()
    {
        Debug.Log("Click");
    }


}
