using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCrewRequest", menuName = "Crew/CrewRequest")]
public class CrewRequest : ScriptableObject
{
    public string Name;
    public Sprite Artwork;

    public RequestType Type;
    public RequestOriginType Origin;
    public CrewMemberType specialMember;
    
    public bool IsFromCrewMember;
    
    public int ExpirationTime;
    public float ExpirationPenalty;
    
    public string FulfillmentCondition;
    public string FulfillmentReward;
}
