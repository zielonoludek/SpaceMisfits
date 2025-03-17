using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputActions inputActions;
    public static InputManager instance;
    public delegate void Swipe(Vector2 direction);

    private bool isDragging = false;
    public float pinchZoomSpeed = 0.01f;
    private float prevMagnitude = 0;
    private int touchCount = 0;

    private Vector2 initialPos = Vector2.zero;

    private void Awake()
    {
        inputActions = new InputActions();
        inputActions.Camera.Enable();

        inputActions.Camera.Zoom.performed += OnZoomScroll;

        inputActions.Camera.Drag.started += OnDrag;
        inputActions.Camera.Drag.performed += OnDrag;
        inputActions.Camera.Drag.canceled += OnDrag;

        SetupPinchZoom();
    }
    private void SetupPinchZoom()
    {
        var touch0contact = new InputAction(type: InputActionType.Button, binding: "<Touchscreen>/touch0/press");
        touch0contact.Enable();
        var touch1contact = new InputAction(type: InputActionType.Button, binding: "<Touchscreen>/touch1/press");
        touch1contact.Enable();

        touch0contact.performed += _ => touchCount++;
        touch1contact.performed += _ => touchCount++;
        touch0contact.canceled += _ => { touchCount--; prevMagnitude = 0; };
        touch1contact.canceled += _ => { touchCount--; prevMagnitude = 0; };

        var touch0pos = new InputAction(type: InputActionType.Value, binding: "<Touchscreen>/touch0/position");
        touch0pos.Enable();
        var touch1pos = new InputAction(type: InputActionType.Value, binding: "<Touchscreen>/touch1/position");
        touch1pos.Enable();

        touch1pos.performed += _ =>
        {
            if (touchCount < 2) return;
            var magnitude = (touch0pos.ReadValue<Vector2>() - touch1pos.ReadValue<Vector2>()).magnitude;
            if (prevMagnitude == 0)
                prevMagnitude = magnitude;
            var difference = magnitude - prevMagnitude;
            prevMagnitude = magnitude;
            ApplyPinchZoom(-difference * pinchZoomSpeed);
        };
    }


    //=============== INPUT CALLBACKS ===============//

    private void OnZoomScroll(InputAction.CallbackContext context)
    {
        if (context.ReadValue<Vector2>().y > 0) GameManager.Instance.CameraManager.ZoomInCamera();
        else if (context.ReadValue<Vector2>().y < 0) GameManager.Instance.CameraManager.ZoomOutCamera();
    }

    private void OnDrag(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            initialPos = GameManager.Instance.CameraManager.GetMousePosition;
            isDragging = true;
            return;
        }

        
        if (context.canceled)
        {
            isDragging = false;
        }
    }


    private void ApplyPinchZoom(float increment)
    {
        if (increment > 0)
            GameManager.Instance.CameraManager.ZoomOutCamera();
        else if (increment < 0)
            GameManager.Instance.CameraManager.ZoomInCamera();
    }
    private void LateUpdate()
    {
        if (!isDragging) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        Vector2 currentPos = GameManager.Instance.CameraManager.GetMousePosition;
        Vector2 diff = currentPos - initialPos; 
        
        GameManager.Instance.CameraManager.MoveCamera(diff.normalized);

    }
}