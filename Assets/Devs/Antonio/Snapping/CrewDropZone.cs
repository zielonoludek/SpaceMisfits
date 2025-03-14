using System.Collections.Generic;
using UnityEngine;

public class CrewDropZone : MonoBehaviour
{
    public ShipPart shipPart;
    public bool isCrewQuarters;
    public int capacity = 1;
    private int currentCrewCount = 0;
    private Renderer rend;
    private Color originalColor;
    public List<Transform> snapPoints = new List<Transform>();
    private HashSet<Transform> usedSnapPoints = new HashSet<Transform>();

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            originalColor = rend.material.color;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var draggable = other.GetComponent<CrewMemberDraggable>();
        if (draggable != null)
        {
            if (IsAvailable())
            {
                HighlightZone(Color.green);
                Debug.Log("Valid drop zone entered.");
            }
            else
            {
                HighlightZone(Color.red);
                Debug.Log("Drop zone is full or unavailable.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var draggable = other.GetComponent<CrewMemberDraggable>();
        if (draggable != null)
        {
            ResetZoneColor();
            Debug.Log("Exited drop zone.");
        }
    }

    public void AssignCrew(CrewmateData crewData)
    {
        if (IsAvailable())
        {
            currentCrewCount++;
            Debug.Log($"{crewData.crewmateName} assigned to {shipPart?.partName ?? "Crew Quarters"}");
        }
        else
        {
            Debug.LogError("Cannot assign crew: Capacity reached.");
        }
    }

    public bool IsAvailable()
    {
        return currentCrewCount < capacity;
    }

    public Transform GetNextAvailableSnapPoint()
    {
        if (currentCrewCount < capacity && isCrewQuarters)
        {
            foreach (var snapPoint in snapPoints)
            {
                if (!usedSnapPoints.Contains(snapPoint))
                {
                    usedSnapPoints.Add(snapPoint);
                    return snapPoint;
                }
            }
        }

        return null;
    }

    public void HighlightZone(Color color)
    {
        if (rend != null)
        {
            rend.material.color = color;
        }
    }

    public void ResetZoneColor()
    {
        if (rend != null)
        {
            rend.material.color = originalColor;
        }
    }
}
