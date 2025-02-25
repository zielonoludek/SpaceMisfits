using UnityEngine;

[CreateAssetMenu(fileName = "SectorEventSO", menuName = "Events/New Event")]
public class SectorEventSO : ScriptableObject
{
    [Header("Event properties")]
    [SerializeField] private string eventTitle;
    
    public enum EventType { FaintSignal, Waypoint, DevilsMaw, SharpenThoseDirks}
    public EventType eventType;
    
    [TextArea(3, 7)][SerializeField] private string eventDescription;

    [SerializeField] private Effect eventEffect;

    [Header("Choice-Based event settings")]
    [SerializeField] private bool hasChoices;

    [SerializeField] private string choice1Description;
    [SerializeField] private Effect choice1Effect;
    
    [SerializeField] private string choice2Description;
    [SerializeField] private Effect choice2Effect;

    public string GetEventTitle() => eventTitle;
    public string GetEventDescription() => eventDescription;
    public Effect GetEventEffect() => eventEffect;
    public string GetChoice1() => hasChoices ? choice1Description : null;
    public string GetChoice2() => hasChoices ? choice2Description : null;
}
