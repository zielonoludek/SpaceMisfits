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
        ResourceManager.Instance.ShipHealth += 3000;
    }
}
