using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RandomCrewAssigner : MonoBehaviour
{
    public List<CrewmateData> availableCrewmates;
    public CrewDropZone crewQuarters;
    public GameObject crewMemberPrefab;
    public Button assignButton;
    public ShipPartManager shipPartManager;

    private void Start()
    {
        if (assignButton != null)
        {
            assignButton.onClick.AddListener(AssignSingleRandomCrewmate);
        }
    }

    private void AssignSingleRandomCrewmate()
    {
        if (crewQuarters == null || crewMemberPrefab == null || availableCrewmates.Count == 0)
        {
            Debug.LogError("Ensure all references are assigned and there are available crewmates.");
            return;
        }

        if (!crewQuarters.IsAvailable())
        {
            Debug.Log("Crew Quarters is full. Cannot assign more crew members.");
            return;
        }

        CrewmateData randomCrewmate = availableCrewmates[Random.Range(0, availableCrewmates.Count)];
        Transform snapPoint = crewQuarters.GetNextAvailableSnapPoint();

        if (snapPoint != null)
        {
            GameObject crewMember = Instantiate(crewMemberPrefab, snapPoint.position, Quaternion.identity);
            CrewMemberDraggable draggable = crewMember.GetComponent<CrewMemberDraggable>();

            if (draggable != null)
            {
                draggable.crewmateData = randomCrewmate;
                draggable.shipPartManager = shipPartManager;
                crewMember.transform.position = snapPoint.position;
            }

            crewQuarters.AssignCrew(randomCrewmate);
        }
        else
        {
            Debug.Log("No available snap points. Cannot assign crew member.");
        }
    }
}
