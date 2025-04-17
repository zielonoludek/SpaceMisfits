using System.Collections.Generic;
using UnityEngine;

public class FogOfWarController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Material fogMaterial;
    [SerializeField] private float visibilityRadius = 5f;
    [SerializeField, Range(0, 1)] private float fadeEdgeSize = 0.2f;
    
    [Header("Quest Settings")]
    [SerializeField] private float questVisibilityRadius = 2.5f;
    
    private GameObject fogQuad;
    private Transform playerTransform;
    private Camera fogRenderCamera;
    
    // Render texture that stores the explored areas
    private RenderTexture exploredAreasTexture;
    private RenderTexture temporaryTexture;
    
    private List<Sector> nextQuestSectors = new List<Sector>();
    
    private static readonly int PlayerPosID = Shader.PropertyToID("_PlayerPos");
    private static readonly int VisibilityRadiusID = Shader.PropertyToID("_VisibilityRadius");
    private static readonly int QuestRadiusID = Shader.PropertyToID("_QuestRadius");
    private static readonly int QuestPositionsID = Shader.PropertyToID("_QuestPositions");
    private static readonly int QuestCountID = Shader.PropertyToID("_QuestCount");
    private static readonly int ExploredTexID = Shader.PropertyToID("_ExploredTex");
    private static readonly int FadeEdgeSizeID = Shader.PropertyToID("_FadeEdgeSize");
    
    private void Start()
    {
        // Create render texture for storing explored areas
        exploredAreasTexture = new RenderTexture(1024, 1024, 16, RenderTextureFormat.R16);
        exploredAreasTexture.Create();
        
        // Create a temporary texture for updating the explored areas
        temporaryTexture = new RenderTexture(1024, 1024, 16, RenderTextureFormat.R16);
        temporaryTexture.Create();
        
        // Set up the fog material
        fogMaterial.SetTexture(ExploredTexID, exploredAreasTexture);
        fogMaterial.SetFloat(VisibilityRadiusID, visibilityRadius);
        fogMaterial.SetFloat(FadeEdgeSizeID, fadeEdgeSize);
        fogMaterial.SetFloat(QuestRadiusID, questVisibilityRadius);
        
        if (fogQuad == null)
        {
            CreateFogQuad();
        }
        
        SetupRenderCamera();
    }
    
    private void Update()
    {
        if (playerTransform == null) return;
        
        // Update player position in the shader
        fogMaterial.SetVector(PlayerPosID, new Vector4(playerTransform.position.x, playerTransform.position.y, 0, 0));
        
        UpdateExploredAreas();
    }
    
    public void RegisterNextQuestSector(Sector sector)
    {
        if (sector != null && !nextQuestSectors.Contains(sector))
        {
            nextQuestSectors.Add(sector);
        }
    }
    
    public void UnregisterNextQuestSector(Sector sector)
    {
        if (sector != null)
        {
            nextQuestSectors.Remove(sector);
        }
    }
    
    public void ClearNextQuestSectors()
    {
        nextQuestSectors.Clear();
    }
    
    private void UpdateExploredAreas()
    {
        // Set the render target to the temporary texture
        fogRenderCamera.targetTexture = temporaryTexture;
        
        fogRenderCamera.backgroundColor = Color.black;
        fogRenderCamera.clearFlags = CameraClearFlags.SolidColor;
        
        // Render the current explored areas
        Graphics.Blit(exploredAreasTexture, temporaryTexture);
        
        // Setup material for drawing the visible circles
        Material drawMaterial = new Material(Shader.Find("Hidden/DrawVisibleCircles"));
        
        // Set player position and visibility radius
        drawMaterial.SetVector(PlayerPosID, new Vector4(playerTransform.position.x, playerTransform.position.y, 0, 0));
        drawMaterial.SetFloat(VisibilityRadiusID, visibilityRadius);
        drawMaterial.SetFloat(QuestRadiusID, questVisibilityRadius);
        
        Vector4[] questPositions = new Vector4[10];
        int questCount = Mathf.Min(nextQuestSectors.Count, 10);
        
        for (int i = 0; i < questCount; i++)
        {
            Vector3 pos = nextQuestSectors[i].transform.position;
            questPositions[i] = new Vector4(pos.x, pos.y, 0, 0);
        }
        
        drawMaterial.SetVectorArray(QuestPositionsID, questPositions);
        drawMaterial.SetInt(QuestCountID, questCount);
        
        // Draw the new visible areas
        Graphics.Blit(temporaryTexture, exploredAreasTexture, drawMaterial);
        
        // Update the texture in the fog material
        fogMaterial.SetTexture(ExploredTexID, exploredAreasTexture);
        
        fogMaterial.SetVectorArray(QuestPositionsID, questPositions);
        fogMaterial.SetInt(QuestCountID, questCount);
    }
    
    private void CreateFogQuad()
    {
        fogQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        fogQuad.name = "FogOfWarQuad";
        fogQuad.transform.SetParent(transform);
        fogQuad.layer = 2;
        
        fogQuad.transform.localScale = new Vector3(100, 100, 1);
        fogQuad.transform.position = new Vector3(0, 0, -1);
        
        // Apply the fog material
        fogQuad.GetComponent<Renderer>().material = fogMaterial;
    }
    
    private void SetupRenderCamera()
    {
        if (fogRenderCamera == null)
        {
            GameObject camObj = new GameObject("FogRenderCamera");
            camObj.transform.SetParent(transform);
            
            fogRenderCamera = camObj.AddComponent<Camera>();
            fogRenderCamera.orthographic = true;
            
            fogRenderCamera.orthographicSize = 50;
            fogRenderCamera.transform.position = new Vector3(0, 0, -10);
            fogRenderCamera.clearFlags = CameraClearFlags.SolidColor;
            fogRenderCamera.backgroundColor = Color.black;
            fogRenderCamera.cullingMask = 0;
            fogRenderCamera.enabled = false;
        }
    }
    
    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
    }
    
    public void SetVisibilityRadius(float radius)
    {
        visibilityRadius = radius;
        fogMaterial.SetFloat(VisibilityRadiusID, visibilityRadius);
    }
    
    // Check if position is visible (not covered by FOW)
    public bool IsPositionVisible(Vector2 worldPosition)
    {
        if (playerTransform == null) return false;
        
        // Check player visibility
        float distToPlayer = Vector2.Distance(worldPosition, new Vector2(playerTransform.position.x, playerTransform.position.y));
        if (distToPlayer < visibilityRadius) return true;
        
        // Check quest visibility
        foreach (var sector in nextQuestSectors)
        {
            Vector2 sectorPos = new Vector2(sector.transform.position.x, sector.transform.position.y);
            float distToQuest = Vector2.Distance(worldPosition, sectorPos);
            if (distToQuest < questVisibilityRadius) return true;
        }
        
        return false;
    }
    
    // Check if a position has been explored
    public bool IsPositionExplored(Vector2 worldPosition)
    {
        if (IsPositionVisible(worldPosition)) return true;
        
        Vector2 texUV = new Vector2((worldPosition.x + 50) / 100, (worldPosition.y + 50) / 100);
        
        if (texUV.x < 0 || texUV.x > 1 || texUV.y < 0 || texUV.y > 1) return false;
        
        Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        
        // Save active render texture and set to explored areas texture
        RenderTexture previousActive = RenderTexture.active;
        RenderTexture.active = exploredAreasTexture;
        
        int x = Mathf.FloorToInt(texUV.x * exploredAreasTexture.width);
        int y = Mathf.FloorToInt(texUV.y * exploredAreasTexture.height);
        tex.ReadPixels(new Rect(x, y, 1, 1), 0, 0);
        tex.Apply();
        
        // Restore previous render texture
        RenderTexture.active = previousActive;
        
        Color pixelColor = tex.GetPixel(0, 0);
        Destroy(tex);
        
        return pixelColor.r > 0.1f;
    }
    
    private void OnDestroy()
    {
        if (exploredAreasTexture != null)
        {
            exploredAreasTexture.Release();
            Destroy(exploredAreasTexture);
        }
        
        if (temporaryTexture != null)
        {
            temporaryTexture.Release();
            Destroy(temporaryTexture);
        }
    }
}