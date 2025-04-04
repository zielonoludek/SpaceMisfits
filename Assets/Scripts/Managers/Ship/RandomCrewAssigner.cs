using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RandomCrewAssigner : MonoBehaviour
{
    public List<CrewmateData> availableCrewmates;
    public CrewDropZone crewQuarters;
    public GameObject crewMemberPrefab;
    public Button assignButton;
    public ShipPartManager shipPartManager;

    private IEnumerator Start()
    {
        yield return null;

        AssignSingleRandomNonQuestCrewmate();
        AssignSingleRandomNonQuestCrewmate();

        if (assignButton != null)
        {
            assignButton.onClick.AddListener(AssignSingleRandomNonQuestCrewmate);
        }
    }

    private void AssignSingleRandomNonQuestCrewmate()
    {
        List<CrewmateData> nonQuestCrew = availableCrewmates.FindAll(crewmate => !crewmate.isQuestMember);

        if (crewQuarters == null || crewMemberPrefab == null || nonQuestCrew.Count == 0)
        {
            if (nonQuestCrew.Count == 0 && assignButton != null)
            {
                assignButton.interactable = false;
                Debug.Log("No more non‑quest crew members available.");
            }
            else
            {
                Debug.LogError("Ensure all references are assigned and that there are available non‑quest crew members.");
            }
            return;
        }

        int randomIndex = Random.Range(0, nonQuestCrew.Count);
        CrewmateData randomCrewmate = nonQuestCrew[randomIndex];
        AssignCrewMember(randomCrewmate);
        availableCrewmates.Remove(randomCrewmate);

        if (availableCrewmates.Count == 0 && assignButton != null)
        {
            assignButton.interactable = false;
            Debug.Log("No more crew members available.");
        }
    }

    private void AssignCrewMember(CrewmateData crew)
    {
        if (!crewQuarters.IsAvailable())
        {
            Debug.Log("Crew Quarters is full. Cannot assign more crew members.");
            return;
        }

        Transform snapPoint = crewQuarters.GetNextAvailableSnapPoint();
        if (snapPoint != null)
        {
            GameObject crewMember = Instantiate(crewMemberPrefab, snapPoint.position, Quaternion.identity);
            Renderer quadRenderer = crewMember.GetComponentInChildren<Renderer>();
            if (quadRenderer != null)
            {
                Material quadMaterial = quadRenderer.material;
                if (quadMaterial != null && crew.crewmateImage != null)
                {
                    quadMaterial.mainTexture = crew.crewmateImage.texture;
                    quadRenderer.gameObject.transform.localScale = new Vector3(8, 12, 1);
                }
            }
            CrewMemberDraggable draggable = crewMember.GetComponent<CrewMemberDraggable>();
            if (draggable != null)
            {
                draggable.crewmateData = crew;
                draggable.shipPartManager = shipPartManager;
                crewMember.transform.SetParent(crewQuarters.transform);
                crewMember.transform.position = snapPoint.position;
                draggable.UpdateDropZoneReferences(crewQuarters.transform, snapPoint);
            }
            crewQuarters.AssignCrew(crew);
            if (crewQuarters.shipPart != null)
            {
                shipPartManager.AssignCrewmateToPart(crew, crewQuarters.shipPart);
            }
        }
        else
        {
            Debug.Log("No available snap points. Cannot assign crew member.");
        }
    }
}