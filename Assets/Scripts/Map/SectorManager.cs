using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class SectorManager : MonoBehaviour
{
    [Serializable]
    public class EventMaterial
    {
        public EventType eventType;
        public Material material;
    }

    public delegate void ETAUpdateHandler(float remainingSeconds);
    public event ETAUpdateHandler OnETAUpdated;

    [SerializeField] Sector startingSector;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<TiedEventSequenceSO> tiedEventSequences;

    private GameObject playerInstance;
    private Sector playerCurrentSector;
    [SerializeField] private FogOfWarController fogOfWarController;

    [Header("Event Materials")]
    [SerializeField] private List<EventMaterial> eventMaterialsList = new List<EventMaterial>();
    
    public Dictionary<EventType, Material> eventMaterials = new Dictionary<EventType, Material>();

    private static SectorManager instance;
    public static SectorManager Instance => instance;

    private readonly System.Random random = new();
    
    private float currentETA = 0f;
    public float CurrentETA => currentETA;

    private bool bIsPlayerMoving = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        
        foreach (var eventMaterial in eventMaterialsList)
        {
            eventMaterials.TryAdd(eventMaterial.eventType, eventMaterial.material);
        }
    }

    private void Start()
    {
        StartCoroutine(InitializeGame());

        foreach (var sequence in tiedEventSequences)
        {
            sequence.ResetSequence();
        }
    }
    private IEnumerator InitializeGame()
    {
        yield return new WaitUntil(() => GameManager.Instance != null && GameManager.Instance.GameScene == GameScene.Map);
        SpawnPlayerAtStartingSector();
    }

    public void MovePlayerToSector(Sector newSector)
    {
        if (playerInstance == null || playerCurrentSector == null) return;
        if (!playerCurrentSector.IsNeighbor(newSector)) 
        {
            Debug.Log("its not neighbor");
            return;
        }

        Lane lane = FindLaneBetween(playerCurrentSector, newSector);
        if (lane == null)
        {
            Debug.Log("lane null");
            return;
        }

        EventSO currentEvent = playerCurrentSector.GetSectorEvent();
        if (currentEvent != null && !currentEvent.isPersistent)
        {
            playerCurrentSector.SetSectorEvent(null);
        }

        Vector3[] path = lane.GetLanePath();
        float distance = lane.GetLaneDistance();

        if (lane.GetSectorB() == playerCurrentSector) Array.Reverse(path);

        StartCoroutine(AnimatePlayerMovement(path, distance, newSector));
    }

    public Sector GetPlayerCurrentSector() => playerCurrentSector;
    public bool IsPlayerMoving() => bIsPlayerMoving;

    private void SpawnPlayerAtStartingSector()
    {
        if (startingSector == null) startingSector = Sector.GetCurrentStartingSector;

        playerInstance = Instantiate(playerPrefab, startingSector.transform.position, quaternion.identity);
        playerCurrentSector = startingSector;
        
        if (fogOfWarController != null)
        {
            fogOfWarController.SetPlayerTransform(playerInstance.transform);
        }
    }

    private IEnumerator AnimatePlayerMovement(Vector3[] path, float distance, Sector targetSector)
    {
        if (path.Length < 2) yield break;

        bIsPlayerMoving = true;

        float halfDayInGame = GameManager.Instance.TimeManager.DayLength / 2f ;
        float totalTravelTimeRealSeconds = distance * halfDayInGame;

        float totalPathLength = 0f;
        for (int i = 0; i < path.Length - 1; i++)
        {
            totalPathLength += Vector3.Distance(path[i], path[i + 1]);
        }
        
        currentETA = totalTravelTimeRealSeconds / GameManager.Instance.ResourceManager.Speed;
        OnETAUpdated?.Invoke(currentETA);

        int index = 0;
        while (index < path.Length - 1)
        {
            Vector3 start = path[index];
            Vector3 end = path[index + 1];

            yield return new WaitUntil(() => GameManager.Instance.ResourceManager.Speed > 0);
            float segmentDurationRealSeconds = (Vector3.Distance(start, end) / totalPathLength) * totalTravelTimeRealSeconds / GameManager.Instance.ResourceManager.Speed;
            float elapsedTime = 0f;
            while (elapsedTime < segmentDurationRealSeconds)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / segmentDurationRealSeconds;
                playerInstance.transform.position = Vector3.Lerp(start, end, t);
                
                currentETA -= Time.deltaTime;
                OnETAUpdated?.Invoke(currentETA);
                
                yield return null;
            }

            index++;
        }

        playerInstance.transform.position = targetSector.transform.position;
        playerCurrentSector = targetSector;
        bIsPlayerMoving = false;

        currentETA = 0f;
        OnETAUpdated?.Invoke(0);
        
        TriggerSectorEvent(targetSector);
    }

    private float CalculateETA(Sector targetSector)
    {
        if (playerInstance == null || playerCurrentSector == null) return -1f;
        if (!playerCurrentSector.IsNeighbor(targetSector)) return -1f;
        
        Lane lane = FindLaneBetween(playerCurrentSector, targetSector);
        if (lane == null) return -1f;
        
        float distance = lane.GetLaneDistance();
        float halfDayInGame = GameManager.Instance.TimeManager.DayLength / 2f;
        float totalTravelTimeRealSeconds = distance * halfDayInGame / GameManager.Instance.ResourceManager.Speed;

        return totalTravelTimeRealSeconds;
    }
    
    public string FormatTime(float timeInSeconds)
    {
        if (timeInSeconds <= 0) return "00:00";
    
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return $"{minutes:00}:{seconds:00}";
    }

    private Lane FindLaneBetween(Sector sectorA, Sector sectorB)
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
        bool eventHandled = false;

        foreach (var sequence in tiedEventSequences)
        {
            EventSO sequenceEvent = sequence.GetCurrentEvent();
            if (sequenceEvent != null && sequenceEvent == eventSO)
            {
                sequence.MarkCurrentEventAsCompleted();
                EventSO nextEvent = sequence.GetCurrentEvent();
                if (nextEvent != null)
                {
                    Sector nextSector = FindSectorByEvent(nextEvent);
                    if (nextSector != null)
                    {
                        if (sequence.enableVisualization)
                        {
                            nextSector.StartPulsating();
                        }
                    }
                }
                sector.StopPulsating();
                ShowEventUI(eventSO);
                eventHandled = true;
            }
        }

        if (!eventHandled && eventSO != null)
        {
            ShowEventUI(eventSO);
        }
        else if (!eventHandled)
        {
            AssignRandomSectorEvent(sector);
        }
    }

    private Sector FindSectorByEvent(EventSO targetEvent)
    {
        Sector[] allSectors = Resources.FindObjectsOfTypeAll<Sector>();
        foreach (Sector sector in allSectors)
        {
            if (sector.GetSectorEvent() == targetEvent)
            {
                return sector;
            }
        }
        Debug.LogWarning($"Could not find sector for event: {targetEvent.eventTitle}");
        return null;
    }

    private void ShowEventUI(EventSO eventSO)
    {
        if (eventSO is SectorEventSO sectorEvent)
        {
            GameManager.Instance.UIManager.EventPanelUI?.ShowEvent(sectorEvent);
        }
        else if (eventSO is FightEventSO fightEvent)
        {
            GameManager.Instance.FightManager.StartFight(fightEvent);
        }
    }

    private void AssignRandomSectorEvent(Sector sector)
    {
        int roll = random.Next(100);

        if (roll < 60)
        {
            SectorEventSO emptySpaceEvent = GetEmptySpaceEvent();
            sector.SetSectorEvent(emptySpaceEvent);
            GameManager.Instance.UIManager.EventPanelUI.ShowEvent(emptySpaceEvent);
        }
        else if (roll < 80)
        {
            GameManager.Instance.FightManager.StartFight();
        }
        else
        {
            SectorEventSO[] allEvents = Resources.LoadAll<SectorEventSO>("ScriptableObjects/Events")
                .Where(e => e.eventType != EventType.Spaceport && e.eventType != EventType.Waypoint)
                .ToArray();

            if (allEvents.Length > 0)
            {
                SectorEventSO randomEvent = allEvents[UnityEngine.Random.Range(0, allEvents.Length)];
                sector.SetSectorEvent(randomEvent);
                GameManager.Instance.UIManager.EventPanelUI.ShowEvent(randomEvent);
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
