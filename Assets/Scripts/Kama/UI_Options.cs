using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UI_Options : MonoBehaviour
{
    [SerializeField] private UIDocument Ui_Options;
    [SerializeField] private InputActionAsset inputActions;

    private VisualElement mainMenu;
    private VisualElement pauseMenu;
    private VisualElement optionsMenu;

    private Button backButton;
    private bool openedFromPause = false;

    private InputAction cancelAction;


    private void OnEnable()
    {
        var root = Ui_Options.rootVisualElement;

        mainMenu = root.Q<VisualElement>("MainMenu");
        pauseMenu = root.Q<VisualElement>("pause");
        optionsMenu = root.Q<VisualElement>("optionsMenu");

        backButton = root.Q<Button>("BackButton");
        backButton.clicked += CloseOptions;

        var uiActionMap = inputActions.FindActionMap("UI");
        cancelAction = uiActionMap.FindAction("Cancel");
        cancelAction.performed += ctx => CloseOptions();
        cancelAction.Enable();

        optionsMenu.style.display = DisplayStyle.None;
    }

    private void OnDisable()
    {
        cancelAction.Disable();
    }

    public void OpenFromMainMenu()
    {
        optionsMenu.style.display = DisplayStyle.Flex;
        mainMenu.style.display = DisplayStyle.None;
        openedFromPause = false;
    }

    private void OpenFromPauseMenu()
    {
        optionsMenu.style.display = DisplayStyle.Flex;
        pauseMenu.style.display = DisplayStyle.None;
        openedFromPause = true;
    }

    private void CloseOptions()
    {
        optionsMenu.style.display = DisplayStyle.None;

        if(openedFromPause)
        {
            pauseMenu.style.display = DisplayStyle.Flex;
        }
        else
        {
            mainMenu.style.display = DisplayStyle.Flex;
        }
    }
}
