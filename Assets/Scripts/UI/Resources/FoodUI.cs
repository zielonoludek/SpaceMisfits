using UnityEngine;

public class FoodUI : MonoBehaviour
{
    [SerializeField] private CircleStatusIndicator statusIndicator;

    private void Start()
    {
        GameManager.Instance.ResourceManager.OnFoodChanged += UpdateCrewHungerUI;
        UpdateCrewHungerUI(GameManager.Instance.ResourceManager.Food);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null && GameManager.Instance.ResourceManager != null)
        {
            GameManager.Instance.ResourceManager.OnFoodChanged -= UpdateCrewHungerUI;
        }
    }

    private void UpdateCrewHungerUI(int amount)
    {
        statusIndicator.UpdateFillAmount(amount);
    }
}
