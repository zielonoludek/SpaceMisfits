using System;
using TMPro;
using UnityEngine;

public class SectorInteraction : MonoBehaviour
{
    private HoverUI hoverUI;
    private Vector3 originalScale;
    private Sector sector;

    private void Awake()
    {
        sector = GetComponent<Sector>();
        hoverUI = FindFirstObjectByType<HoverUI>();
        originalScale = transform.localScale;
    }

    private void OnMouseEnter()
    {
        if(GameManager.Instance.GameState == GameState.Event) return;
        if (hoverUI != null)
        {
            if (sector.GetSectorEvent() != null)
            {
                hoverUI.ShowPopup(sector.GetSectorEvent().sectorHoverInfoText, transform.position);
            }
            else
            {
                hoverUI.ShowPopup("", transform.position);
            }
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
        if(GameManager.Instance.GameState == GameState.Event) return;
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