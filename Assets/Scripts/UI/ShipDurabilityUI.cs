using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipDurabilityUI : MonoBehaviour
{
    public TextMeshProUGUI hullText;
    public TextMeshProUGUI sailsText;
    public TextMeshProUGUI cannonText;

    private void Start()
    {
        GameManager.Instance. ShipDurabilityManager.OnShipPartDamaged += UpdateUI;
        GameManager.Instance. ShipDurabilityManager.OnShipPartRepaired += UpdateUI;
        RefreshUI();
    }

    private void OnDestroy()
    {
        GameManager.Instance. ShipDurabilityManager.OnShipPartDamaged -= UpdateUI;
        GameManager.Instance. ShipDurabilityManager.OnShipPartRepaired -= UpdateUI;
    }

    private void UpdateUI(string partName, int durability)
    {
        if (partName == "Hull") hullText.text = "Hull: " + durability;
        if (partName == "Sails") sailsText.text = "Sails: " + durability;
        if (partName == "Cannon") cannonText.text = "Cannon: " + durability;
    }

    private void RefreshUI()
    {
        UpdateUI("Hull", GameManager.Instance. ShipDurabilityManager.GetDurability("Hull"));
        UpdateUI("Sails", GameManager.Instance. ShipDurabilityManager.GetDurability("Sails"));
        UpdateUI("Cannon", GameManager.Instance. ShipDurabilityManager.GetDurability("Cannon"));
    }
}
