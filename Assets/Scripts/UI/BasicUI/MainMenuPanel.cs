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
    }

    private void Setup()
    {
        gameObject.SetActive(true);

        startBtn.onClick.RemoveAllListeners();
        settingsBtn.onClick.RemoveAllListeners();
        quitBtn.onClick.RemoveAllListeners();
        continueBtn.onClick.RemoveAllListeners();

        startBtn.onClick.AddListener(() => SceneManager.LoadScene("MainGameScene"));
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