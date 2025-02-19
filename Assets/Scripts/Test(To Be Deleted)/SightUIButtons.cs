using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SightUIButtons : MonoBehaviour
{
    public TextMeshProUGUI sightText;
    public Button increaseButton;
    public Button decreaseButton;

    private void Start()
    {
        ResourceManager.Instance.OnSightChanged += UpdateUI;

        increaseButton.onClick.AddListener(IncreaseSight);
        decreaseButton.onClick.AddListener(DecreaseSight);

        UpdateUI(ResourceManager.Instance.GetCurrentSight());
    }

    private void IncreaseSight()
    {
        ResourceManager.Instance.IncreaseSight();
    }

    private void DecreaseSight()
    {
        ResourceManager.Instance.DecreaseSight();
    }

    private void UpdateUI(int newSight)
    {
        sightText.text = $"Sight Level: {newSight}";
    }

    private void OnDestroy()
    {
        ResourceManager.Instance.OnSightChanged -= UpdateUI;
    }
}
