using System;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class Lane : MonoBehaviour
{
    [Header("Lane appearance")]
    
    [Tooltip("Number of points in the curve, the more the smoother")]
    [SerializeField][Range(3, 30)] private int curveResolution = 10;
    
    [Tooltip("Controls vertical curvature intensity")]
    [SerializeField ]private float curveHeight = 0f;
    
    [Tooltip("Controls horizontal curvature intensity")]
    [SerializeField] private float curveWidth = 0f;
    
    
    [Header("Lane settings")]
    
    [SerializeField][Range(1, 5)] private int laneSpeed = 1;
    
    private enum LaneType {Lane, SpiralLane, HyperLane}
    [SerializeField] private LaneType laneType = LaneType.Lane;
    
    // Connected sectors
    [SerializeField][HideInInspector] public Sector sectorA;
    [SerializeField][HideInInspector] public Sector sectorB;

    private LineRenderer lineRenderer;
    
    // Previous position of connected sectors
    private Vector3 lastPosA;
    private Vector3 lastPosB;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Update lane when values are changed in the inspector
        if (sectorA != null && sectorB != null)
        {
            UpdateLane();
            RegisterNeighbors();
        }
    }
#endif

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        RegisterNeighbors();
    }

    private void Update()
    {
        // If a sector is missing , destroy the lane
        if (sectorA == null || sectorB == null)
        {
            DestroyLine();
            return;
        }
        
        // Update lane position only if nodes have moved
        if (sectorA.transform.position != lastPosA || sectorB.transform.position != lastPosB)
        {
            UpdateLane();
        }
    }

    private void UpdateLane()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        Vector3 start = sectorA.transform.position;
        Vector3 end = sectorB.transform.position;

        // Midpoint for smooth curve
        Vector3 midPoint = (start + end) / 2;

        // Adjust control point based on curve height and curve width
        Vector3 control = midPoint + new Vector3(curveWidth, curveHeight, 0);

        // Generate Bezier curve points
        Vector3[] bezierPoints = CalculateBezierCurve(start, control, end);
        lineRenderer.positionCount = bezierPoints.Length;
        lineRenderer.SetPositions(bezierPoints);

        lastPosA = sectorA.transform.position;
        lastPosB = sectorB.transform.position;
        
        if (!Application.isPlaying)
        {
            EditorUtility.SetDirty(this);
        }
    }

    private void RegisterNeighbors()
    {
        if (sectorA != null && sectorB != null)
        {
            sectorA.AddNeighbor(sectorB);
            sectorB.AddNeighbor(sectorA);
        }
    }

    private Vector3[] CalculateBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        Vector3[] points = new Vector3[curveResolution];
        for (int i = 0; i < curveResolution; i++)
        {
            float t = i / (float)(curveResolution - 1);
            
            // Quadratic Bezier Formula
            points[i] = Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;
        }

        return points;
    }

    private void DestroyLine()
    {
        if (!Application.isPlaying)
        {
            Undo.DestroyObjectImmediate(gameObject);
        }
    }

    public void Initialize(Transform firstSector, Transform secondSector)
    {
        sectorA = firstSector.GetComponent<Sector>();
        sectorB = secondSector.GetComponent<Sector>();
        UpdateLane();
    }
}
