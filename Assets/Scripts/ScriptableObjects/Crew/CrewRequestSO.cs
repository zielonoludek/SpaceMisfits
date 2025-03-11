using UnityEngine;

[CreateAssetMenu(fileName = "NewCrewRequest", menuName = "Crew/CrewRequest")]
public class CrewRequestSO : ScriptableObject
{
    public string Name;
    public Sprite Artwork;

    public RequestType Type;
    //public bool IsFromCrewMember;
    public RequestOriginType Origin;
    public CrewMemberType specialMember;

    [Space]
    public float ExpirationTime;
    public float ExpirationPenalty;

    [SerializeReference] public IFulfillmentCondition FulfillmentCondition;
    [SerializeField, HideInInspector] public EventSO EventToExecute;

    [Space]
    public float StartTime;
    public Effect RewardType;
    public int RewardAmount;


    private void OnValidate()
    {
        if (Origin == RequestOriginType.Event && !(FulfillmentCondition is BoolFulfillmentCondition))
        {
            FulfillmentCondition = new BoolFulfillmentCondition();
            EventToExecute ??= null;
        }
        else if (Origin != RequestOriginType.Event && !(FulfillmentCondition is IntFulfillmentCondition))
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
    public bool ConditionMet;
}

[System.Serializable]
public class IntFulfillmentCondition : IFulfillmentCondition
{
    public int RequiredValue;
}
