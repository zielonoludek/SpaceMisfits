using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Sector : MonoBehaviour
{
    [Tooltip("Determines if this sector is the one where player starts the game")]
    [SerializeField] private bool isStartingSector;
    [SerializeField] private SectorEventSO sectorEvent;
    
    public static Sector GetCurentStartingSector => currentStartingSector;
    public SectorEventSO GetSectorEvent() => sectorEvent;
    
    private static Sector currentStartingSector;
    private HashSet<Sector> neighbors = new HashSet<Sector>();
    private Dictionary<Sector, Lane> connectedLanes = new Dictionary<Sector, Lane>();

    private MeshRenderer meshRenderer;

    private static readonly Dictionary<SectorEventSO.EventType, Color> eventColors =
        new Dictionary<SectorEventSO.EventType, Color>
        {
            { SectorEventSO.EventType.FaintSignal, Color.white },
            { SectorEventSO.EventType.Waypoint, Color.green },
            { SectorEventSO.EventType.DevilsMaw, Color.blue },
            { SectorEventSO.EventType.SharpenThoseDirks, Color.red }
        };

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        UpdateSectorColor();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Ensures sector event and name are updated when changed in editor
        SetSectorEvent(sectorEvent);
        
        // Ensures only one sector is the starting sector
        if (isStartingSector)
        {
            if (currentStartingSector != null && currentStartingSector != this)
            {
                currentStartingSector.isStartingSector = false;
                EditorUtility.SetDirty(currentStartingSector);
            }

            currentStartingSector = this;
        }
    }
#endif
    
    public void AddNeighbor(Sector sector, Lane lane)
    {
        if (sector != null && lane != null && !connectedLanes.ContainsKey(sector))
        {
            neighbors.Add(sector);
            connectedLanes[sector] = lane;
        }
    }

    public bool IsNeighbor(Sector sector)
    {
        return neighbors.Contains(sector);
    }

    public void SetSectorEvent(SectorEventSO newEvent)
    {
        sectorEvent = newEvent;
        
        string finalName = newEvent != null ? newEvent.eventTitle : "Unnamed Event";
        gameObject.name = $"Sector ({finalName})";
        
        UpdateSectorColor();
        NotifyLane();
    }

    private void UpdateSectorColor()
    {
        if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();

        if (sectorEvent != null && eventColors.ContainsKey(sectorEvent.eventType))
        {
            meshRenderer.sharedMaterial.color = eventColors[sectorEvent.eventType];
        }
        else
        {
            meshRenderer.sharedMaterial.color = Color.white;
        }
    }

    private void NotifyLane()
    {
        foreach (var lane in connectedLanes.Values)
        {
            lane.NotifySectorEventChanged();
        }
    }
}
