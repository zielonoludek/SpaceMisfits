using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class SectorManager : MonoBehaviour
{
    [Serializable]
    public struct SightLevelSettings
    {
        // How far player can see sectors
        public int sectorVisibility;
        // How far spaceports can be seen
        public int spaceportVisibility;
    }

    [Header("Sight Settings")]
    [SerializeField] private SightLevelSettings[] sightLevels = new SightLevelSettings[4]
    {
        new SightLevelSettings { sectorVisibility = 1, spaceportVisibility = 3 },
        new SightLevelSettings { sectorVisibility = 2, spaceportVisibility = 3 },
        new SightLevelSettings { sectorVisibility = 3, spaceportVisibility = 4 },
        new SightLevelSettings { sectorVisibility = 4, spaceportVisibility = 5 } 
    };
    
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private EventPopupUI eventPopupUI;
    
    private static GameObject playerInstance;
    private static Sector playerCurrentSector;
    private static SectorManager Instance;
    
    System.Random random = new System.Random();

    private static bool bIsPlayerMoving = false;
    
    // Stores all discovered sectors
    private static HashSet<Sector> visibleSectors = new HashSet<Sector>();
    // Stores all visited sectors
    private static HashSet<Sector> visitedSectors = new HashSet<Sector>();
    // Stores all discovered lanes
    private static HashSet<Lane> discoveredLanes = new HashSet<Lane>();


    #region Public functions

    public static void MovePlayerToSector(Sector newSector)
    {
        // Temporary, for fights to work
        GameManager.Instance.ResourceManager.Notoriety += 100;
        
        if(playerInstance == null || playerCurrentSector == null) return;
        if(!playerCurrentSector.IsNeighbor(newSector)) return;

        Lane lane = FindLaneBetween(playerCurrentSector, newSector);
        if(lane == null) return;

        // If the current sector event is not persistent, remove it
        EventSO currentEvent = playerCurrentSector.GetSectorEvent();
        if (currentEvent != null && !currentEvent.isPersistent)
        {
            playerCurrentSector.SetSectorEvent(null);
        }

        Vector3[] path = lane.GetLanePath();
        float speed = lane.GetLaneDistance();
        
        // reverse the path if moving from sectorB to sectorA
        if (lane.GetSectorB() == playerCurrentSector) Array.Reverse(path);

        Instance.StartCoroutine(Instance.AnimatePlayerMovement(path, speed, newSector));
    }
    
    public static Sector GetPlayerCurrentSector()
    {
        return playerCurrentSector;
    }

    public static bool IsPlayerMoving()
    {
        return bIsPlayerMoving;
    }

    #endregion
    
    
    private void Awake()
    {
        Instance = this;
        StartCoroutine(InitializeGame());
    }

    private IEnumerator InitializeGame()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
        GameManager.Instance.ResourceManager.OnSightChanged += UpdateVisibility;
        SpawnPlayerAtStartingSector();
    }

    private void SpawnPlayerAtStartingSector()
    {
        Sector startingSector = Sector.GetCurrentStartingSector;

        if (startingSector == null)
        {
            Debug.LogError("No starting sector found! Make sure one sector is set as starting sector");
            return;
        }
        
        // Spawn player at the starting sector's position
        playerInstance = Instantiate(playerPrefab, startingSector.transform.position, quaternion.identity);
        
        // Set the player's current sector
        playerCurrentSector = startingSector;
        
        // Reveal sectors and lanes neighboring to the starting sector
        RevealSector(startingSector, GameManager.Instance.ResourceManager.GetCurrentSight());
    }

    private IEnumerator AnimatePlayerMovement(Vector3[] path, float speed, Sector targetSector)
    {
        // Ensure valid path
        if(path.Length < 2) yield break;
        
        bIsPlayerMoving = true;
        int index = 0;
        
        while (index < path.Length - 1)
        {
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
        RevealSector(targetSector, GameManager.Instance.ResourceManager.GetCurrentSight());
    }

    // Called whenever sight level changes
    private void UpdateVisibility(int sightLevel)
    {
        foreach (var sector in visibleSectors)
        {
            sector.SetVisibility(false);
        }
        visibleSectors.Clear();
        discoveredLanes.Clear();
        RevealSector(playerCurrentSector, sightLevel);
    }

    private static void RevealSector(Sector sector, int sightLevel, int depth = 0)
    {
        if (depth >= Instance.sightLevels[sightLevel].sectorVisibility)
            return;
        
        int sectorVisibility = Instance.sightLevels[sightLevel].sectorVisibility;
        int spaceportVisibility = Instance.sightLevels[sightLevel].spaceportVisibility;
        
        // Mark sector as discovered and add it to the discovered sectors list
        sector.SetVisibility(true);
        visibleSectors.Add(sector);

        // Loop through sector's neighbors
        foreach (Sector neighbor in sector.GetNeighbors())
        {
            if (!visibleSectors.Contains(neighbor))
            {
                neighbor.SetVisibility(true);
                visibleSectors.Add(neighbor);

                // Recursively reveal neighbors based on sector visibility
                if (depth < sectorVisibility)
                {
                    RevealSector(neighbor, sightLevel, depth + 1);
                }
            }

            // Check for spaceports further away
            RevealDistantSpaceports(neighbor, 1, spaceportVisibility);

            // Find the lane between current sector and this neighbor
            Lane connectingLane = FindLaneBetween(sector, neighbor);
            if (connectingLane != null)
            {
                // Show this lane and add to discovered lanes
                connectingLane.SetVisibility(true);
                discoveredLanes.Add(connectingLane);
            }
        }
        
        // Loop through all lanes in the scene
        foreach (Lane lane in FindObjectsByType<Lane>(FindObjectsSortMode.None))
        {
            // If the lane is not discovered, hide it
            if (!discoveredLanes.Contains(lane))
            {
                lane.SetVisibility(false);
            }
        }
    }

    private static void RevealDistantSpaceports(Sector sector, int depth, int maxDepth)
    {
        // Stop searching after x sectors distance based on sight level
        if(depth >= maxDepth) return;

        foreach (Sector nextNeighbor in sector.GetNeighbors())
        {
            if (visibleSectors.Contains(nextNeighbor)) continue;

            // Reveal if it's a spaceport
            if (nextNeighbor.GetSectorEvent() is SectorEventSO sectorEvent && sectorEvent.eventType == EventType.Spaceport)
            {
                nextNeighbor.SetVisibility(true);
                visibleSectors.Add(nextNeighbor);
            }
            
            // If no spaceport found, search again, deeper
            RevealDistantSpaceports(nextNeighbor, depth + 1, maxDepth);
        }
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
        EventSO eventSO = sector.GetSectorEvent();

        visitedSectors.Add(sector);

        if (eventSO != null)
        {
            if (eventSO.IsAnyLinkedEventCompleted())
            {
                Debug.Log($"A linked event was completed before visiting {sector.name}");
            }
            
            if (eventSO is SectorEventSO sectorEvent)
            {
                if (eventPopupUI != null)
                {
                    eventPopupUI.ShowEvent(sectorEvent);
                }
            }
            else if (eventSO is FightEventSO fightEvent)
            {
                GameManager.Instance.FightManager.StartFight(fightEvent);
            }
            
            eventSO.MarkEventAsCompleted();
            return;
        }
        
        AssignRandomSectorEvent(sector);
    }

    private void AssignRandomSectorEvent(Sector sector)
    {
        int roll = random.Next(100);

        if (roll < 60)
        {
            SectorEventSO emptySpaceEvent = GetEmptySpaceEvent();
            sector.SetSectorEvent(emptySpaceEvent);
            eventPopupUI.ShowEvent(emptySpaceEvent);
        }
        else if (roll < 80)
        {
            GameManager.Instance.FightManager.StartFight();
        }
        else
        {
            // Gets random sector event from events folder, excluding Spaceports and Waypoints
            SectorEventSO[] allEvents = Resources.LoadAll<SectorEventSO>("ScriptableObjects/Events")
                .Where(e => e.eventType != EventType.Spaceport && e.eventType != EventType.Waypoint)
                .ToArray();
            
            if (allEvents.Length > 0)
            {
                SectorEventSO randomEvent = allEvents[UnityEngine.Random.Range(0, allEvents.Length)];
                sector.SetSectorEvent(randomEvent);
                eventPopupUI.ShowEvent(randomEvent);
            }
        }
    }

    private SectorEventSO GetEmptySpaceEvent()
    {
        SectorEventSO emptySpaceEvent = Resources.LoadAll<SectorEventSO>("ScriptableObjects/Events")
            .FirstOrDefault(e => e.eventType == EventType.EmptySpace);

        if (emptySpaceEvent == null)
        {
            Debug.LogWarning("No Empty Space event found! Please create one in 'Resources/ScriptableObjects/Events'");
        }

        return emptySpaceEvent;
    }
}