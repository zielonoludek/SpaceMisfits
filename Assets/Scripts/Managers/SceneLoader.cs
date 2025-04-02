using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public event System.Action NewSceneLoaded;

    private void Start()
    {
        ChangeSceneEnum(SceneManager.GetActiveScene().buildIndex);
    }
    public void LoadNewScene(int sceneIndex, bool forceZoomed = false)
    {
        GameManager.Instance.CameraManager.SaveZoomState();
        SceneManager.LoadSceneAsync(sceneIndex);
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            GameManager.Instance.CameraManager.RestoreZoomState(forceZoomed);
            NewSceneLoaded?.Invoke();
        };
        ChangeSceneEnum(sceneIndex);
    }
    private void ChangeSceneEnum(int sceneIndex)
    {
        if (sceneIndex == 0) GameManager.Instance.GameScene = GameScene.MainMenu;
        else if(sceneIndex == 1) GameManager.Instance.GameScene = GameScene.Ship;
        else if(sceneIndex == 2) GameManager.Instance.GameScene = GameScene.Map;
        else GameManager.Instance.GameScene = GameScene.None;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameManager.Instance.CameraManager.RestoreZoomState();
        NewSceneLoaded?.Invoke();
    }
}
