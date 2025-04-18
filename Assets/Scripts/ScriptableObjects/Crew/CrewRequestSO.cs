using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "NewCrewRequest", menuName = "Crew/CrewRequest")]
public class CrewRequestSO : ScriptableObject
{
    public string Name;
    public string Description;
    public Sprite Artwork;

    public RequestType Type;
    public RequestOriginType OriginType;
    public CrewMemberType specialMember;

    public bool IsFromCrewMember;
    public CrewMemberConsumption originCrewMemberConsumption;
    public string CrewMemberName;

    [Space]
    [SerializeField] public Vector3 TimeLimitDayHoursMinutes;
    [HideInInspector] public float StartTime;
    [HideInInspector] public float ExpirationTime;

    [Space]
    public RequestOriginType Requirement;
    public bool IsThreshold;
    [SerializeReference] public IFulfillmentCondition FulfillmentCondition;
    [SerializeField, HideInInspector] public EventSO EventToExecute;

    [Space]
    [SerializedDictionary("Type", "Amount")] public SerializedDictionary<EffectType, int> Rewards = new SerializedDictionary<EffectType, int>();
    [SerializedDictionary("Type", "Amount")] public SerializedDictionary<EffectType, int> Penalties = new SerializedDictionary<EffectType, int>();

    private void OnValidate()
    {
        if (Requirement == RequestOriginType.Event)
        {
            if (!(FulfillmentCondition is BoolFulfillmentCondition)) FulfillmentCondition = new BoolFulfillmentCondition();
            EventToExecute ??= null;
        }
        else if (IsThreshold)
        {
            if (!(FulfillmentCondition is MaintainThresholdCondition)) FulfillmentCondition = new MaintainThresholdCondition();
            EventToExecute = null;
        }
        else
        {
            if (!(FulfillmentCondition is IntFulfillmentCondition)) FulfillmentCondition = new IntFulfillmentCondition();
            EventToExecute = null;
        }
    }


    public float TimeLimitInSeconds()
    {
        return GameManager.Instance.TimeManager.ConvertTimeToFloat(TimeLimitDayHoursMinutes);
    }

    public bool CanFulfillRequest()
    {
        if (Requirement == RequestOriginType.Event) return IsEventFulfilled();

        if (GameManager.Instance.ResourceManager.TryGetResourceTypeFromRequestOrigin(Requirement, out EffectType effectType))
            return IsResourceConditionFulfilled(effectType);

        if (Requirement == RequestOriginType.Food && IsFromCrewMember) return IsHungerFulfilled();

        if (Requirement == RequestOriginType.Sleep && IsFromCrewMember) return IsSleepinessFulfilled();

        return false;
    }

    private bool IsEventFulfilled()
    {
        return FulfillmentCondition is BoolFulfillmentCondition boolCondition && boolCondition.EventDone;
    }

    private bool IsResourceConditionFulfilled(EffectType effectType)
    {
        int currentValue = GameManager.Instance.ResourceManager.GetResourceValue(effectType);

        if (FulfillmentCondition is IntFulfillmentCondition intCondition) return currentValue >= intCondition.RequiredValue;
        if (FulfillmentCondition is MaintainThresholdCondition thresholdCondition) return thresholdCondition.UpdateCondition(currentValue);

        return false;
    }

    private bool IsHungerFulfilled()
    {
        if (originCrewMemberConsumption.currentHunger < originCrewMemberConsumption.hungerThreshold) return false;

        return FulfillmentCondition is IntFulfillmentCondition intCondition &&
               originCrewMemberConsumption.currentHunger <= intCondition.RequiredValue;
    }

    private bool IsSleepinessFulfilled()
    {
        if (originCrewMemberConsumption.currentSleepiness < originCrewMemberConsumption.sleepinessThreshold) return false;

        return FulfillmentCondition is IntFulfillmentCondition intCondition &&
               originCrewMemberConsumption.currentSleepiness <= intCondition.RequiredValue;
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

[System.Serializable]
public class MaintainThresholdCondition : IFulfillmentCondition
{
    public int ThresholdValue;
    public float RequiredDuration;

    [HideInInspector] public float CurrentDuration = 0f;

    public bool UpdateCondition(int currentValue)
    {
        if (currentValue >= ThresholdValue)
        {
            CurrentDuration += Time.deltaTime;
            return CurrentDuration >= RequiredDuration;
        }
        else
        {
            CurrentDuration = 0f;
            return false;
        }
    }
}