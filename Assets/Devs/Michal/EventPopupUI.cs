using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventPopupUI : MonoBehaviour
{
    [Header("UI References")] 
    [SerializeField] private GameObject eventPanel;
    [SerializeField] private TextMeshProUGUI eventTitleText;
    [SerializeField] private TextMeshProUGUI eventTypeText;
    [SerializeField] private TextMeshProUGUI eventDescriptionText;
    [SerializeField] private TextMeshProUGUI eventEffectText;
    [SerializeField] private Button choice1Button;
    [SerializeField] private Button choice2Button;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI choice1Text;
    [SerializeField] private TextMeshProUGUI choice2Text;

    private SectorEventSO currentEvent;

    private void Start()
    {
        eventPanel.SetActive(false);
    }

    public void ShowEvent(SectorEventSO sectorEvent)
    {
        currentEvent = sectorEvent;
        
        eventPanel.SetActive(true);
        eventTitleText.text = sectorEvent.GetEventTitle();
        eventTypeText.text = $"{sectorEvent.eventType}";
        eventDescriptionText.text = sectorEvent.GetEventDescription();
        eventEffectText.text = sectorEvent.GetEventEffect() != null ? sectorEvent.GetEventEffect().effectName : "No Effect";

        if (sectorEvent.GetChoice1() != null && sectorEvent.GetChoice2() != null)
        {
            choice1Button.gameObject.SetActive(true);
            choice2Button.gameObject.SetActive(true);
            closeButton.gameObject.SetActive(false);
            choice1Text.text = sectorEvent.GetChoice1();
            choice2Text.text = sectorEvent.GetChoice2();
            
            choice1Button.onClick.AddListener(() => SelectChoice(1));
            choice2Button.onClick.AddListener(() => SelectChoice(2));
        }
        else
        {
            choice1Button.gameObject.SetActive(false);
            choice2Button.gameObject.SetActive(false);
            
            closeButton.gameObject.SetActive(true);
            closeButton.onClick.AddListener(CloseEvent);
        }
    }

    private void CloseEvent()
    {
        eventPanel.SetActive(false);
    }

    private void SelectChoice(int choice)
    {
        if (choice == 1)
        {
            Debug.Log($"Selected Choice 1: {currentEvent.GetChoice1()}");
        }
        else if (choice == 2)
        {
            Debug.Log($"Selected Choice 2: {currentEvent.GetChoice2()}");
        }

        CloseEvent();
    }
}
