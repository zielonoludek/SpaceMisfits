using UnityEngine;
using System.Collections;

public class MapDebug : MonoBehaviour
{
    public static MapDebug Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        GameManager.Instance.SceneLoader.NewSceneLoaded += () =>
        {
            StopAllCoroutines();
            StartCoroutine(ToggleVisibility());
        };
    }

    private IEnumerator ToggleVisibility()
    {
        if (GameManager.Instance.GameScene == GameScene.Map)
        {
            yield return new WaitForSecondsRealtime(5);
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
