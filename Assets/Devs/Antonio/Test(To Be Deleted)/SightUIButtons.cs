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

        increaseSightButton.onClick.AddListener(() => GameManager.Instance.ResourceManager.Sight += 1 );
        decreaseSightButton.onClick.AddListener(() => GameManager.Instance.ResourceManager.Sight -= 1);

        UpdateSightUI(GameManager.Instance.ResourceManager.GetCurrentSight());

        GameManager.Instance.ResourceManager.OnSpeedChanged += UpdateSpeedUI;

        increaseSpeedButton.onClick.AddListener(() => GameManager.Instance.ResourceManager.Speed += 1);
        decreaseSpeedButton.onClick.AddListener(() => GameManager.Instance.ResourceManager.Speed -= 1);

        UpdateSpeedUI(GameManager.Instance.ResourceManager.GetCurrentSpeed());
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
