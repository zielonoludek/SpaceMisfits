using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void LoadNewScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
