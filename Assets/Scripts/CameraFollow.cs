
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The player or object the camera follows
    public Vector3 offset = new Vector3(0f, 2f, -5f); // Offset from the target
    public float smoothSpeed = 0.125f; // Smoothness of the camera movement

    public float mouseSensitivity = 10f; // Sensitivity of mouse movement
    public float pitchMin = -30f; // Minimum pitch (up/down angle)
    public float pitchMax = 60f; // Maximum pitch (up/down angle)

    private float yaw = 0f; // Horizontal rotation (left/right)
    private float pitch = 0f; // Vertical rotation (up/down)

    private Vector2 mouseDelta; // Stores mouse movement input

    public bool isPaused = false; // Flag to pause camera movement

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.actions.FindActionMap("Player").Enable();
        }
    }

    void OnEnable()
    {
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            var lookAction = playerInput.actions["Look"];
            if (lookAction != null)
            {
                lookAction.performed += OnLook;
                lookAction.canceled += OnLook;
                lookAction.Enable();
            }
        }
    }

    void OnDisable()
    {
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            var lookAction = playerInput.actions["Look"];
            if (lookAction != null)
            {
                lookAction.performed -= OnLook;
                lookAction.canceled -= OnLook;
                lookAction.Disable();
            }
        }
    }

    // Input System callback for mouse movement
    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.performed && !isPaused)
        {
            mouseDelta = context.ReadValue<Vector2>();
        }
        else if (context.canceled || isPaused)
        {
            mouseDelta = Vector2.zero;
        }
    }

    public void PauseCamera()
    {
        isPaused = true;
        mouseDelta = Vector2.zero; // Reset mouse input to avoid unintended movement
    }

    public void ResumeCamera()
    {
        Debug.Log("resume camera called!");
        isPaused = false;
        mouseDelta = Vector2.zero; 
    /*var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            var lookAction = playerInput.actions["Look"];
            if (lookAction != null && !lookAction.enabled)
            {
                lookAction.Enable();
            }
        }
        */
    }

    void LateUpdate()
    {
        if (isPaused || target == null) return;

        // Get mouse input from the Input System
        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        // Update yaw and pitch based on mouse input
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax); // Clamp pitch to prevent flipping

        // Calculate the rotation based on yaw and pitch
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Calculate the desired position of the camera
        Vector3 desiredPosition = target.position + rotation * offset;

        // Smoothly move the camera to the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Make the camera look at the target
        transform.LookAt(target);
    }
}
