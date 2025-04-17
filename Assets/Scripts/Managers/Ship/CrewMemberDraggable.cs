using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrewMemberDraggable : MonoBehaviour
{
    public CrewmateData crewmateData;
    public ShipPartManager shipPartManager;

    [SerializeField] private InputActions inputActions;

    private Vector3 currentScreenPosition;
    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 offset;
    private Renderer rend;
    private Transform initialParent;
    private GameObject currentTrigger;
    private Transform previousSnapPoint;
    private CrewDropZone previousDropZone;
    private static CrewMemberDraggable currentlyDragging = null;

    private Vector3 WorldPosition
    {
        get
        {
            float z = mainCamera.WorldToScreenPoint(transform.position).z;
            return mainCamera.ScreenToWorldPoint(currentScreenPosition + new Vector3(0, 0, z));
        }
    }

    private bool IsCrewClickedOn
    {
        get
        {
            if (GameManager.IsGameOver) return false;
            Ray ray = mainCamera.ScreenPointToRay(currentScreenPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                return hit.transform == transform;
            }
            return false;
        }
    }

    private void Awake()
    {
        mainCamera = Camera.main;
        rend = GetComponent<Renderer>();
        initialParent = transform.parent;

        inputActions = new InputActions();
        inputActions.CrewDrag.Press.Enable();
        inputActions.CrewDrag.ScreenPosition.Enable();
        inputActions.CrewDrag.ScreenPosition.performed += context => { currentScreenPosition = context.ReadValue<Vector2>(); };
        inputActions.CrewDrag.Press.performed += _ => { if (IsCrewClickedOn) StartCoroutine(CrewDrag()); };
        inputActions.CrewDrag.Press.canceled += _ => { EndDrag(); };
    }

    private IEnumerator CrewDrag()
    {
        if (currentlyDragging != null || isDragging) yield break;

        isDragging = true;
        currentlyDragging = this;

        offset = transform.position - WorldPosition;
        Cursor.visible = false;
        HighlightValidDropZones();

        while (isDragging)
        {
            transform.position = WorldPosition + offset;
            yield return null;
        }
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
            RevertToPreviousPosition();
        }

        Debug.Log("Drag ended");
    }

    private void SnapToTrigger()
    {
        CrewDropZone dropZone = currentTrigger?.GetComponent<CrewDropZone>();
        if (dropZone == null)
        {
            RevertToPreviousPosition();
            return;
        }

        Collider zoneCollider = dropZone.GetComponent<Collider>();
        if (zoneCollider != null && !zoneCollider.bounds.Contains(transform.position))
        {
            RevertToPreviousPosition();
            return;
        }

        if (dropZone.IsAvailable())
        {
            Transform snapPoint = dropZone.isCrewQuarters
                ? dropZone.GetNextAvailableSnapPoint()
                : (dropZone.snapPoints.Count > 0 ? dropZone.snapPoints[0] : null);

            if (snapPoint != null)
            {
                if (previousDropZone != null && previousSnapPoint != null)
                {
                    previousDropZone.RemoveCrew(crewmateData, previousSnapPoint);
                    if (previousDropZone.shipPart != null)
                    {
                        shipPartManager.RemoveCrewmateFromPart(crewmateData, previousDropZone.shipPart);
                        previousDropZone.shipPart.RemoveEffect();
                    }
                }

                PlaceInZone(dropZone, snapPoint);
            }
            else
            {
                RevertToPreviousPosition();
            }
        }
        else
        {
            CrewMemberDraggable swapTarget = GetSwapTarget(dropZone);
            if (swapTarget != null)
            {
                DoSwap(swapTarget);
            }
            else
            {
                RevertToPreviousPosition();
            }
        }
    }

    private CrewMemberDraggable GetSwapTarget(CrewDropZone dropZone)
    {
        if (currentTrigger == null || currentTrigger.GetComponent<CrewDropZone>() != dropZone)
        {
            Debug.Log("Current trigger does not match drop zone.");
            return null;
        }

        Transform closestSnapPoint = null;
        float minDistance = float.MaxValue;

        foreach (var snapPoint in dropZone.snapPoints)
        {
            float dist = Vector3.Distance(transform.position, snapPoint.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestSnapPoint = snapPoint;
            }
        }

        Debug.Log($"[World] Closest snap point: {closestSnapPoint?.name}, Distance: {minDistance}");

        if (closestSnapPoint == null || minDistance > 5.0f)
        {
            Debug.Log("Not close enough to a snap point.");
            return null;
        }

        CrewMemberDraggable[] crewMembers = dropZone.GetComponentsInChildren<CrewMemberDraggable>();

        foreach (var crew in crewMembers)
        {
            if (crew == this) continue;

            if (crew.previousSnapPoint == closestSnapPoint)
            {
                Debug.Log($"Found crew to swap: {crew.name}");
                return crew;
            }

            float snapDist = Vector3.Distance(crew.transform.position, closestSnapPoint.position);
            if (snapDist < 0.3f)
            {
                Debug.Log($"Found close crew to swap by distance: {crew.name}, dist: {snapDist}");
                return crew;
            }
        }

        Debug.Log("No swap target found at snap point.");
        return null;
    }

    private void PlaceInZone(CrewDropZone zone, Transform snapPoint, bool isSwap = false)
    {
        transform.position = snapPoint.position;
        transform.SetParent(zone.transform);
        previousDropZone = zone;
        previousSnapPoint = snapPoint;
        currentTrigger = zone.gameObject;

        if (!isSwap)
        {
            shipPartManager.AssignCrewmateToPart(crewmateData, zone.shipPart);
            zone.AssignCrew(crewmateData);
        }

        zone.ResetZoneColor();
    }

    private void DoSwap(CrewMemberDraggable target)
    {
        CrewDropZone originZone = this.previousDropZone;
        Transform originSnap = this.previousSnapPoint;
        CrewDropZone targetZone = target.previousDropZone;
        Transform targetSnap = target.previousSnapPoint;

        if (originZone == null || targetZone == null)
        {
            Debug.LogError("Swap failed: Missing drop zone reference.");
            RevertToPreviousPosition();
            return;
        }

        if (originZone.shipPart != null)
        {
            shipPartManager.RemoveCrewmateFromPart(this.crewmateData, originZone.shipPart);
            originZone.shipPart.RemoveEffect();
        }

        if (targetZone.shipPart != null)
        {
            shipPartManager.RemoveCrewmateFromPart(target.crewmateData, targetZone.shipPart);
            targetZone.shipPart.RemoveEffect();
        }

        this.PlaceInZone(targetZone, targetSnap, true);
        target.PlaceInZone(originZone, originSnap, true);

        if (targetZone.shipPart != null)
        {
            shipPartManager.AssignCrewmateToPart(this.crewmateData, targetZone.shipPart);
        }

        if (originZone.shipPart != null)
        {
            shipPartManager.AssignCrewmateToPart(target.crewmateData, originZone.shipPart);
        }

        Debug.Log("Swapped and reassigned crew members between drop zones.");
    }

    private void RevertToPreviousPosition()
    {
        if (previousSnapPoint != null && previousDropZone != null)
        {
            transform.position = previousSnapPoint.position;
            transform.SetParent(previousDropZone.transform);
        }
        else
        {
            transform.position = initialParent.position;
            transform.SetParent(initialParent);
        }

        Debug.Log("Reverted to previous position");
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

    public void UpdateDropZoneReferences(Transform newParent, Transform assignedSnapPoint)
    {
        initialParent = newParent;
        CrewDropZone dropZone = newParent.GetComponent<CrewDropZone>();
        if (dropZone != null)
        {
            previousDropZone = dropZone;
            previousSnapPoint = assignedSnapPoint;
            currentTrigger = dropZone.gameObject;
        }
    }

    private void HighlightValidDropZones()
    {
        CrewDropZone[] dropZones = FindObjectsByType<CrewDropZone>(FindObjectsSortMode.None);

        foreach (var dropZone in dropZones)
        {
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