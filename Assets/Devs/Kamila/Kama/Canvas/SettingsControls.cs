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
        if (inputActions == null)
        {
            Debug.LogError("❌ Brak przypisanego InputActionAsset!");
            return;
        }

        // Pobranie całego obiektu SettingsControls
        settingsControls = GameObject.Find("SettingsControls");

        if (settingsControls == null)
        {
            Debug.LogError("❌ Nie znaleziono GameObject 'SettingsControls' w hierarchii!");
            return;
        }

        // Znajdujemy elementy UI wewnątrz obiektu SettingsControls
        resetButton = settingsControls.transform.Find("ResetButton")?.GetComponent<Button>();
        allResetButton = settingsControls.transform.Find("AllResetButton")?.GetComponent<Button>();
        rebindButton = settingsControls.transform.Find("RebindButton")?.GetComponent<Button>();
        rebindLabel = settingsControls.transform.Find("RebindLabel")?.GetComponent<Text>();

        // Sprawdzamy, czy wszystkie elementy istnieją
        if (resetButton == null) Debug.LogError("❌ Nie znaleziono ResetButton w SettingsControls.");
        if (allResetButton == null) Debug.LogError("❌ Nie znaleziono AllResetButton w SettingsControls.");
        if (rebindButton == null) Debug.LogError("❌ Nie znaleziono RebindButton w SettingsControls.");
        if (rebindLabel == null) Debug.LogError("❌ Nie znaleziono RebindLabel w SettingsControls.");

        if (resetButton != null) resetButton.onClick.AddListener(ResetSelectedRebinds);
        if (allResetButton != null) allResetButton.onClick.AddListener(AllResetRebinds);
        if (rebindButton != null) rebindButton.onClick.AddListener(StartRebinding);
    }
    private void StartRebinding()
    {
        if (moveActionReference == null || moveActionReference.action == null)
        {
            Debug.LogError("❌ moveActionReference lub jego akcja jest NULL! Upewnij się, że został przypisany w Inspektorze.");
            return;
        }

        var action = moveActionReference.action;
        action.Disable();
        rebindLabel.text = "Press a key...";

        rebindingOperation = action.PerformInteractiveRebinding()
            .WithControlsExcluding("Move")
            .OnMatchWaitForAnother(0.1f)
            .OnCancel(operation =>
            {
                Debug.LogWarning("⚠️ Rebinding anulowany.");
                operation.Dispose();
                action.Enable();
                UpdateRebindLabel();
            })
            .OnComplete(operation =>
            {
                Debug.Log("✅ Sukces! Klawisz przypisany.");
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
