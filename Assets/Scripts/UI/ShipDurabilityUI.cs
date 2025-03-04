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
        ShipDurabilityManager.Instance.OnShipPartDamaged += UpdateUI;
        ShipDurabilityManager.Instance.OnShipPartRepaired += UpdateUI;
        RefreshUI();
    }

    private void OnDestroy()
    {
        ShipDurabilityManager.Instance.OnShipPartDamaged -= UpdateUI;
        ShipDurabilityManager.Instance.OnShipPartRepaired -= UpdateUI;
    }

    private void UpdateUI(string partName, int durability)
    {
        if (partName == "Hull") hullText.text = "Hull: " + durability;
        if (partName == "Sails") sailsText.text = "Sails: " + durability;
        if (partName == "Cannon") cannonText.text = "Cannon: " + durability;
    }

    private void RefreshUI()
    {
        UpdateUI("Hull", ShipDurabilityManager.Instance.GetDurability("Hull"));
        UpdateUI("Sails", ShipDurabilityManager.Instance.GetDurability("Sails"));
        UpdateUI("Cannon", ShipDurabilityManager.Instance.GetDurability("Cannon"));
    }
}
