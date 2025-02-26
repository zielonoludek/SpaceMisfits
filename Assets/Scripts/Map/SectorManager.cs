using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class SectorManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private EventPopupUI eventPopupUI;
    private static GameObject playerInstance;
    private static Sector playerCurrentSector;
    private static SectorManager Instance;

    private static bool bIsPlayerMoving = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnPlayerAtStartingSector();
    }

    private void SpawnPlayerAtStartingSector()
    {
        Sector startingSector = Sector.GetCurentStartingSector;

        if (startingSector == null)
        {
            Debug.LogError("No starting sector found! Make sure one sector is set as starting sector");
            return;
        }
        
        // Spawn player at the starting sector's position
        playerInstance = Instantiate(playerPrefab, startingSector.transform.position, quaternion.identity);
        
        // Set the player's current sector
        playerCurrentSector = startingSector;
    }

    public static void MovePlayerToSector(Sector newSector)
    {
        if(playerInstance == null || playerCurrentSector == null) return;
        if(!playerCurrentSector.IsNeighbor(newSector)) return;

        Lane lane = FindLaneBetween(playerCurrentSector, newSector);
        if(lane == null) return;

        Vector3[] path = lane.GetLanePath();
        float speed = lane.GetLaneLength();
        
        // reverse the path if moving from sectorB to sectorA
        if (lane.sectorB == playerCurrentSector && lane.sectorA == newSector)
        {
            Array.Reverse(path);
        }

        Instance.StartCoroutine(Instance.AnimatePlayerMovement(path, speed, newSector));
    }

    private IEnumerator AnimatePlayerMovement(Vector3[] path, float speed, Sector targetSector)
    {
        // Ensure valid path
        if(path.Length < 2) yield break;

        int index = 0;
        while (index < path.Length - 1)
        {
            bIsPlayerMoving = true;
            Vector3 start = path[index];
            Vector3 end = path[index + 1];

            // Time needed per segment
            float segmentDuration = Vector3.Distance(start, end) / speed;
            float elapsedTime = 0f;

            while (elapsedTime < segmentDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / segmentDuration;
                playerInstance.transform.position = Vector3.Lerp(start, end, t);
                yield return null;
            }

            index++;
        }

        playerInstance.transform.position = targetSector.transform.position;
        playerCurrentSector = targetSector;
        bIsPlayerMoving = false;
        TriggerSectorEvent(targetSector);
    }

    private static Lane FindLaneBetween(Sector sectorA, Sector sectorB)
    {
        Lane[] lanes = FindObjectsByType<Lane>(FindObjectsSortMode.None);
        foreach (Lane lane in lanes)
        {
            if ((lane.sectorA == sectorA && lane.sectorB == sectorB) || (lane.sectorA == sectorB && lane.sectorB == sectorA))
            {
                return lane;
            }
        }

        return null;
    }

    private void TriggerSectorEvent(Sector sector)
    {
        SectorEventSO eventSO = sector.GetSectorEvent();
        if (eventSO != null)
        {
            if (eventPopupUI != null)
            {
                eventPopupUI.ShowEvent(eventSO);
            }
            else
            {
                Debug.LogError("EventUI not found in the scene!");
            }
            // Debug.Log($"Event triggered:  {eventSO.GetEventTitle()}");
        }
    }

    public static Sector GetPlayerCurrentSector()
    {
        return playerCurrentSector;
    }

    public static bool IsPlayerMoving()
    {
        return bIsPlayerMoving;
    }
}
