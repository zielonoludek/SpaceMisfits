using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class EventSO : ScriptableObject
{
    [Header("Event Persistence")] 
    [Tooltip("If true, the event remains after leaving the sector")]
    public bool isPersistent;
    
    [Header("Event properties")]
    public string eventTitle;
    public EventType eventType;
    [TextArea(3, 7)][SerializeField] public string eventDescription;
}
