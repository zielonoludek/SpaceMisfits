using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MainMenuPanel : MonoBehaviour
{
    [SerializeField] private Button startBtn;
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button quitBtn;

    void Start()
    {
        Setup();
        GameManager.Instance.TimeManager.PauseTime(true);

    }

    private void Setup()
    {
        gameObject.SetActive(true);

        startBtn.onClick.RemoveAllListeners();
        settingsBtn.onClick.RemoveAllListeners();
        quitBtn.onClick.RemoveAllListeners();
        continueBtn.onClick.RemoveAllListeners();

        startBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.Reset();
            GameManager.Instance.SceneLoader.LoadNewScene(1);
        });
        continueBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.SceneLoader.LoadNewScene(1);
            GameManager.Instance.TimeManager.PauseTime(false);
        });
        settingsBtn.onClick.AddListener(() => GameManager.Instance.UIManager.OptionsPanelUI.Setup());
        quitBtn.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });

    }
    void OpenOptions()
    {
        GameManager.Instance.UIManager.OptionsPanelUI.Setup();
    }
}