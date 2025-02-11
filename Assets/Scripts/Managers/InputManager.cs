using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private InputAction position, press;
    [SerializeField] private float swipeResistance = 100;

    private InputActions inputActions;
    public static InputManager instance;
    public delegate void Swipe(Vector2 direction);
    private Vector2 currentPos => position.ReadValue<Vector2>();

    private float smoothTime = 0.1f;
    public float pinchZoomSpeed = 0.01f;
    private float prevMagnitude = 0;
    private int touchCount = 0;

    private Vector2 initialPos;
    private Vector2 currentInput;       
    private Vector2 smoothInput;        
    private Vector2 smoothInputVelocity;

    private void Awake()
    {
        inputActions = new InputActions();

        inputActions.Camera.Enable();

        inputActions.Camera.Zoom.performed += OnZoomScroll;
        inputActions.Camera.Move.performed += OnMovePerformed;
        inputActions.Camera.Move.canceled += OnMoveCanceled;

        SetupPinchZoom();
        SetupSwipe();
    }

    private void SetupSwipe()
    {
        position.Enable();
        press.Enable();
        press.performed += _ => { initialPos = currentPos; };
        press.canceled += _ => DetectSwipe();
        instance = this;
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
    private void OnMovePerformed(InputAction.CallbackContext context) => currentInput = context.ReadValue<Vector2>();
    private void OnMoveCanceled(InputAction.CallbackContext context) => currentInput = Vector2.zero;

    private void ApplyPinchZoom(float increment)
    {
        if (increment > 0)
            GameManager.Instance.CameraManager.ZoomOutCamera();
        else if (increment < 0)
            GameManager.Instance.CameraManager.ZoomInCamera();
    }
    private void DetectSwipe()
    {
        Vector2 delta = currentPos - initialPos;
        Vector2 direction = Vector2.zero;

        if (Mathf.Abs(delta.x) > swipeResistance)
        {
            direction.x = Mathf.Clamp(delta.x, -1, 1);
        }
        if (Mathf.Abs(delta.y) > swipeResistance)
        {
            direction.y = Mathf.Clamp(delta.y, -1, 1);
        }
        if (direction != Vector2.zero)
            GameManager.Instance.CameraManager.MoveCamera(direction);
    }

    void Update()
    {
        smoothInput = Vector2.SmoothDamp(
            smoothInput,        
            currentInput,        
            ref smoothInputVelocity,
            smoothTime           
        );
        GameManager.Instance.CameraManager.MoveCamera(smoothInput);
    }
}