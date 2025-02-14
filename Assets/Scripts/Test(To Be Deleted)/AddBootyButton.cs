using UnityEngine;
using UnityEngine.UI;

public class AddBootyButton : MonoBehaviour
{
    public Button bootyButton;

    private void Start()
    {
        bootyButton.onClick.AddListener(AddBooty);
    }

    private void AddBooty()
    {
        ResourceManager.Instance.Booty += 100;
    }
}
