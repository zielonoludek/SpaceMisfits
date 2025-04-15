using UnityEngine;

public class FogOfWarController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Material fogMaterial;
    [SerializeField] private float visibilityRadius = 5f;
    [SerializeField, Range(0, 1)] private float fadeEdgeSize = 0.2f;
    
    private GameObject fogQuad;
    private Transform playerTransform;
    private Camera fogRenderCamera;
    
    // Render texture that stores the explored areas
    private RenderTexture exploredAreasTexture;
    private RenderTexture temporaryTexture;
    
    void Start()
    {
        // Create render texture for storing explored areas
        exploredAreasTexture = new RenderTexture(1024, 1024, 0, RenderTextureFormat.R8);
        exploredAreasTexture.Create();
        
        // Create a temporary texture for updating the explored areas
        temporaryTexture = new RenderTexture(1024, 1024, 0, RenderTextureFormat.R8);
        temporaryTexture.Create();
        
        // Set up the fog material
        fogMaterial.SetTexture("_ExploredTex", exploredAreasTexture);
        fogMaterial.SetFloat("_VisibilityRadius", visibilityRadius);
        fogMaterial.SetFloat("_FadeEdgeSize", fadeEdgeSize);
        
        if (fogQuad == null)
        {
            CreateFogQuad();
        }
        
        SetupRenderCamera();
    }
    
    void Update()
    {
        if (playerTransform == null) return;
        
        // Update player position in the shader
        fogMaterial.SetVector("_PlayerPos", new Vector4(playerTransform.position.x, playerTransform.position.y, 0, 0));
        
        // Update explored areas
        UpdateExploredAreas();
    }
    
    void UpdateExploredAreas()
    {
        // Set the render target to the temporary texture
        fogRenderCamera.targetTexture = temporaryTexture;
        
        // Clear with black (unexplored)
        fogRenderCamera.backgroundColor = Color.black;
        fogRenderCamera.clearFlags = CameraClearFlags.SolidColor;
        
        // Render the current explored areas
        Graphics.Blit(exploredAreasTexture, temporaryTexture);
        
        // Setup material for drawing the current visible circle
        Material drawMaterial = new Material(Shader.Find("Hidden/DrawVisibleCircle"));
        drawMaterial.SetVector("_PlayerPos", new Vector4(playerTransform.position.x, playerTransform.position.y, 0, 0));
        drawMaterial.SetFloat("_VisibilityRadius", visibilityRadius);
        
        // Draw the new visible area
        Graphics.Blit(temporaryTexture, exploredAreasTexture, drawMaterial);
        
        // Update the texture in the fog material
        fogMaterial.SetTexture("_ExploredTex", exploredAreasTexture);
    }
    
    void CreateFogQuad()
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
    
    void SetupRenderCamera()
    {
        if (fogRenderCamera == null)
        {
            GameObject camObj = new GameObject("FogRenderCamera");
            camObj.transform.SetParent(transform);
            
            fogRenderCamera = camObj.AddComponent<Camera>();
            fogRenderCamera.orthographic = true;
            
            // Match the camera size to your game world
            fogRenderCamera.orthographicSize = 50;
            fogRenderCamera.transform.position = new Vector3(0, 0, -10);
            fogRenderCamera.clearFlags = CameraClearFlags.SolidColor;
            fogRenderCamera.backgroundColor = Color.black;
            fogRenderCamera.cullingMask = 0;
        }
    }
    
    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
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