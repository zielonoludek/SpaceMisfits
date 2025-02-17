using UnityEngine;

public class UIOptions : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject pauseMenu;
    public GameObject OptionCanvas;

    public bool openPauseGame = false;
    public void OpenFromMainMenu()
    {
        OptionCanvas.SetActive(true);
        mainMenu.SetActive(false);
        openPauseGame = false;

    }

    public void OpenFromPauseMenu()
    {
        OptionCanvas.SetActive(true);
        mainMenu.SetActive(false);
        openPauseGame = true;

    }

    public void CloseOptions()
    {
        OptionCanvas.SetActive(false);

        if (openPauseGame)
        {
            pauseMenu.SetActive(true);
        }
        else
        {
            mainMenu.SetActive(true);
        }
    }
}
