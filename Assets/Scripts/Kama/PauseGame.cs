using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    public static bool isPause = false;
    public GameObject PauseUI;

    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            Pause();        
        }

    }

    public void Pause()
    {
        isPause = !isPause;
        PauseUI.SetActive(isPause);
        Time.timeScale = isPause ? 0 : 1;
    }
    public void MainMenu()
    {
        //SceneManager.LoadSceneAsync(0);
        SceneManager.LoadScene("MainMenu");
    }    

    public void QuitGame()
    {
        Application.Quit();
    }
}
