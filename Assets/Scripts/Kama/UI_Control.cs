using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UI_Control : MonoBehaviour
{
    [SerializeField] private UIDocument Ui_Options;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private InputActionReference moveActionReference;

    private UI_Options ui_Options;

    private Button rebindButton;
    private Label rebindLabel;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private VisualElement optionsMenu;
    private VisualElement ControlMenu;

    private Button backButton;
    private Button ButtonControl;

    private InputAction cancelAction;

    private void OnEnable()
    {

        var root = Ui_Options.rootVisualElement;

        optionsMenu = root.Q<VisualElement>("optionsMenu");
        optionsMenu.style.display = DisplayStyle.None;

        ControlMenu = root.Q<VisualElement>("ControlMenu");
        ControlMenu.style.display = DisplayStyle.None;

        ButtonControl = root.Q<Button>("Rebind");
        ButtonControl.clicked += OpenControl;

        backButton = root.Q<Button>("BackButton");
        backButton.clicked += CloseControl;

        var uiActionMap = inputActions.FindActionMap("UI");
        cancelAction = uiActionMap.FindAction("Cancel");
        cancelAction.performed += ctx => CloseControl();
        cancelAction.Enable();

        var resetButton = root.Q<Button>("ResetButton");
        resetButton.clicked += ResetSelectedRebinds;

        var allresetButton = root.Q<Button>("AllResetButton");
        allresetButton.clicked += AllResetRebinds;

        rebindButton = root.Q<Button>("RebindButton");
        rebindLabel = root.Q<Label>("RebindLabel");
        rebindButton.clicked += StartRebinding;
    }

    private void OnDisable()
    {
        gameObject.SetActive(false);

        if (cancelAction != null)
        {
            cancelAction.Disable();
        }

        rebindButton.clicked -= StartRebinding;
        rebindingOperation?.Dispose();
    }

    private void CloseControl()
    {
        ui_Options = GameObject.FindFirstObjectByType<UI_Options>();
        ui_Options.CloseControl();

    }

    private void OpenControl()
    {
        ControlMenu.style.display = DisplayStyle.Flex;
        ui_Options.OpenControl();

    }
    private void StartRebinding()
    {
        if (Application.platform != RuntimePlatform.WindowsPlayer &&
           Application.platform != RuntimePlatform.WindowsEditor &&
           Application.platform != RuntimePlatform.LinuxPlayer &&
           Application.platform != RuntimePlatform.OSXPlayer &&
           Application.platform != RuntimePlatform.OSXEditor)
        {
            return;
        }


        var action = moveActionReference.action;
        if (action == null) return;

        action.Disable();

        rebindLabel.text = "Press a key";


        rebindingOperation = action.PerformInteractiveRebinding()
            .WithControlsExcluding("Move")
            .OnMatchWaitForAnother(0.1f).
            OnCancel(operaton =>
            {
                Debug.LogWarning("Fail");
                operaton.Dispose();
                action.Enable();
                UpdateRebindLabel();
            }).
            OnComplete(operation =>
            {
                Debug.Log("Sukcess");
                operation.Dispose();
                SaveRebinds();
                UpdateRebindLabel();
                action.Enable();
            })
            .Start();
    }

    private void SaveRebinds()
    {
        var action = moveActionReference.action;
        string rebindData = action.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebind_" + action.id, rebindData);
        PlayerPrefs.Save();
    }

    private void LoadRebinds()
    {
        var action = moveActionReference.action;
        string rebindData = PlayerPrefs.GetString("rebind_" + action.id);
        action.LoadBindingOverridesFromJson(rebindData);
        UpdateRebindLabel();
    }

    private void UpdateRebindLabel()
    {
        var action = moveActionReference.action;
        if (action == null) return;

        string pcBinding = "";

        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (!action.bindings[i].isPartOfComposite && action.bindings[i].path.Contains("Keyboard"))
            {
                pcBinding = action.GetBindingDisplayString(i, InputBinding.DisplayStringOptions.DontIncludeInteractions);
                break;
            }
        }
        rebindLabel.text = pcBinding;
    }

    private void ResetSelectedRebinds()
    {
        var action = moveActionReference.action;
        if (action == null) return;

        int bindingIndex = action.GetBindingIndexForControl(action.controls[0]);
        if (bindingIndex >= 0)
        {
            action.ApplyBindingOverride(bindingIndex, action.bindings[bindingIndex].path);
        }

        PlayerPrefs.SetString("rebind_" + action.id, action.SaveBindingOverridesAsJson());
        PlayerPrefs.Save();

        UpdateRebindLabel();
    }
    private void AllResetRebinds()
    {
        var action = moveActionReference.action;
        if (action == null) return;

        action.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey("rebind_" + action.id);
        PlayerPrefs.Save();
        UpdateRebindLabel();
    }
}
