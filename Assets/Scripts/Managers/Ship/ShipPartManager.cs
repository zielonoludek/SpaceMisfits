using UnityEngine;
using System.Collections.Generic;

public class ShipPartManager : MonoBehaviour
{
    [System.Serializable]
    public class ShipPartAssignment
    {
        public ShipPart shipPart;
        public Transform partPosition;
        public List<CrewmateData> assignedCrewmates = new List<CrewmateData>();
        public HashSet<CrewmateData> effectedCrewmates = new HashSet<CrewmateData>();
    }

    [Header("Ship Part Assignments")]
    public List<ShipPartAssignment> shipParts = new List<ShipPartAssignment>();

    private void Start()
    {
        InitializeShipParts();
    }

    private void InitializeShipParts()
    {
        foreach (var assignment in shipParts)
        {
            assignment.assignedCrewmates.Clear();
            assignment.effectedCrewmates.Clear();
            UpdatePartState(assignment);
        }
    }

    public void AssignCrewmateToPart(CrewmateData crewmate, ShipPart targetPart)
    {
        foreach (var assignment in shipParts)
        {
            if (assignment.assignedCrewmates.Contains(crewmate))
            {
                RemoveCrewmateFromPart(crewmate, assignment.shipPart);
                break;
            }
        }

        foreach (var assignment in shipParts)
        {
            if (assignment.shipPart == targetPart)
            {
                if (!assignment.assignedCrewmates.Contains(crewmate))
                {
                    assignment.assignedCrewmates.Add(crewmate);
                    Debug.Log($"{crewmate.crewmateName} assigned to {targetPart.partName}");
                }

                if (!assignment.effectedCrewmates.Contains(crewmate))
                {
                    assignment.effectedCrewmates.Add(crewmate);
                    crewmate.ApplyEffect();
                    Debug.Log($"Effect applied for {crewmate.crewmateName} on {targetPart.partName}");
                }

                UpdatePartState(assignment);
                break;
            }
        }
    }

    private void UpdatePartState(ShipPartAssignment assignment)
    {
        if (assignment.assignedCrewmates.Count > 0)
        {
            assignment.shipPart.ApplyEffect();
            Debug.Log($"{assignment.shipPart.partName} is enhanced by assigned crew.");
        }
        else
        {
            Debug.Log($"{assignment.shipPart.partName} has no crew assigned and thus no bonus effect.");
        }
    }

    public void RemoveCrewmateFromPart(CrewmateData crewmate, ShipPart targetPart)
    {
        foreach (var assignment in shipParts)
        {
            if (assignment.shipPart == targetPart && assignment.assignedCrewmates.Contains(crewmate))
            {
                assignment.assignedCrewmates.Remove(crewmate);
                assignment.effectedCrewmates.Remove(crewmate);
                Debug.Log($"{crewmate.crewmateName} removed from {targetPart.partName}");
                UpdatePartState(assignment);
                break;
            }
        }
    }

    public ShipPart GetCrewQuartersPart()
    {
        foreach (var assignment in shipParts)
        {
            if (assignment.shipPart != null && assignment.shipPart.partName == "Crew Quarters")
            {
                return assignment.shipPart;
            }
        }
        return null;
    }
}