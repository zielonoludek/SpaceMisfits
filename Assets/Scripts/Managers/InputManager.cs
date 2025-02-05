using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputActions inputActions;

    private float smoothTime = 0.1f;
    private Vector2 currentInput;       
    private Vector2 smoothInput;        
    private Vector2 smoothInputVelocity;

    private void Awake()
    {
        inputActions = new InputActions();
        DisableInputActions();

        inputActions.Camera.Enable();

        inputActions.Camera.Zoom.performed += OnZoomScroll;
        inputActions.Camera.Move.performed += OnMovePerformed;
        inputActions.Camera.Move.canceled += OnMoveCanceled;
    }

    private void DisableInputActions()
    {
        inputActions.Camera.Zoom.performed -= OnZoomScroll;
        inputActions.Camera.Move.performed -= OnMovePerformed;
        inputActions.Camera.Move.canceled -= OnMoveCanceled;

        inputActions.Camera.Disable();
    }

    //=============== INPUT CALLBACKS ===============//

    private void OnZoomScroll(InputAction.CallbackContext context)
    {
        if (context.ReadValue<Vector2>().y > 0) GameManager.Instance.CameraManager.ZoomInCamera();
        else if (context.ReadValue<Vector2>().y < 0) GameManager.Instance.CameraManager.ZoomOutCamera();
    }
    private void OnMovePerformed(InputAction.CallbackContext context) => currentInput = context.ReadValue<Vector2>();
    private void OnMoveCanceled(InputAction.CallbackContext context) => currentInput = Vector2.zero;

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