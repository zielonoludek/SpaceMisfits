using System;
using UnityEngine;

public class ShipHealthUI : MonoBehaviour
{
    [SerializeField] private CircleStatusIndicator statusIndicator;

    private void Start()
    {
        GameManager.Instance.ResourceManager.OnShipHealthChanged += UpdateShipHealthUI;
        UpdateShipHealthUI(GameManager.Instance.ResourceManager.ShipHealth);
    }
    
    private void OnDestroy()
    {
        if (GameManager.Instance != null && GameManager.Instance.ResourceManager != null)
        {
            GameManager.Instance.ResourceManager.OnShipHealthChanged -= UpdateShipHealthUI;
        }
    }

    private void UpdateShipHealthUI(int amount)
    {
        statusIndicator.UpdateFillAmount(amount);
    }
}
