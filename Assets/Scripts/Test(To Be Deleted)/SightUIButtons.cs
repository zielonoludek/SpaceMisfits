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
        GameManager.Instance.ResourceManager.OnSightChanged += UpdateUI;

        increaseButton.onClick.AddListener(IncreaseSight);
        decreaseButton.onClick.AddListener(DecreaseSight);

        UpdateUI(GameManager.Instance.ResourceManager.GetCurrentSight());
    }

    private void IncreaseSight()
    {
        GameManager.Instance.ResourceManager    .IncreaseSight();
    }

    private void DecreaseSight()
    {
        GameManager.Instance.ResourceManager.DecreaseSight();
    }

    private void UpdateUI(int newSight)
    {
        sightText.text = $"Sight Level: {newSight}";
    }

    private void OnDestroy()
    {
        GameManager.Instance.ResourceManager.OnSightChanged -= UpdateUI;
    }
}
