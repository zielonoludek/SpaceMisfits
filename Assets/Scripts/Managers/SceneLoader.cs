using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public event System.Action NewSceneLoaded;

    private void Start()
    {
        ChangeSceneEnum(SceneManager.GetActiveScene().buildIndex);
    }
    public void LoadNewScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
        SceneManager.sceneLoaded += OnSceneLoaded;
        ChangeSceneEnum(sceneIndex);
    }
    private void ChangeSceneEnum(int sceneIndex)
    {
        if (sceneIndex == 1) GameManager.Instance.GameScene = GameScene.Ship;
        if (sceneIndex == 2) GameManager.Instance.GameScene = GameScene.Map;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; 
        NewSceneLoaded?.Invoke();
    }
}
