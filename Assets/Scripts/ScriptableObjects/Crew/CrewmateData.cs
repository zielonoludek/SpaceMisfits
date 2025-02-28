using UnityEngine;

[CreateAssetMenu(fileName = "NewCrewmate", menuName = "Crew/CrewmateData")]
public class CrewmateData : ScriptableObject
{
    [Header("Crewmate Information")]
    public string crewmateName;
    [TextArea] public string crewmateDescription;

    [Header("Passive Effect")]
    public PassiveEffectType PassiveEffectType;
    public int effectValue = 1;

    public void ApplyEffect()
    {
        switch (PassiveEffectType)
        {
            case PassiveEffectType.IncreaseSight:
                ResourceManager.Instance.IncreaseSight(effectValue);
                Debug.Log($"{crewmateName} increased Sight by {effectValue}!");
                break;

            case PassiveEffectType.IncreaseBooty:
                ResourceManager.Instance.Booty += effectValue;
                Debug.Log($"{crewmateName} added {effectValue} Booty!");
                break;
            
            case PassiveEffectType.IncreaseNotoriety:
                ResourceManager.Instance.Notoriety += effectValue;
                Debug.Log($"{crewmateName} increased Notoriety by {effectValue}!");
                break;

            default:
                Debug.Log($"{crewmateName} has no effect.");
                break;
        }
    }
}

public enum PassiveEffectType
{
    None,
    IncreaseSight,
    IncreaseBooty,
    IncreaseNotoriety
}

/*
These lines should be used in code when the need to recuirt a crew member
CrewmateData newCrewmate = Resources.Load<CrewmateData>("CaptainRads");
CrewManager.Instance.RecruitCrewmate(newCrewmate);
*/
