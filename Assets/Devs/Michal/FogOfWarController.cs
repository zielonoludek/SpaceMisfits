using UnityEngine;

public class FogOfWarController : MonoBehaviour
{
    [SerializeField] private Material fogOfWarMaterial;
    [SerializeField] private float circleRadius = 5f;
    [SerializeField] private Color fogColor = new Color(1f, 0f, 0f, 0.7f);
    
    [SerializeField] private enum PlaneAxis { XZ = 0, YZ = 1, XY = 2 }
    [SerializeField] private PlaneAxis planeAxis = PlaneAxis.XY;
    
    private void Start()
    {
        if (fogOfWarMaterial != null)
        {
            fogOfWarMaterial.SetFloat("_CircleRadius", circleRadius);
            fogOfWarMaterial.SetColor("_FogColor", fogColor);
            fogOfWarMaterial.SetInt("_PlaneAxis", (int)planeAxis);
        }
    }
    
    public void UpdatePlayerPosition(Vector3 position)
    {
        if (fogOfWarMaterial != null)
        {
            fogOfWarMaterial.SetVector("_PlayerPos", position);
        }
    }
}
