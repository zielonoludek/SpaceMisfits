using UnityEngine;
using TMPro;

public class ResourcesUI : MonoBehaviour
{
    public TextMeshProUGUI bootyText;
    public TextMeshProUGUI notorietyText;
    public TextMeshProUGUI shipHealthText;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI CrewMoodText;

    private void Start()
    {
        GameManager.Instance.ResourceManager.OnBootyChanged += UpdateBootyUI;
        GameManager.Instance.ResourceManager.OnNotorietyChanged += UpdateNotorietyUI;
        GameManager.Instance.ResourceManager.OnShipHealthChanged += UpdateShipHealthUI;
        GameManager.Instance.ResourceManager.OnFoodChanged += UpdateFoodUI;
        GameManager.Instance.ResourceManager.OnCrewMoodChanged += UpdateCrewMoodUI;
        UpdateBootyUI(GameManager.Instance.ResourceManager.Booty);
        UpdateNotorietyUI(GameManager.Instance.ResourceManager.Notoriety);
        UpdateShipHealthUI(GameManager.Instance.ResourceManager.ShipHealth);
        UpdateFoodUI(GameManager.Instance.ResourceManager.Food);
        UpdateCrewMoodUI(GameManager.Instance.ResourceManager.CrewMood);
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

    private void UpdateCrewMoodUI(int amount)
    {
        CrewMoodText.text = "Crew Mood: " + amount;
    }
}