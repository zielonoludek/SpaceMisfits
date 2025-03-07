using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SubsceneToggle : MonoBehaviour
{
    [SerializeField] private string mapScene = "MapScene";
    [SerializeField] private List<GameObject> itemsToHide;

    private bool isSubsceneLoaded = false;

    public void ToggleSubscene()
    {
        if (isSubsceneLoaded)
        {
            SceneManager.UnloadSceneAsync(mapScene);
            foreach (GameObject item in itemsToHide)
            {
                if (item != null)
                {
                    item.SetActive(true);
                }
            }
            Debug.Log("Subscene Unloaded: " + mapScene);
            isSubsceneLoaded = false;
        }
        else
        {
            SceneManager.LoadScene(mapScene, LoadSceneMode.Additive);
            foreach (GameObject item in itemsToHide)
            {
                if (item != null)
                {
                    item.SetActive(false);
                }
            }
            Debug.Log("Subscene Loaded: " + mapScene);
            isSubsceneLoaded = true;
        }
    }
}
