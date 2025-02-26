using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] CinemachineCamera zoomCamera;
    [SerializeField] CinemachineCamera normalCamera;

    [SerializeField] float movementSpeed;

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
        transform.position -= new Vector3(direction.x, 0, direction.y) * movementSpeed * Time.deltaTime / GameManager.Instance.TimeManager.TimeSpeed;
    }

    public Vector3 GetMousePosition => Mouse.current.position.ReadValue();
    public Vector3 GetCameraPosition => transform.position;
}