using System;
using UnityEditor;
using UnityEngine;

public class Lane : MonoBehaviour
{
    [Header("Lane appearance")]
    
    [Tooltip("Number of points in the curve, the more the smoother")]
    [SerializeField][Range(3, 30)] private int curveResolution = 10;
    
    [Tooltip("Controls vertical curvature intensity")]
    [SerializeField ]private float curveHeight = 0f;
    
    [Tooltip("Controls horizontal curvature intensity")]
    [SerializeField] private float curveWidth = 0f;
    
    public Sector sectorA;
    public Sector sectorB;

    private LineRenderer lineRenderer;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Update lane when values are changed in the inspector
        if (sectorA != null && sectorB != null)
        {
            UpdateLane();
        }
    }
#endif

    private void UpdateLane()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        Vector3 start = sectorA.transform.position;
        Vector3 end = sectorB.transform.position;

        Vector3 midPoint = (start + end) / 2;

        Vector3 control = midPoint + new Vector3(curveWidth, curveHeight, 0);

        Vector3[] bezierPoints = CalculateBezierCurve(start, control, end);
        lineRenderer.positionCount = bezierPoints.Length;
        lineRenderer.SetPositions(bezierPoints);
        
        if (!Application.isPlaying)
        {
            EditorUtility.SetDirty(this);
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

    public void Initialize(Transform firstSector, Transform secondSector)
    {
        sectorA = firstSector.GetComponent<Sector>();
        sectorB = secondSector.GetComponent<Sector>();
        UpdateLane();
    }
}
