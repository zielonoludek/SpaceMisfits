using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SectorEventSO", menuName = "Events/New Event")]
public class SectorEventSO : EventSO
{
    [Header("Effects & Crewmates")]
    public Effect eventEffect;
    public CrewmateData crewmateToRecruit;

    [Header("Choice-Based event settings")]
    public bool hasChoices;

    [Range(2, 4)] public int numberOfChoices = 2;
    
    [System.Serializable]
    public class Choice
    {
        public string choiceDescription;
        public Effect choiceEffect;
        public CrewmateData crewmate;
    }
    
    public List<Choice> choices = new List<Choice>() { new Choice(), new Choice() };

    // public string GetChoice(int index) => (hasChoices && index < choices.Count) ? choices[index].choiceDescription : null;
}
