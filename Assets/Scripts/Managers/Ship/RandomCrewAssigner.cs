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
            if (availableCrewmates.Count == 0 && assignButton != null)
            {
                assignButton.interactable = false;
                Debug.Log("No more crew members available.");
            }
            else
            {
                Debug.LogError("Ensure all references are assigned and there are available crewmates.");
            }
            return;
        }

        if (!crewQuarters.IsAvailable())
        {
            Debug.Log("Crew Quarters is full. Cannot assign more crew members.");
            return;
        }

        int randomIndex = Random.Range(0, availableCrewmates.Count);
        CrewmateData randomCrewmate = availableCrewmates[randomIndex];
        Transform snapPoint = crewQuarters.GetNextAvailableSnapPoint();

        if (snapPoint != null)
        {
            GameObject crewMember = Instantiate(crewMemberPrefab, snapPoint.position, Quaternion.identity);           
            //crewMember.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            Renderer quadRenderer = crewMember.GetComponentInChildren<Renderer>();
            if (quadRenderer != null)
            {
                Material quadMaterial = quadRenderer.material;
                if (quadMaterial != null && randomCrewmate.crewmateImage != null)
                {
                    quadMaterial.mainTexture = randomCrewmate.crewmateImage.texture;
                    quadRenderer.gameObject.transform.localScale = new Vector3(8, 12, 1);
                }
            }
            
            CrewMemberDraggable draggable = crewMember.GetComponent<CrewMemberDraggable>();
            if (draggable != null)
            {
                draggable.crewmateData = randomCrewmate;
                draggable.shipPartManager = shipPartManager;
                crewMember.transform.position = snapPoint.position;
                crewMember.transform.SetParent(null);
            }

            crewQuarters.AssignCrew(randomCrewmate);
            if (crewQuarters.shipPart != null)
            {
                shipPartManager.AssignCrewmateToPart(randomCrewmate, crewQuarters.shipPart);
            }

            availableCrewmates.RemoveAt(randomIndex);

            if (availableCrewmates.Count == 0 && assignButton != null)
            {
                assignButton.interactable = false;
                Debug.Log("No more crew members available.");
            }
        }
        else
        {
            Debug.Log("No available snap points. Cannot assign crew member.");
        }
    }

}
