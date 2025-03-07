using UnityEngine;
using UnityEngine.UI;

public class RemoveShipHealthButton : MonoBehaviour
{
    public Button shipHealthButton;

    private void Start()
    {
        shipHealthButton.onClick.AddListener(RemoveShipHealth);
    }

    private void RemoveShipHealth()
    {
        GameManager.Instance.ResourceManager.ShipHealth -= 3000;
    }
}
