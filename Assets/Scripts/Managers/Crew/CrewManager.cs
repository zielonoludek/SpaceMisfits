using System.Collections.Generic;
using UnityEngine;

public class CrewManager : MonoBehaviour
{
    [Header("Crew List")]
    public List<CrewmateData> crewList = new List<CrewmateData>();


    public void RecruitCrewmate(CrewmateData newCrewmate)
    {
        if (!crewList.Contains(newCrewmate))
        {
            crewList.Add(newCrewmate);
            newCrewmate.ApplyEffect();
            Debug.Log($"Recruited {newCrewmate.crewmateName}!");
        }
        else
        {
            Debug.Log($"{newCrewmate.crewmateName} is already in the crew!");
        }
    }
}
