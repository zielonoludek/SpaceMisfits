using UnityEngine;
using UnityEngine.UI;

public class AddShipHealthButton : MonoBehaviour
{
    public Button shipHealthButton;

    private void Start()
    {
        shipHealthButton.onClick.AddListener(AddShipHealth);
    }

    private void AddShipHealth()
    {
        GameManager.Instance.ResourceManager.ShipHealth += 3000;
    }
}
