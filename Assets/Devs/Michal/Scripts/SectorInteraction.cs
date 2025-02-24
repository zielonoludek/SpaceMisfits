using System;
using UnityEngine;

public class SectorInteraction : MonoBehaviour
{
    private Vector3 originalScale;
    private Sector sector;

    private void Awake()
    {
        sector = GetComponent<Sector>();
        originalScale = transform.localScale;
    }

    private void OnMouseEnter()
    {
        if (CanBeInteracted())
        {
            transform.localScale = originalScale * 1.2f;
        }
    }

    private void OnMouseExit()
    {
        transform.localScale = originalScale;
    }

    private void OnMouseDown()
    {
        if (CanBeInteracted())
        {
            SectorManager.MovePlayerToSector(sector);
        }
    }

    private bool CanBeInteracted()
    {
        if (SectorManager.GetPlayerCurrentSector() == null) return false;
        return SectorManager.GetPlayerCurrentSector().IsNeighbor(sector) && !SectorManager.IsPlayerMoving();
    }
}
