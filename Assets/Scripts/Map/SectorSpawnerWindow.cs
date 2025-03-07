using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SectorSpawnerWindow : EditorWindow
{
    private GameObject sectorPrefab;
    private GameObject lanePrefab;
    private SectorEventSO sectorEvent;
    
    [MenuItem("Tools/Sector Spawner")]
    public static void ShowWindow()
    {
        GetWindow<SectorSpawnerWindow>("Sector Spawner");
    }

    private void OnGUI()
    {
        LoadPrefabs();
        
        GUILayout.Label("Sector Spawner", EditorStyles.boldLabel);
        
        EditorGUILayout.ObjectField("Sector Prefab", sectorPrefab, typeof(GameObject), false);
        
        // Field to assign Sector event
        sectorEvent = EditorGUILayout.ObjectField("Select Sector Event", sectorEvent, typeof(SectorEventSO), false) as SectorEventSO; 
        
        // Button to spawn sector
        if (GUILayout.Button("Spawn Sector") && sectorPrefab != null)
        {
            SpawnSector();
        }
        
        EditorGUILayout.Space();
        
        GUILayout.Label("Create Lane", EditorStyles.boldLabel);
        
        EditorGUILayout.ObjectField("Lane Prefab", lanePrefab, typeof(GameObject), false);
        
        // Button to create a lane between selected sectors
        if (GUILayout.Button("Create Lane") && lanePrefab != null)
        {
            CreateLane();
        }
    }

    private void LoadPrefabs()
    {
        sectorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Map/Sector.prefab");
        lanePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Map/Lane.prefab");
    }

    private void SpawnSector()
    {
        // Ensure map and sectors parent game objects exist
        Transform mapParent = GetOrCreateParent("Map");
        Transform sectorsParent = GetOrCreateParent("Sectors", mapParent);
        
        // Instantiate sector prefab
        GameObject newSector = PrefabUtility.InstantiatePrefab(sectorPrefab) as GameObject;
        if (newSector != null)
        {
            newSector.transform.position = Vector3.zero;
            newSector.transform.SetParent(sectorsParent);

            Sector sectorScript = newSector.GetComponent<Sector>();
            if (sectorScript != null)
            {
                sectorScript.SetSectorEvent(sectorEvent);
            }
            
            Undo.RegisterCreatedObjectUndo(newSector, "Spawn Sector");
            Selection.activeGameObject = newSector;
        }
    }

    private void CreateLane()
    {
        // Get two selected sectors
        var selectedSectors = Selection.gameObjects.Where(obj => obj.CompareTag("Sector")).ToArray();
        if (selectedSectors.Length != 2)
        {
            Debug.LogWarning("Please select exactly two sectors!");
            return;
        }

        Sector sectorA = selectedSectors[0].GetComponent<Sector>();
        Sector sectorB = selectedSectors[1].GetComponent<Sector>();

        if (LaneExists(sectorA, sectorB))
        {
            Debug.LogWarning($"A lane already exists between {sectorA.name} and {sectorB.name}");
            return;
        }
        
        // Endure map and lanes parent game object exist
        Transform mapParent = GetOrCreateParent("Map");
        Transform lanesParent = GetOrCreateParent("Lanes", mapParent);
        
        // Instantiate the Lane prefab
        GameObject lane = PrefabUtility.InstantiatePrefab(lanePrefab) as GameObject;
        if (lane != null)
        {
            lane.transform.SetParent(lanesParent);
            
            // Extract sector names and format the lane name
            string sector1Name = selectedSectors[0].name.Replace("Sector (", "").Replace(")", "").Trim();
            string sector2Name = selectedSectors[1].name.Replace("Sector (", "").Replace(")", "").Trim();
            lane.name = $"Lane ({sector1Name} - {sector2Name})";
            
            // Initialize the lane script with the selected sectors
            Lane laneScript = lane.GetComponent<Lane>();
            if (laneScript != null)
            {
                laneScript.Initialize(selectedSectors[0].transform, selectedSectors[1].transform);
            }
            
            Undo.RegisterCreatedObjectUndo(lane, "Create Lane");
        }
    }

    // Helper function to get or create parent game object in hierarchy
    private Transform GetOrCreateParent(string parentName, Transform parent = null)
    {
        GameObject parentObj = GameObject.Find(parentName);
        if (parentObj == null)
        {
            parentObj = new GameObject(parentName);
            if (parent != null)
            {
                parentObj.transform.SetParent(parent);
            }
            Undo.RegisterCreatedObjectUndo(parentObj, "Create Parent Group");
        }

        return parentObj.transform;
    }

    // Function checking of lane between two selected sectors already exists or not
    private bool LaneExists(Sector sectorA, Sector sectorB)
    {
        Lane[] lanes = FindObjectsByType<Lane>(FindObjectsSortMode.None);
        foreach (Lane lane in lanes)
        {
            if ((lane.GetSectorA() == sectorA && lane.GetSectorB() == sectorB) ||
                (lane.GetSectorA() == sectorB && lane.GetSectorB() == sectorA))
            {
                return true;
            }
        }

        return false;
    }
}
