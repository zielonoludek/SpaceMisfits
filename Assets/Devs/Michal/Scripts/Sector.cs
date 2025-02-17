using System.Collections.Generic;
using UnityEngine;

public class Sector : MonoBehaviour
{
    private HashSet<Sector> neighbors = new HashSet<Sector>();
    
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
