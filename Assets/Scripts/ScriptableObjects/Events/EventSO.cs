using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class EventSO : ScriptableObject
{
    [Header("Linked Events")] [Tooltip("Other events that share progress with this event.")]
    public List<EventSO> tiedEvents = new List<EventSO>();
    private HashSet<EventSO> completedEvents = new HashSet<EventSO>();
    
    [Header("Event Persistence")] 
    [Tooltip("If true, the event remains after leaving the sector")]
    public bool isPersistent;
    
    [Header("Event properties")]
    public string eventTitle;
    public EventType eventType;
    [TextArea(3, 7)][SerializeField] public string eventDescription;

    public void MarkEventAsCompleted()
    {
        if (!completedEvents.Contains(this))
        {
            completedEvents.Add(this);
        }

        foreach (var linkedEvent in tiedEvents)
        {
            linkedEvent.SyncCompletionState(this);
        }
    }

    public bool IsAnyLinkedEventCompleted()
    {
        foreach (var linkedEvent in tiedEvents)
        {
            if (completedEvents.Contains(linkedEvent))
            {
                return true;
            }
        }
        return false;
    }

    private void SyncCompletionState(EventSO completedEvent)
    {
        if (!completedEvents.Contains(completedEvent))
        {
            completedEvents.Add(completedEvent);
        }
    }
    
    private void OnValidate()
    {
        // Ensure bidirectional linking when adding
        foreach (var linkedEvent in tiedEvents)
        {
            if (linkedEvent != null && !linkedEvent.tiedEvents.Contains(this))
            {
                linkedEvent.tiedEvents.Add(this);
                UnityEditor.EditorUtility.SetDirty(linkedEvent);
            }
        }

        // Ensure bidirectional removal when an event is removed
        foreach (var linkedEvent in new List<EventSO>(completedEvents)) // Create a copy to avoid modifying while iterating
        {
            if (linkedEvent != null && !tiedEvents.Contains(linkedEvent))
            {
                linkedEvent.tiedEvents.Remove(this);
                UnityEditor.EditorUtility.SetDirty(linkedEvent);
            }
        }

        // Checks if any event still has a reference to this and removes it
        foreach (var eventSO in UnityEditor.AssetDatabase.FindAssets("t:EventSO")
                     .Select(guid => UnityEditor.AssetDatabase.LoadAssetAtPath<EventSO>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid))))
        {
            if (eventSO != this && eventSO.tiedEvents.Contains(this) && !tiedEvents.Contains(eventSO))
            {
                eventSO.tiedEvents.Remove(this);
                UnityEditor.EditorUtility.SetDirty(eventSO);
            }
        }

        // Ensure persistence for Waypoint and Spaceport events
        if (eventType == EventType.Waypoint || eventType == EventType.Spaceport)
        {
            isPersistent = true;
        }
    }
}
