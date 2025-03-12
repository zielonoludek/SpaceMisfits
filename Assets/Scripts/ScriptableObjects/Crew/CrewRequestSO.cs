using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName = "NewCrewRequest", menuName = "Crew/CrewRequest")]
public class CrewRequestSO : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Artwork;

    public RequestType Type;
    public CrewMemberType specialMember;

    public bool IsFromCrewMember;
    public string CrewMemberName;

    [Space]
    [SerializeField]  private Vector3 TimeLimitDayHoursMinutes = Vector3.zero;
    [HideInInspector] public float TimeLimit;
    [HideInInspector] public float StartTime;
    [HideInInspector] public float ExpirationTime;

    [Space]
    public RequestOriginType Requrement;
    [SerializeReference] public IFulfillmentCondition FulfillmentCondition;
    [SerializeField, HideInInspector] public EventSO EventToExecute;

    [Space]
    [SerializedDictionary("Type", "Amount")] public SerializedDictionary<EffectType, int> Rewards = new SerializedDictionary<EffectType, int>();
    [SerializedDictionary("Type", "Amount")] public SerializedDictionary<EffectType, int> Penalties = new SerializedDictionary<EffectType, int>();

    private void OnValidate()
    {
        TimeLimit = (TimeLimitDayHoursMinutes.x * 86400) + (TimeLimitDayHoursMinutes.y * 3600) + (TimeLimitDayHoursMinutes.z * 60);

        if (Requrement == RequestOriginType.Event && !(FulfillmentCondition is BoolFulfillmentCondition))
        {
            FulfillmentCondition = new BoolFulfillmentCondition();
            EventToExecute ??= null;
        }
        else if (Requrement != RequestOriginType.Event && !(FulfillmentCondition is IntFulfillmentCondition))
        {
            FulfillmentCondition = new IntFulfillmentCondition();
            EventToExecute = null;
        }
    }
}

public interface IFulfillmentCondition { }

[System.Serializable]
public class BoolFulfillmentCondition : IFulfillmentCondition
{
    public bool EventDone;
}

[System.Serializable]
public class IntFulfillmentCondition : IFulfillmentCondition
{
    public int RequiredValue;
}
