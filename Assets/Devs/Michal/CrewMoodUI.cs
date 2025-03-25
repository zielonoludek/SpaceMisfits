using System;
using UnityEngine;

public class CrewMoodUI : MonoBehaviour
{
    [SerializeField] private CircleStatusIndicator statusIndicator;

    private void Start()
    {
        GameManager.Instance.ResourceManager.OnCrewMoodChanged += UpdateCrewMoodUI;
        UpdateCrewMoodUI(GameManager.Instance.ResourceManager.CrewMood);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null && GameManager.Instance.ResourceManager != null)
        {
            GameManager.Instance.ResourceManager.OnCrewMoodChanged -= UpdateCrewMoodUI;
        }
    }

    private void UpdateCrewMoodUI(int amount)
    {
        statusIndicator.UpdateFillAmount(amount);
    }
}
