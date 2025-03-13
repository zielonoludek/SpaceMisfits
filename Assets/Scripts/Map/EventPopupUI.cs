
using System;
using System.Collections.Generic;
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
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform choiceButtonContainer;
    [SerializeField] private Button choiceButtonPrefab;

    private SectorEventSO currentEvent;
    private List<Button> activeChoiceButtons = new List<Button>();

    private void Start()
    {
        eventPanel.SetActive(false);
    }
    
    public void ShowEvent(SectorEventSO sectorEvent)
    {
        currentEvent = sectorEvent;
        
        // Pause time manager
        GameManager.Instance.TimeManager.PauseTime(true);
        
        eventPanel.SetActive(true);
        eventTitleText.text = sectorEvent.eventTitle;
        eventTypeText.text = $"{sectorEvent.eventType}";
        eventDescriptionText.text = sectorEvent.eventDescription;

        eventEffectText.text = sectorEvent.eventEffect != null
            ? $"{sectorEvent.eventEffect.description}! {sectorEvent.eventEffect.effectType} {sectorEvent.eventEffect.amount}"
            : "";

        GenerateChoiceButtons(sectorEvent);
    }
    
    private void GenerateChoiceButtons(SectorEventSO sectorEvent)
    {
        foreach (var button in activeChoiceButtons)
        {
            Destroy(button.gameObject);
        }
        activeChoiceButtons.Clear();
        
        // Create buttons based on the number of choices
        if (sectorEvent.hasChoices && sectorEvent.choices.Count > 0)
        {
            for (int i = 0; i < sectorEvent.choices.Count; i++)
            {
                var choice = sectorEvent.choices[i];
                Button newButton = Instantiate(choiceButtonPrefab, choiceButtonContainer);
                newButton.gameObject.SetActive(true);
                
                // Find the text components inside the button prefab
                TextMeshProUGUI choiceText = newButton.transform.Find("ChoiceText").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI choiceEffectText = newButton.transform.Find("ChoiceEffectText").GetComponent<TextMeshProUGUI>();
                
                choiceText.text = choice.choiceDescription;
                choiceEffectText.text = choice.choiceEffect != null 
                    ? $"{choice.choiceEffect.effectType} {choice.choiceEffect.amount}" 
                    : "";

                int capturedIndex = i;
                newButton.onClick.AddListener(() => SelectChoice(capturedIndex));

                activeChoiceButtons.Add(newButton);
            }

            closeButton.gameObject.SetActive(false);
        }
        else
        {
            closeButton.gameObject.SetActive(true);
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseEvent);
        }
    }

    private void CloseEvent()
    {
        eventPanel.SetActive(false);
        if (currentEvent.eventEffect != null)
        {
            currentEvent.eventEffect.ApplyEffect();
        }

        if (currentEvent.crewmateToRecruit != null)
        {
            GameManager.Instance.CrewManager.RecruitCrewmate(currentEvent.crewmateToRecruit);
        }
        
        // Unpause time manager
        GameManager.Instance.TimeManager.PauseTime(false);
    }

    private void SelectChoice(int choiceIndex)
    {
        if (choiceIndex < 0 || choiceIndex >= currentEvent.choices.Count) return;

        SectorEventSO.Choice choice = currentEvent.choices[choiceIndex];

        if (choice.choiceEffect != null)
        {
            choice.choiceEffect.ApplyEffect();
        }
        
        if (choice.crewmate != null)
        {
            GameManager.Instance.CrewManager.RecruitCrewmate(choice.crewmate);
        }
        
        CloseEvent();
    }
}
