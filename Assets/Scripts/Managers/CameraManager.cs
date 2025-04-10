using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] CinemachineCamera zoomCamera;
    [SerializeField] CinemachineCamera normalCamera;

    private bool zoomed = false;
    private static bool savedZoomState = false;

    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float damping = 15f;

    private Plane plane;

    private float speed;

    private Vector3 horizontalVelocity;
    private Vector3 lastPosition;
    private Vector3 startDrag;
    private Vector3 targetPosition;

    private void Start()
    {
        if (GameManager.Instance.GameScene == GameScene.Map)
        {
            zoomed = true;
        }
        else if (GameManager.Instance.GameScene == GameScene.Ship)
        {
            zoomed = false;
        }
        UpdateCameraPriority();
    }
    private void OnEnable()
    {
        lastPosition = this.transform.position;
        plane = new Plane(Vector3.forward, Vector3.zero);
    }
    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject() || !Application.isFocused) return;

        DragCamera();
        UpdateVelocity();
        UpdateBasePosition();

    }
    #region Getters
    public Vector3 GetMousePosition => Mouse.current.position.ReadValue();
    public Vector3 GetCameraPosition => transform.position;
    public CinemachineCamera currentCamera { get; set; }
    public bool ZoomState { get { return zoomed; } }
    #endregion

    #region Zoom
    public void ToggleCamera()
    {
        zoomed = !zoomed;
        UpdateCameraPriority();
    }

    public void ZoomInCamera()
    {
        zoomed = true;
        UpdateCameraPriority();
    }
    public void ZoomOutCamera()
    {
        zoomed = false;
        UpdateCameraPriority();
    }
    private void UpdateCameraPriority()
    {
        if (zoomed) zoomCamera.Priority = 20;
        else zoomCamera.Priority = 0;
    }
    public void SaveZoomState()
    {
        savedZoomState = zoomed;
    }

    public void RestoreZoomState(bool forceZoomed = false)
    {
        zoomed = forceZoomed || savedZoomState;
        UpdateCameraPriority();
    }
    #endregion

    #region Movement

    private void UpdateVelocity()
    {
        horizontalVelocity = (this.transform.position - lastPosition) / Time.deltaTime;
        horizontalVelocity.y = 0f;
        lastPosition = this.transform.position;
    }

    private void DragCamera()
    {
        if (!Mouse.current.rightButton.isPressed) return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (plane.Raycast(ray, out float distance))
        {
            if (Mouse.current.rightButton.wasPressedThisFrame) startDrag = ray.GetPoint(distance);
            else targetPosition += startDrag - ray.GetPoint(distance);
            Debug.Log("ray");
        }
    }

    private void UpdateBasePosition()
    {
        if (targetPosition.sqrMagnitude > 0.1f)
        {
            speed = Mathf.Lerp(speed, maxSpeed, Time.deltaTime * acceleration);
            transform.position += targetPosition * speed * Time.deltaTime;
        }
        else
        {
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Time.deltaTime * damping);
            transform.position += horizontalVelocity * Time.deltaTime;
        }

        targetPosition = Vector3.zero;
    }
    #endregion
}