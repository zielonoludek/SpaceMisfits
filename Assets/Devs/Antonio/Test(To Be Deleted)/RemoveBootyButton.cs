using UnityEngine;
using UnityEngine.UI;

public class RemoveBootyButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(AddBooty);
    }

    private void AddBooty()
    {
        GameManager.Instance.ResourceManager.Booty -= 100;
    }
}
