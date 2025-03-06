using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SightUIButtons : MonoBehaviour
{
    public TextMeshProUGUI sightText;
    public Button increaseSightButton;
    public Button decreaseSightButton;

    public TextMeshProUGUI speedText;
    public Button increaseSpeedButton;
    public Button decreaseSpeedButton;

    private void Start()
    {
        ResourceManager.Instance.OnSightChanged += UpdateSightUI;

        increaseSightButton.onClick.AddListener(IncreaseSight);
        decreaseSightButton.onClick.AddListener(DecreaseSight);

        UpdateSightUI(ResourceManager.Instance.GetCurrentSight());

        ResourceManager.Instance.OnSpeedChanged += UpdateSpeedUI;

        increaseSpeedButton.onClick.AddListener(IncreaseSpeed);
        decreaseSpeedButton.onClick.AddListener(DecreaseSpeed);

        UpdateSpeedUI(ResourceManager.Instance.GetCurrentSpeed());
    }

    private void IncreaseSight()
    {
        ResourceManager.Instance.IncreaseSight();
    }

    private void DecreaseSight()
    {
        ResourceManager.Instance.DecreaseSight();
    }

    private void IncreaseSpeed()
    {
        ResourceManager.Instance.IncreaseSpeed();
    }

    private void DecreaseSpeed()
    {
        ResourceManager.Instance.DecreaseSpeed();
    }

    private void UpdateSightUI(int newSight)
    {
        sightText.text = $"Sight Level: {newSight}";
    }

    private void UpdateSpeedUI(int newSpeed)
    {
        speedText.text = $"Speed Level: {newSpeed}";
    }

    private void OnDestroy()
    {
        ResourceManager.Instance.OnSightChanged -= UpdateSightUI;
        ResourceManager.Instance.OnSpeedChanged -= UpdateSpeedUI;
    }
}
