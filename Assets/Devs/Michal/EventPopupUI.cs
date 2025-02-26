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
    [SerializeField] private TextMeshProUGUI choice1EffectText;
    [SerializeField] private TextMeshProUGUI choice2EffectText;

    private SectorEventSO currentEvent;

    private void Start()
    {
        eventPanel.SetActive(false);
    }

    public void ShowEvent(SectorEventSO sectorEvent)
    {
        currentEvent = sectorEvent;
        Time.timeScale = 0;
        
        eventPanel.SetActive(true);
        eventTitleText.text = sectorEvent.eventTitle;
        eventTypeText.text = $"{sectorEvent.eventType}";
        eventDescriptionText.text = sectorEvent.eventDescription;

        if (sectorEvent.eventEffect != null)
        {
            eventEffectText.text = $"{sectorEvent.eventEffect.description} {sectorEvent.eventEffect.amount}";
        }
        else
        {
            eventEffectText.text = "";
        }

        if (sectorEvent.GetChoice1() != null && sectorEvent.GetChoice2() != null)
        {
            choice1Button.gameObject.SetActive(true);
            choice2Button.gameObject.SetActive(true);
            closeButton.gameObject.SetActive(false);
            
            choice1Text.text = sectorEvent.GetChoice1();
            choice2Text.text = sectorEvent.GetChoice2();

            if (sectorEvent.choice1Effect != null)
            {
                choice1EffectText.text = $"{sectorEvent.choice1Effect.description} {sectorEvent.choice1Effect.amount}";    
            }
            else
            {
                choice1EffectText.text = "";
            }

            if (sectorEvent.choice2Effect != null)
            {
                choice2EffectText.text = $"{sectorEvent.choice2Effect.description} {sectorEvent.choice2Effect.amount}";   
            }
            else
            {
                choice2EffectText.text = "";
            }
            
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
        Time.timeScale = 1;
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
