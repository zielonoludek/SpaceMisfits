using System;
using UnityEngine;

public class SectorInteraction : MonoBehaviour
{
    private HoverUI hoverUI;
    private Vector3 originalScale;
    [SerializeField] private Sector sector;

    private void Awake()
    {
        sector = GetComponent<Sector>();
        hoverUI = FindFirstObjectByType<HoverUI>();
        originalScale = transform.localScale;
    }

    private void OnMouseEnter()
    {
        if (hoverUI != null)
        {
            string eventType = sector.GetSectorEvent() != null ? sector.GetSectorEvent().eventType.ToString() : "No Event";
            // hoverUI.ShowPopup(eventType, transform.position);
        }
        
        if (CanBeInteracted())
        {
            transform.localScale = originalScale * 1.2f;
        }
    }

    private void OnMouseExit()
    {
        transform.localScale = originalScale;

        if (hoverUI != null)
        {
            hoverUI.HidePopup();
        }
    }

    private void OnMouseDown()
    {
        if (CanBeInteracted())
        {
            SectorManager.Instance.MovePlayerToSector(sector);
        }
    }

    private bool CanBeInteracted()
    {
        if (SectorManager.Instance.GetPlayerCurrentSector() == null) return false;
        return SectorManager.Instance.GetPlayerCurrentSector().IsNeighbor(sector) && !SectorManager.Instance.IsPlayerMoving();
    }
}