using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EventPopupUI : MonoBehaviour
{
    [Header("UI References")] 
    [SerializeField] private GameObject eventPanel;
    [SerializeField] private TextMeshProUGUI eventTitleText;
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
        
        AddHoverEffectToButtonText(closeButton);
    }
    
    public void ShowEvent(SectorEventSO sectorEvent)
    {
        currentEvent = sectorEvent;
        
        // Pause time manager
        GameManager.Instance.TimeManager.PauseTime(true);
        
        eventPanel.SetActive(true);
        eventTitleText.text = sectorEvent.eventTitle;
        eventDescriptionText.text = sectorEvent.eventDescription;

        // Display multiple event effects
        if (sectorEvent.eventEffects != null && sectorEvent.eventEffects.Count > 0)
        {
            StringBuilder effectsText = new StringBuilder();
            foreach (Effect effect in sectorEvent.eventEffects)
            {
                if (effect != null)
                {
                    if (effectsText.Length > 0)
                    {
                        effectsText.AppendLine();
                    }
                    effectsText.Append($"{effect.description}! {effect.effectType} {effect.amount}");
                }
            }
            eventEffectText.text = effectsText.ToString();
        }
        else
        {
            eventEffectText.text = "";
        }

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
                Transform horizontalBox = newButton.transform.Find("HorizontalBox");
                TextMeshProUGUI choiceText = horizontalBox.Find("ChoiceText").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI choiceEffectText = horizontalBox.Find("ChoiceEffectText").GetComponent<TextMeshProUGUI>();
                
                choiceText.text = choice.choiceDescription;

                // Display multiple choice effects
                if (choice.choiceEffects != null && choice.choiceEffects.Count > 0)
                {
                    StringBuilder effectsText = new StringBuilder();
                    foreach (Effect effect in choice.choiceEffects)
                    {
                        if (effect != null)
                        {
                            if (effectsText.Length > 0)
                                effectsText.Append(", ");
                            effectsText.Append($"{effect.effectType} {effect.amount}");
                        }
                    }
                    choiceEffectText.text = effectsText.ToString();
                }
                else
                {
                    choiceEffectText.text = "";
                }

                int capturedIndex = i;
                newButton.onClick.AddListener(() => SelectChoice(capturedIndex));

                activeChoiceButtons.Add(newButton);

                // Add hover effect to choice button text
                AddHoverEffectToButtonText(newButton);
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
        
        // Apply all event effects
        if (currentEvent.eventEffects != null)
        {
            foreach (Effect effect in currentEvent.eventEffects)
            {
                if (effect != null)
                {
                    effect.ApplyEffect();
                }
            }
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

        // Apply all choice effects
        if (choice.choiceEffects != null)
        {
            foreach (Effect effect in choice.choiceEffects)
            {
                if (effect != null)
                {
                    effect.ApplyEffect();
                }
            }
        }

        if (choice.crewmate != null)
        {
            GameManager.Instance.CrewManager.RecruitCrewmate(choice.crewmate);
        }

        EventSO nextEvent = choice.GetRandomEvent();
        if (nextEvent != null)
        {
            if (nextEvent is SectorEventSO sectorEvent)
            {
                ShowEvent(sectorEvent);
            }
            else if (nextEvent is FightEventSO fightEvent)
            {
                GameManager.Instance.FightManager.StartFight(fightEvent);
                CloseEvent();
            }
        }
        else
        {
            CloseEvent();
        }
    }

    private void AddHoverEffectToButtonText(Button button)
    {
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText != null)
        {
            EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            pointerEnterEntry.callback.AddListener((data) => { buttonText.color = Color.red; });
            trigger.triggers.Add(pointerEnterEntry);

            EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            pointerExitEntry.callback.AddListener((data) => { buttonText.color = Color.black; });
            trigger.triggers.Add(pointerExitEntry);
        }
    }
}