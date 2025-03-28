using UnityEngine;
using System.Collections;

public class PlayerDebug : MonoBehaviour
{
    public static PlayerDebug Instance { get; private set; }

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
            if (GameManager.Instance.GameScene == GameScene.Map)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        };
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
