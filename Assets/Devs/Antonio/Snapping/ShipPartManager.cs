using UnityEngine;
using System.Collections.Generic;

public class ShipPartManager : MonoBehaviour
{
    [System.Serializable]
    public class ShipPartAssignment
    {
        public ShipPart shipPart;
        public Transform partPosition;
        public GameObject boundaryVisual;
        public List<CrewmateData> assignedCrewmates = new List<CrewmateData>();
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
            if (assignment.boundaryVisual != null)
            {
                assignment.boundaryVisual.SetActive(true);
            }

            assignment.assignedCrewmates.Clear();
            UpdatePartState(assignment);
        }
    }

    public void AssignCrewmateToPart(CrewmateData crewmate, ShipPart targetPart)
    {
        foreach (var assignment in shipParts)
        {
            if (assignment.shipPart == targetPart)
            {
                assignment.assignedCrewmates.Add(crewmate);
                Debug.Log($"{crewmate.crewmateName} assigned to {targetPart.partName}");
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
            foreach (var crewmate in assignment.assignedCrewmates)
            {
                crewmate.ApplyEffect();
                Debug.Log($"{assignment.shipPart.partName} is enhanced by {crewmate.crewmateName}");
            }
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
            if (assignment.shipPart == targetPart)
            {
                assignment.assignedCrewmates.Remove(crewmate);
                Debug.Log($"{crewmate.crewmateName} removed from {targetPart.partName}");
                UpdatePartState(assignment);
                break;
            }
        }
    }
}