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
        GameManager.Instance.ResourceManager.OnSightChanged += UpdateSightUI;

        increaseSightButton.onClick.AddListener(IncreaseSight);
        decreaseSightButton.onClick.AddListener(DecreaseSight);

        UpdateSightUI(GameManager.Instance.ResourceManager.GetCurrentSight());

        GameManager.Instance.ResourceManager.OnSpeedChanged += UpdateSpeedUI;

        increaseSpeedButton.onClick.AddListener(IncreaseSpeed);
        decreaseSpeedButton.onClick.AddListener(DecreaseSpeed);

        UpdateSpeedUI(GameManager.Instance.ResourceManager.GetCurrentSpeed());
    }

    private void IncreaseSight()
    {
        GameManager.Instance.ResourceManager.IncreaseSight();
    }

    private void DecreaseSight()
    {
        GameManager.Instance.ResourceManager.DecreaseSight();
    }

    private void IncreaseSpeed()
    {
        GameManager.Instance.ResourceManager.IncreaseSpeed();
    }

    private void DecreaseSpeed()
    {
        GameManager.Instance.ResourceManager.DecreaseSpeed();
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
        GameManager.Instance.ResourceManager.OnSightChanged -= UpdateSightUI;
        GameManager.Instance.ResourceManager.OnSpeedChanged -= UpdateSpeedUI;
    }
}
