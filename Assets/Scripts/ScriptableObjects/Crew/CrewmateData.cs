using UnityEngine;

[CreateAssetMenu(fileName = "NewCrewmate", menuName = "Crew/CrewmateData")]
public class CrewmateData : ScriptableObject
{
    [Header("Crewmate Information")]
    public string crewmateName;
    public Sprite crewmateImage;
    public CrewMemberType crewMemberType;

    [TextArea] public string crewmateDescription;
    public GenericEffect genericEffect;

    public void ApplyEffect()
    {
        if (genericEffect != null)
        {
            genericEffect.ApplyEffect();
            Debug.Log($"{crewmateName} applied effect: {genericEffect.effectName}");
        }
        else
        {
            Debug.Log($"{crewmateName} has no effect.");
        }
    }
}

/*
These lines should be used in code when the need to recuirt a crew member
CrewmateData newCrewmate = Resources.Load<CrewmateData>("CaptainRads");
GameManager.Instance.CrewManager.RecruitCrewmate(newCrewmate);
*/
