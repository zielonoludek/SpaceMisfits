using UnityEngine;

[CreateAssetMenu(fileName = "SectorEventSO", menuName = "Events/New Event")]
public class SectorEventSO : ScriptableObject
{
    [Header("Event properties")]
    public string eventTitle;
    
    public enum EventType { FaintSignal, Waypoint, DevilsMaw, SharpenThoseDirks}
    public EventType eventType;
    
    [TextArea(3, 7)][SerializeField] public string eventDescription;

    public Effect eventEffect;

    [Header("Choice-Based event settings")]
    public bool hasChoices;

    public string choice1Description;
    public Effect choice1Effect;
    
    public string choice2Description;
    public Effect choice2Effect;
    
    public string GetChoice1() => hasChoices ? choice1Description : null;
    public string GetChoice2() => hasChoices ? choice2Description : null;
}
