using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Sector : MonoBehaviour
{
    [SerializeField] private bool isStartingSector;
    private static Sector currentStartingSector;

    public static Sector GetCurentStartingSector => currentStartingSector;
    
    private HashSet<Sector> neighbors = new HashSet<Sector>();

#if UNITY_EDITOR
    private void OnValidate()
    {
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
    
    public void AddNeighbor(Sector sector)
    {
        if (sector != null && !neighbors.Contains(sector))
        {
            neighbors.Add(sector);
        }
    }

    public bool IsNeighbor(Sector sector)
    {
        return neighbors.Contains(sector);
    }
}
