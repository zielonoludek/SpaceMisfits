using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject pauseMenu;
    public GameObject OptionCanvas;

    public bool openPauseGame = false;
    
    public AudioMixer audioMixer;
    public Dropdown resolutiondropdown;

    public Resolution[] resolutions;
    
 

    public void Start()
    {
        resolutions = Screen.resolutions;
        resolutiondropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutiondropdown.AddOptions(options);
        resolutiondropdown.value = currentResolutionIndex;
        resolutiondropdown.RefreshShownValue();

    }
    
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
    
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool isfullScreen)
    {
        Screen.fullScreen = isfullScreen;
    }
}
