using UnityEngine;
using TMPro;

public class ResourcesUI : MonoBehaviour
{
    public TextMeshProUGUI bootyText;
    public TextMeshProUGUI notorietyText;
    public TextMeshProUGUI shipHealthText;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI crewMoraleText;

    private void Start()
    {
        ResourceManager.Instance.OnBootyChanged += UpdateBootyUI;
        ResourceManager.Instance.OnNotorietyChanged += UpdateNotorietyUI;
        ResourceManager.Instance.OnShipHealthChanged += UpdateShipHealthUI;
        ResourceManager.Instance.OnFoodChanged += UpdateFoodUI;
        ResourceManager.Instance.OnCrewMoraleChanged += UpdateCrewMoraleUI;
        UpdateBootyUI(ResourceManager.Instance.Booty);
        UpdateNotorietyUI(ResourceManager.Instance.Notoriety);
        UpdateShipHealthUI(ResourceManager.Instance.ShipHealth);
        UpdateFoodUI(ResourceManager.Instance.Food);
        UpdateCrewMoraleUI(ResourceManager.Instance.CrewMorale);
    }

    private void OnDestroy()
    {
        ResourceManager.Instance.OnBootyChanged -= UpdateBootyUI;
        ResourceManager.Instance.OnNotorietyChanged -= UpdateNotorietyUI;
        ResourceManager.Instance.OnShipHealthChanged -= UpdateShipHealthUI;
        ResourceManager.Instance.OnFoodChanged -= UpdateFoodUI;
        ResourceManager.Instance.OnCrewMoraleChanged -= UpdateCrewMoraleUI;
    }

    private void UpdateBootyUI(int amount)
    {
        bootyText.text = "Booty: " + amount;
    }

    private void UpdateNotorietyUI(int amount)
    {
        notorietyText.text = "Notoriety: " + amount;
    }

    private void UpdateShipHealthUI(int amount)
    {
        shipHealthText.text = "Ship Health: " + amount;
    }

    private void UpdateFoodUI(int amount)
    {
        foodText.text = "Food: " + amount;
    }

    private void UpdateCrewMoraleUI(int amount)
    {
        crewMoraleText.text = "Crew Morale: " + amount;
    }
}