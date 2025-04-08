using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TiedEventSequenceSO", menuName = "Events/New Tied Event Sequence")]
public class TiedEventSequenceSO : ScriptableObject
{
    public string sequenceName;
    [Header("Ordered list of events")] 
    [Tooltip("Events should be visited in order from top to bottom")]
    public List<EventSO> orderedEvents = new List<EventSO>();

    [Tooltip("If enabled, game will show pulsing effect on the next sector")]
    public bool enableVisualization;
    
    private int currentIndex = 0;

    public EventSO GetCurrentEvent()
    {
        if (currentIndex < orderedEvents.Count)
        {
            return orderedEvents[currentIndex];
        }

        return null;
    }

    public EventSO GetNextEvent()
    {
        if (currentIndex + 1 < orderedEvents.Count)
        {
            return orderedEvents[currentIndex + 1];
        }

        return null;
    }

    public void MarkCurrentEventAsCompleted()
    {
        if (currentIndex < orderedEvents.Count)
        {
            Debug.Log($"Event {orderedEvents[currentIndex].eventTitle} in sequence {sequenceName} completed!");
            currentIndex++;

            if (currentIndex < orderedEvents.Count)
            {
                Debug.Log($"Next event unlocked: {orderedEvents[currentIndex].eventTitle}");
            }
            else
            {
                Debug.Log($"Event {orderedEvents[currentIndex - 1].eventTitle} was the last event in this sequence!");
            }
        }
    }

    public bool IsSequenceCompleted()
    {
        return currentIndex >= orderedEvents.Count;
    }
    
    public void ResetSequence()
    {
        Debug.Log($"Resetting sequence {sequenceName}");
        currentIndex = 0;
    }
}
