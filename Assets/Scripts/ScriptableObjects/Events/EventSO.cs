using UnityEngine;

public abstract class EventSO : ScriptableObject
{
    [Header("Event properties")]
    public string eventTitle;
    public EventType eventType;
    [TextArea(3, 7)][SerializeField] public string eventDescription;
}
