using UnityEngine;

[CreateAssetMenu(fileName = "SectorEventSO", menuName = "Events/New Event")]
public class SectorEventSO : ScriptableObject
{
    public enum EventType { Story, Battle, Treasure, Spaceport}

    public EventType eventType;
    public string eventTitle;
    [TextArea(3, 5)] public string eventDescription;
}
