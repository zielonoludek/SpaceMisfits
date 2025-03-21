using UnityEngine;

public class CrewMemberDraggable : MonoBehaviour
{
    public CrewmateData crewmateData;
    private Vector3 offset;
    private bool isDragging = false;
    private Renderer rend;
    private Vector3 initialPosition;
    private Transform initialParent;
    private GameObject currentTrigger;
    public ShipPartManager shipPartManager;
    private static CrewMemberDraggable currentlyDragging = null;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        initialPosition = transform.position;
        initialParent = transform.parent;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            TryStartDrag();

            if (isDragging)
            {
                Drag();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
    }

    private void TryStartDrag()
    {
        if (isDragging || CrewMemberDraggable.currentlyDragging != null) return;

        RaycastHit hit = CastRay();

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            offset = transform.position - GetMouseWorldPos();
            isDragging = true;
            currentlyDragging = this;
            Cursor.visible = false;

            HighlightValidDropZones();
            Debug.Log("Drag started");
        }
    }

    private void Drag()
    {
        Vector3 mouseWorldPos = GetMouseWorldPos();
        transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, initialPosition.z) + offset;
    }

    private void EndDrag()
    {
        if (!isDragging) return;

        isDragging = false;
        currentlyDragging = null;
        Cursor.visible = true;

        ResetAllDropZoneHighlights();

        if (currentTrigger != null)
        {
            SnapToTrigger();
        }
        else
        {
            ReturnToInitialPosition();
        }

        Debug.Log("Drag ended");
    }

    private void SnapToTrigger()
    {
        CrewDropZone dropZone = currentTrigger != null ? currentTrigger.GetComponent<CrewDropZone>() : null;

        if (dropZone == null || !dropZone.IsAvailable())
        {
            Debug.Log("Invalid or unavailable drop zone. Returning to initial position.");
            ReturnToInitialPosition();
            return;
        }

        Transform snapPoint = dropZone.isCrewQuarters
            ? dropZone.GetNextAvailableSnapPoint()
            : (dropZone.snapPoints.Count > 0 ? dropZone.snapPoints[0] : null);

        if (snapPoint != null)
        {
            transform.position = snapPoint.position;
            transform.SetParent(dropZone.transform);

            if (!dropZone.isCrewQuarters)
            {
                shipPartManager.RemoveCrewmateFromPart(crewmateData, shipPartManager.GetCrewQuartersPart());
            }

            dropZone.AssignCrew(crewmateData);
            shipPartManager.AssignCrewmateToPart(crewmateData, dropZone.shipPart);

            dropZone.ResetZoneColor();
            Debug.Log($"{crewmateData.crewmateName} assigned to {dropZone.shipPart?.partName ?? "Crew Quarters"}");
        }
        else
        {
            Debug.Log("No available snap points. Returning to initial position.");
            ReturnToInitialPosition();
        }
    }

    private void ReturnToInitialPosition()
    {
        transform.position = initialPosition;
        transform.SetParent(initialParent);
        Debug.Log("Invalid drop zone. Returning to Crew Quarters.");
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 screenMousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z);
        return Camera.main.ScreenToWorldPoint(screenMousePos);
    }

    private RaycastHit CastRay()
    {
        Vector3 screenMousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
        Vector3 screenMousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
        Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);
        RaycastHit hit;
        int layerMask = ~LayerMask.GetMask("Ignore Raycast");
        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit, Mathf.Infinity, layerMask);
        return hit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentlyDragging != this) return;

        if (other.CompareTag("DropZone"))
        {
            currentTrigger = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentlyDragging != this) return;

        if (currentTrigger != null && other.gameObject == currentTrigger)
        {
            currentTrigger = null;
        }
        HighlightValidDropZones();
    }

    private void HighlightValidDropZones()
    {
        CrewDropZone[] dropZones = FindObjectsByType<CrewDropZone>(FindObjectsSortMode.None);

        foreach (var dropZone in dropZones)
        {
            if (dropZone.isCrewQuarters) continue;

            if (dropZone.IsAvailable())
            {
                dropZone.HighlightZone(Color.green);
            }
            else
            {
                dropZone.HighlightZone(Color.red);
            }
        }
    }

    private void ResetAllDropZoneHighlights()
    {
        CrewDropZone[] dropZones = FindObjectsByType<CrewDropZone>(FindObjectsSortMode.None);

        foreach (var dropZone in dropZones)
        {
            dropZone.ResetZoneColor();
        }
    }

}
