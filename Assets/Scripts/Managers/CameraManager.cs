using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] CinemachineCamera zoomCamera;
    [SerializeField] CinemachineCamera normalCamera;

    [SerializeField] float movementSpeed = 10;
    [SerializeField] float zoomedMovementSpeed = 5;

    private bool zoomed = false;

    public CinemachineCamera currentCamera { get; set; }
    public bool ZoomState { get { return zoomed; } }

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

    public void MoveCamera(Vector2 direction)
    {
        Vector3 dir = new Vector3(direction.x, direction.y, 0) * Time.unscaledDeltaTime;
        
        if (zoomed) dir *= zoomedMovementSpeed;
        else dir *= movementSpeed;

        transform.position -= dir;
    }

    public Vector3 GetMousePosition => Mouse.current.position.ReadValue();
    public Vector3 GetCameraPosition => transform.position;

}