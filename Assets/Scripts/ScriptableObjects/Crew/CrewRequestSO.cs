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
    [SerializeField]  public Vector3 TimeLimitDayHoursMinutes;
    public float StartTime;
    public float ExpirationTime;

    [Space]
    public RequestOriginType Requirement;
    [SerializeReference] public IFulfillmentCondition FulfillmentCondition;
    [SerializeField, HideInInspector] public EventSO EventToExecute;

    [Space]
    [SerializedDictionary("Type", "Amount")] public SerializedDictionary<EffectType, int> Rewards = new SerializedDictionary<EffectType, int>();
    [SerializedDictionary("Type", "Amount")] public SerializedDictionary<EffectType, int> Penalties = new SerializedDictionary<EffectType, int>();

    private void OnValidate()
    {
        if (Requirement == RequestOriginType.Event && !(FulfillmentCondition is BoolFulfillmentCondition))
        {
            FulfillmentCondition = new BoolFulfillmentCondition();
            EventToExecute ??= null;
        }
        else if (Requirement != RequestOriginType.Event && !(FulfillmentCondition is IntFulfillmentCondition))
        {
            FulfillmentCondition = new IntFulfillmentCondition();
            EventToExecute = null;
        }
    }
    public float TimeLimitInSeconds()
    {
        return GameManager.Instance.TimeManager.ConvertTimeVec3ToSeconds(TimeLimitDayHoursMinutes);
    }
    public bool CanFulfillRequest()
    {
        if (Requirement == RequestOriginType.Event)
        {
            return FulfillmentCondition is BoolFulfillmentCondition boolCondition && boolCondition.EventDone;
        }

        if (TryGetEffectTypeFromRequestOrigin(Requirement, out EffectType effectType))
        {
            int currentValue = GameManager.Instance.ResourceManager.GetResourceValue(effectType);
            return FulfillmentCondition is IntFulfillmentCondition intCondition && currentValue >= intCondition.RequiredValue;
        }

        return false;
    }
    private bool TryGetEffectTypeFromRequestOrigin(RequestOriginType requestType, out EffectType effectType)
    {
        Dictionary<RequestOriginType, EffectType> mapping = new Dictionary<RequestOriginType, EffectType>
        {
            { RequestOriginType.Booty, EffectType.Booty },
            { RequestOriginType.Notoriety, EffectType.Notoriety },
            { RequestOriginType.Health, EffectType.Health },
            { RequestOriginType.Sight, EffectType.Sight },
            { RequestOriginType.Speed, EffectType.Speed },
            { RequestOriginType.Food, EffectType.Food },
            { RequestOriginType.CrewMood, EffectType.CrewMood }
        };

        return mapping.TryGetValue(requestType, out effectType);
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