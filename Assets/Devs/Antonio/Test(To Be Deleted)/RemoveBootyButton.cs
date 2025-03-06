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
        ResourceManager.Instance.Booty -= 100;
    }
}
