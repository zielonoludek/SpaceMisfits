using UnityEngine;
using UnityEngine.UI;

public class AddBootyButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(AddBooty);
    }

    private void AddBooty()
    {
        GameManager.Instance.ResourceManager.Booty += 100;
    }
}
