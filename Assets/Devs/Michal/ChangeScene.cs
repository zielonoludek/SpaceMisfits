using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ChangeSceneUI : MonoBehaviour
{
    [SerializeField] Button changeSceneBtn;
    [SerializeField] int changeSceneNum;

    private void Start()
    {
        changeSceneBtn?.onClick.AddListener(() => GameManager.Instance.SceneLoader.LoadNewScene(changeSceneNum));
    }
}