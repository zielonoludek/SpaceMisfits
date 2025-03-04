using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCrewRequest", menuName = "Crew/Request")]
public class CrewRequest : ScriptableObject
{
    public string requestName;
    [Range(1, 10)] public int difficultyLevel;
    public string fulfillmentCondition;
    public DateTime expirationDate;

    [Header("Progress Tracking")]
    public int requiredAmount;
    public int currentProgress;

    public bool IsFulfilled() => currentProgress >= requiredAmount;
    public bool IsExpired() => DateTime.Now > expirationDate;
}
