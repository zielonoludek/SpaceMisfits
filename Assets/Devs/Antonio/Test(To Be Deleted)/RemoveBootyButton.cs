using UnityEngine;
using UnityEngine.UI;

public class RemoveBootyButton : MonoBehaviour
{
    public Button bootyButton;

    private void Start()
    {
        bootyButton.onClick.AddListener(AddBooty);
    }

    private void AddBooty()
    {
        ResourceManager.Instance.Booty -= 100;
    }
}
