using System.Collections;
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
        CrewDropZone dropZone = currentTrigger.GetComponent<CrewDropZone>();

        if (dropZone == null || !dropZone.IsAvailable())
        {
            RevertToPreviousPosition();
            return;
        }

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

            transform.position = snapPoint.position;
            transform.SetParent(dropZone.transform);
            previousSnapPoint = snapPoint;
            previousDropZone = dropZone;

            shipPartManager.AssignCrewmateToPart(crewmateData, dropZone.shipPart);
            dropZone.AssignCrew(crewmateData);

            dropZone.ResetZoneColor();
        }
        else
        {
            RevertToPreviousPosition();
        }
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

        //Debug.Log("Reverted to previous position");
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