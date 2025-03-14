using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SettingsControls : MonoBehaviour
{
    [SerializeField] GameObject settingsControls;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private InputActionReference moveActionReference;

    private Button rebindButton;
    private Text rebindLabel;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private Button resetButton;
    private Button allResetButton;

    private InputAction clickAction;

    private void Awake()
    {
        var uiActionMap = inputActions.FindActionMap("UI");
        clickAction = uiActionMap.FindAction("Click");
        clickAction.Enable();

        resetButton = GameObject.Find("ResetButton").GetComponent<Button>();
        allResetButton = GameObject.Find("AllResetButton").GetComponent<Button>();

        rebindButton = GameObject.Find("RebindButton").GetComponent<Button>();
        rebindLabel = GameObject.Find("RebindLabel").GetComponent<Text>();

        resetButton.onClick.AddListener(ResetSelectedRebinds);
        allResetButton.onClick.AddListener(AllResetRebinds);
        rebindButton.onClick.AddListener(StartRebinding);
    }

    private void StartRebinding()
    {
        var action = moveActionReference.action;
        if (action == null) return;

        action.Disable();
        rebindLabel.text = "Press a key...";

        rebindingOperation = action.PerformInteractiveRebinding()
            .WithControlsExcluding("Move")
            .OnMatchWaitForAnother(0.1f)
            .OnCancel(operation =>
            {
                Debug.LogWarning("Fail");
                operation.Dispose();
                action.Enable();
                UpdateRebindLabel();
            })
            .OnComplete(operation =>
            {
                Debug.Log("Success");
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
        rebindLabel.text = action.GetBindingDisplayString();
    }

    private void ResetSelectedRebinds()
    {
        var action = moveActionReference.action;
        action.RemoveAllBindingOverrides();
        SaveRebinds();
        UpdateRebindLabel();
    }

    private void AllResetRebinds()
    {
        var action = moveActionReference.action;
        action.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey("rebind_" + action.id);
        PlayerPrefs.Save();
        UpdateRebindLabel();
    }

    public void OpenControls()
    {
        settingsControls.SetActive(true);
    }

    public void CloseControls()
    {
        settingsControls.SetActive(false);
    }

}
