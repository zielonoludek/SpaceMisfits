using UnityEngine;
using UnityEngine.UI;

public class OptionsPanel : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject graphicsPanel;
    [SerializeField] private GameObject controlsPanel;

    [SerializeField] private Button quitBtn;
    [SerializeField] private Button audioBtn;
    [SerializeField] private Button graphicsBtn;
    [SerializeField] private Button controlsBtn;

    public void Setup()
    {
        optionsPanel.SetActive(true);   

        quitBtn.onClick.RemoveAllListeners();
        audioBtn.onClick.RemoveAllListeners();
        graphicsBtn.onClick.RemoveAllListeners();
        controlsBtn.onClick.RemoveAllListeners();

        quitBtn.onClick.AddListener(() => optionsPanel.SetActive(false));
        audioBtn.onClick.AddListener(() => OpenPanel(SettingsPanel.Audio));
        graphicsBtn.onClick.AddListener(() => OpenPanel(SettingsPanel.Graphics));
        controlsBtn.onClick.AddListener(() => OpenPanel(SettingsPanel.Controls));

        OpenPanel(SettingsPanel.Audio);
    }
    void OpenPanel(SettingsPanel name)
    {
        switch (name)
        {
            case SettingsPanel.Audio:
                audioPanel.SetActive(true);
                graphicsPanel.SetActive(false);
                controlsPanel.SetActive(false);
                break;
            case SettingsPanel.Graphics:
                audioPanel.SetActive(false);
                graphicsPanel.SetActive(true);
                controlsPanel.SetActive(false);
                break;
            case SettingsPanel.Controls:
                audioPanel.SetActive(false);
                graphicsPanel.SetActive(false);
                controlsPanel.SetActive(true);
                break;
            default:
                audioPanel.SetActive(true);
                graphicsPanel.SetActive(false);
                controlsPanel.SetActive(false);
                break;
        }
    }
    public void Close()
    {
        OpenPanel(SettingsPanel.Audio);
        optionsPanel.SetActive(false);
    }
}