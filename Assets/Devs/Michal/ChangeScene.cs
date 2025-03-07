using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void LoadNewScene()
    {
        SceneManager.LoadScene(1);
    }
}
