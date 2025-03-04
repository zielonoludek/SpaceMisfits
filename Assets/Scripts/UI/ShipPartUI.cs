using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShipPartUI : MonoBehaviour
{
    public List<ShipPart> linkedParts;
    public Button damageButton;
    public Button repairButton;
    public Button upgradeButton;
    public Button applyEffectButton;
    public TextMeshProUGUI partDetailsText;

    private int selectedPartIndex = 0;

    private void Start()
    {
        ResetAllParts();
        damageButton.onClick.AddListener(() => ModifyDurability(-10));
        repairButton.onClick.AddListener(() => ModifyDurability(10));
        upgradeButton.onClick.AddListener(UpgradePart);
        applyEffectButton.onClick.AddListener(ApplyPartEffect);
        RefreshUI();
    }

    private void ResetAllParts()
    {
        foreach (var part in linkedParts)
        {
            part.ResetValues();
        }
    }

    private void ModifyDurability(int amount)
    {
        if (linkedParts.Count > 0)
        {
            ShipPart part = linkedParts[selectedPartIndex];
            if (amount < 0) part.TakeDamage(-amount);
            else part.Repair(amount);
            RefreshUI();
        }
    }

    private void UpgradePart()
    {
        if (linkedParts.Count > 0)
        {
            ShipPart part = linkedParts[selectedPartIndex];
            if (part.level < 3)
            {
                part.level++;
                Debug.Log($"{part.partName} upgraded to level {part.level}!");
                RefreshUI();
            }
            else
            {
                Debug.Log($"{part.partName} is already at max level!");
            }
        }
    }

    private void ApplyPartEffect()
    {
        if (linkedParts.Count > 0)
        {
            linkedParts[selectedPartIndex].ApplyEffect();
            RefreshUI();
        }
    }

    private void RefreshUI()
    {
        if (linkedParts.Count > 0)
        {
            ShipPart part = linkedParts[selectedPartIndex];
            partDetailsText.text = $"{part.partName}\nDurability: {part.durability}\nLevel: {part.level}";
        }
    }
}