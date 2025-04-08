using System;
using System.Collections;
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
    private Coroutine pulsatingCoroutine;
    private bool isPulsating = false;
    
    #region Public functions

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
        
        UpdateSectorMaterial();
        UpdateSectorIcon();
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

    public void StartPulsating()
    {
        if (!isPulsating && gameObject.activeSelf)
        {
            isPulsating = true;
            pulsatingCoroutine = StartCoroutine(PulsateEffect());
        }
    }
    
    public void StopPulsating()
    {
        if (isPulsating)
        {
            isPulsating = false;
            if (pulsatingCoroutine != null)
            {
                StopCoroutine(pulsatingCoroutine);
                pulsatingCoroutine = null;
            }
            transform.localScale = Vector3.one  * 0.4f;
        }
    }
    #endregion



    
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Ensures sector event and name are updated when changed in editor
        SetSectorEvent(sectorEvent);

        // Ensures only one sector is the starting sector
        if (isStartingSector)
        {
            // Add null check here
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
                // Only update material if SectorManager exists
                if (SectorManager.Instance != null)
                {
                    UpdateSectorMaterial();
                }
                EditorUtility.SetDirty(this);
            }
        };
    }
#endif
    

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        InitializeSectorIcon();
        UpdateSectorMaterial();
        SetVisibility(false);
    }

    private void UpdateSectorMaterial()
    {
        if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null) return;
    
        // Only modify instances in the scene
        if (!Application.isPlaying && PrefabUtility.IsPartOfPrefabAsset(gameObject))
        {
            return;
        }
    
        // When there's a valid event with a material defined in SectorManager
        if (sectorEvent != null && SectorManager.Instance != null && SectorManager.Instance.eventMaterials.ContainsKey(sectorEvent.eventType))
        {
            Material materialToUse = SectorManager.Instance.eventMaterials[sectorEvent.eventType];
        
            if (materialToUse != null)
            {
                if (Application.isPlaying)
                {
                    meshRenderer.material = materialToUse;
                }
                else
                {
                    meshRenderer.sharedMaterial = materialToUse;
                }
            }
        }
        // When there's no event or no material for the event, use default gray
        else
        {
            // Default material for sectors with no events
            if (Application.isPlaying)
            {
                // Create a new material instance for runtime
                Material defaultMaterial = new Material(Shader.Find("Sprites/Default"));
                defaultMaterial.color = Color.gray;
                meshRenderer.material = defaultMaterial;
            }
            else
            {
                // For editor time
                Material defaultMaterial = new Material(Shader.Find("Sprites/Default"));
                defaultMaterial.color = Color.gray;
                meshRenderer.sharedMaterial = defaultMaterial;
            }
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
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                // In edit mode, delay setting the sprite, to get rid of annoying warning
                EditorApplication.delayCall += () =>
                {
                    if (this != null && sectorIconRenderer != null)
                    {
                        sectorIconRenderer.sprite = (sectorEvent != null) ? sectorIcon : null;
                    }
                };
                return;
            }
#endif
            // In play mode, set the sprite directly
            sectorIconRenderer.sprite = (sectorEvent != null) ? sectorIcon : null;
        }
    }

    private void NotifyLane()
    {
        foreach (var lane in connectedLanes.Values)
        {
            lane.NotifySectorEventChanged();
        }
    }
    
    private IEnumerator PulsateEffect()
    {
        float pulseSpeed = 1.5f;
        float minScale = 0.4f;
        float maxScale = 0.8f;

        while (isPulsating)
        {
            float scale = Mathf.Lerp(minScale, maxScale, 0.5f * (1 + Mathf.Sin(Time.time * pulseSpeed * Mathf.PI)));
            transform.localScale = Vector3.one * scale;
            yield return null;
        }

        transform.localScale = Vector3.one * minScale;
    }
}