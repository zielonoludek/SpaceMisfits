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
        
        string finalName = newEvent != null ? newEvent.GetEventTitle() : "Unnamed Event";
        gameObject.name = $"Sector ({finalName})";
        
        NotifyLane();
    }

    private void NotifyLane()
    {
        foreach (var lane in connectedLanes.Values)
        {
            lane.NotifySectorEventChanged();
        }
    }
}
