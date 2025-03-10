using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Sector : MonoBehaviour
{
    [Tooltip("Determines if this sector is the one where player starts the game")]
    [SerializeField] private bool isStartingSector;
    [SerializeField] private EventSO sectorEvent;
    [SerializeField] private Sprite sectorIcon;
    
    public static Sector GetCurrentStartingSector => currentStartingSector;
    public EventSO GetSectorEvent() => sectorEvent;
    
    private static Sector currentStartingSector;
    private HashSet<Sector> neighbors = new HashSet<Sector>();
    private Dictionary<Sector, Lane> connectedLanes = new Dictionary<Sector, Lane>();

    private MeshRenderer meshRenderer;
    private SpriteRenderer sectorIconRenderer;

    private static readonly Dictionary<EventType, Color> eventColors =
        new Dictionary<EventType, Color>
        {
            { EventType.FaintSignal, Color.white },
            { EventType.Waypoint, Color.green },
            { EventType.DevilsMaw, Color.blue },
            { EventType.SharpenThoseDirks, Color.red },
            { EventType.Spaceport, Color.yellow }
        };

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        InitializeSectorIcon();
        UpdateSectorColor();
        SetVisibility(false);
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
        // Defer the initialization of the sector icon (used to avoid warning messages)
        EditorApplication.delayCall += () =>
        {
            if (this != null)
            {
                InitializeSectorIcon();
                EditorUtility.SetDirty(this);
            }
        };
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

    public HashSet<Sector> GetNeighbors()
    {
        return neighbors;
    }

    public void SetSectorEvent(EventSO newEvent)
    {
        sectorEvent = newEvent;
        
        string finalName = newEvent != null ? newEvent.eventTitle : "Unnamed Event";
        gameObject.name = $"Sector ({finalName})";
        
        UpdateSectorColor();
        NotifyLane();
    }

    public void SetVisibility(bool isVisible)
    {
        gameObject.SetActive(isVisible);
        foreach (Lane lane in connectedLanes.Values)
        {
            lane.SetVisibility(isVisible);
        }
    }

    private void UpdateSectorColor()
    {
        if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();

        // Only modify instances in the scene
        if (!Application.isPlaying && PrefabUtility.IsPartOfPrefabAsset(gameObject))
        {
            return;
        }
        
        // Ensure the sector has a material assigned
        if (meshRenderer.sharedMaterial == null)
        {
            meshRenderer.sharedMaterial = new Material(Shader.Find("Sprites/Default")); // Assigns a default material
        }
        
        // Use sharedMaterial in edit mode and material in play mode
        Material targetMaterial = Application.isPlaying ? meshRenderer.material : meshRenderer.sharedMaterial;

        // If the sector has an event, use its corresponding color
        if (sectorEvent != null && eventColors.ContainsKey(sectorEvent.eventType))
        {
            targetMaterial.color = eventColors[sectorEvent.eventType];
        }
        else
        {
            targetMaterial.color = Color.gray;
        }
    }

    private void InitializeSectorIcon()
    {
        Transform iconTransform = transform.Find("SectorIcon");
        if (iconTransform != null)
        {
            sectorIconRenderer = iconTransform.GetComponent<SpriteRenderer>();
        }

        UpdateSectorIcon();
    }

    private void UpdateSectorIcon()
    {
        if (sectorIconRenderer != null)
        {
            sectorIconRenderer.sprite = sectorIcon;
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