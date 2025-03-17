using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SettingsControls : MonoBehaviour
{
    [SerializeField] private InputActionReference moveActionReference;
    [SerializeField] private TMP_Text rebindLabel;
    [SerializeField] private TMP_Text rebindText;
    [SerializeField] private Button rebindButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button allResetButton;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private void Awake()
    {
        rebindText.text = moveActionReference.action.name;
        UpdateRebindLabel();
        LoadRebinds();

        rebindButton.onClick.AddListener(StartRebinding);
        resetButton.onClick.AddListener(ResetSelectedRebind);

        if (allResetButton != null)
        {
            allResetButton.onClick.AddListener(ResetAllBindings);
        }
    }

    private void StartRebinding()
    {
        var action = moveActionReference.action;
        if (action == null) return;

        action.Disable();
        rebindLabel.text = "Press a key...";

        rebindingOperation = action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnCancel(operation =>
            {
                operation.Dispose();
                action.Enable();
                UpdateRebindLabel();
            })
            .OnComplete(operation =>
            {
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
        PlayerPrefs.SetString($"rebind_{action.id}", rebindData);
        PlayerPrefs.Save();
    }

    private void LoadRebinds()
    {
        var action = moveActionReference.action;
        string rebindData = PlayerPrefs.GetString($"rebind_{action.id}", "");
        if (!string.IsNullOrEmpty(rebindData))
        {
            action.LoadBindingOverridesFromJson(rebindData);
        }
        UpdateRebindLabel();
    }

    private void UpdateRebindLabel()
    {
        var action = moveActionReference.action;
        string displayString = "";

        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (action.bindings[i].isPartOfComposite) continue;
            if (action.bindings[i].path.Contains("Keyboard"))
            {
                displayString = action.GetBindingDisplayString(i, InputBinding.DisplayStringOptions.DontIncludeInteractions);
                break;
            }
        }

        if (rebindLabel != null)
            rebindLabel.text = displayString;
    }

    private void ResetSelectedRebind()
    {
        var action = moveActionReference.action;
        action.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey($"rebind_{action.id}");
        PlayerPrefs.Save();
        UpdateRebindLabel();
    }

    private void ResetAllBindings()
    {
        var inputActions = moveActionReference.action.actionMap.asset;
        foreach (var actionMap in inputActions.actionMaps)
        {
            foreach (var action in actionMap.actions)
            {
                action.RemoveAllBindingOverrides();
                PlayerPrefs.DeleteKey($"rebind_{action.id}");
            }
        }

        PlayerPrefs.Save();
        UpdateRebindLabel();
    }
}
