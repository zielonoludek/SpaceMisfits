using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class PausePanelUI : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    [SerializeField] private Button continueBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button quitBtn;
    [SerializeField] private Button backToMenuBtn;

    public void Setup()
    {
        pausePanel.SetActive(true);
        GameManager.Instance.TimeManager.PauseTime(true);

        continueBtn.onClick.RemoveAllListeners();
        settingsBtn.onClick.RemoveAllListeners();
        quitBtn.onClick.RemoveAllListeners();
        backToMenuBtn.onClick.RemoveAllListeners();

        settingsBtn.onClick.AddListener(OpenOptions);
        backToMenuBtn.onClick.AddListener(OpenMainMenu);
        continueBtn.onClick.AddListener(Close);
        quitBtn.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }

    private void OpenOptions()
    {
        Close();
        GameManager.Instance.UIManager.OptionsPanelUI.Setup();
    }
    private void OpenMainMenu()
    {
        GameManager.Instance.SceneLoader.LoadNewScene(0); 
        GameManager.Instance.TimeManager.PauseTime(true);

    }
    public void Close()
    {
        GameManager.Instance.TimeManager.PauseTime(false);
        pausePanel.SetActive(false);
    }
}
