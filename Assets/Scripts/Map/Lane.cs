
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class Lane : MonoBehaviour
{
    // =============== LANE APPEARANCE ===============
    [Header("Lane appearance")]
    
    [Tooltip("Number of points in the curve, the more the smoother")]
    [SerializeField][Range(3, 30)] private int curveResolution = 10;
    
    [Tooltip("Controls vertical curvature intensity")]
    [SerializeField ]private float curveHeight = 0f;
    
    [Tooltip("Controls horizontal curvature intensity")]
    [SerializeField] private float curveWidth = 0f;
    
    
    // =============== LANE SETTINGS ===============
    [Header("Lane settings")]
    
    [Tooltip("Controls the length of the lane, modifying the time it takes to travel between sectors, the more the longer the distance")]
    [SerializeField][Range(1, 5)] private int laneDistance = 1;
    //[SerializeField] private LaneType laneType = LaneType.Lane;
    public int GetLaneDistance() => 6 - laneDistance;
    private enum LaneType {Lane, SpiralLane, HyperLane}
    
    
    // Connected sectors
    [SerializeField][HideInInspector] public Sector sectorA;
    [SerializeField][HideInInspector] public Sector sectorB;

    public Sector GetSectorA() => sectorA;
    public Sector GetSectorB() => sectorB;

    private LineRenderer lineRenderer;
    private Transform textTransform;
    private TextMeshPro sectorDistanceText;
    
    // Previous position of connected sectors
    private Vector3 lastPosA;
    private Vector3 lastPosB;
    
    
    
    #region Public functions

    public void Initialize(Transform firstSector, Transform secondSector)
        {
            sectorA = firstSector.GetComponent<Sector>();
            sectorB = secondSector.GetComponent<Sector>();
            UpdateLane();
            UpdateLaneName();
        }
    
        public void NotifySectorEventChanged()
        {
            UpdateLaneName();
        }
        
        public Vector3[] GetLanePath()
        {
            if (sectorA == null || sectorB == null) return new Vector3[0];
    
            Vector3 start = sectorA.transform.position;
            Vector3 end = sectorB.transform.position;
            Vector3 midPoint = (start + end) / 2;
            Vector3 control = midPoint + new Vector3(curveWidth, curveHeight, 0);
    
            return CalculateBezierCurve(start, control, end);
        }
    
        public void SetVisibility(bool isVisible)
        {
            if (Application.isPlaying)
            {
                gameObject.SetActive(isVisible);
            }
        }

    #endregion
    
    
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Update lane when values are changed in the inspector
        if (sectorA != null && sectorB != null)
        {
            UpdateLane();
            RegisterNeighbors();
            UpdateLaneName();
        }
        EditorApplication.delayCall += () =>
        {
            if (this != null)
            {
                FindOrCreateText();
                UpdateTextDisplay();
                EditorUtility.SetDirty(this);
            }
        };
    }
#endif

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetVisibility(false);
    }

    private void Start()
    {
        RegisterNeighbors();
        UpdateLaneName();
    }

    private void Update()
    {
        // If a sector is missing , destroy the lane
        if (sectorA == null || sectorB == null)
        {
            DestroyLine();
            return;
        }
        
        // Update lane position only if sectors have moved
        if (sectorA.transform.position != lastPosA || sectorB.transform.position != lastPosB)
        {
            UpdateLane();
        }
        
        UpdateTextPosition();
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
            sectorA.AddNeighbor(sectorB, this);
            sectorB.AddNeighbor(sectorA, this);
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

    // Updates the lane name to match the connected sectors events
    private void UpdateLaneName()
    {
        if (sectorA == null || sectorB == null) return;

        string sectorAName = sectorA.GetSectorEvent() != null ? sectorA.GetSectorEvent().eventTitle : "Unnamed";
        string sectorBName = sectorB.GetSectorEvent() != null ? sectorB.GetSectorEvent().eventTitle : "Unnamed";

        gameObject.name = $"Lane ({sectorAName} - {sectorBName})";
    }

    private void FindOrCreateText()
    {
        textTransform = transform.Find("LaneText");

        if (textTransform != null)
        {
            sectorDistanceText = textTransform.GetComponent<TextMeshPro>();
        }
    }
    
    private void UpdateTextPosition()
    {
        if (textTransform == null) return;

        Vector3 start = sectorA.transform.position;
        Vector3 end = sectorB.transform.position;
        Vector3 midPoint = (start + end) / 2;
        textTransform.position = midPoint;
    }
    
    private void UpdateTextDisplay()
    {
        if (sectorDistanceText == null) return;
        
        sectorDistanceText.text = laneDistance.ToString();
    }
}
