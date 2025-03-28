using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SectorEventSO", menuName = "Events/New Event")]
public class SectorEventSO : EventSO
{
    [Header("Effects & Crewmates")]
    public CrewmateData crewmateToRecruit;
    [Range(0, 10)] public int numberOfEventEffects = 0;
    public List<Effect> eventEffects = new List<Effect>() { null };

    [Header("Choice-Based event settings")]
    public bool hasChoices;
    [Range(1, 4)] public int numberOfChoices = 1;
    
    [System.Serializable]
    public class EventProbability
    {
        public EventSO eventOption;
        [Range(0, 100)] public float probability;
    }
    
    [System.Serializable]
    public class Choice
    {
        public string choiceDescription;
        public CrewmateData crewmate;
        [Range(0, 10)] public int numberOfChoiceEffects = 0;
        public List<Effect> choiceEffects = new List<Effect>() { null };
        public List<EventProbability> possibleEvents = new List<EventProbability>();
        
        public EventSO GetRandomEvent()
        {
            float totalWeight = 0f;
            foreach (var ev in possibleEvents)
                totalWeight += ev.probability;

            if (totalWeight <= 0) return null;

            float randomPoint = UnityEngine.Random.value * totalWeight;
            float cumulative = 0f;

            foreach (var ev in possibleEvents)
            {
                cumulative += ev.probability;
                if (randomPoint <= cumulative)
                    return ev.eventOption;
            }

            return null;
        }
    }
    
    public List<Choice> choices = new List<Choice>() { new Choice(), new Choice() };

    // public string GetChoice(int index) => (hasChoices && index < choices.Count) ? choices[index].choiceDescription : null;
}
