using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float movementSpeed = 10f;        // Speed of movement with WASD
    public float fastMovementMultiplier = 2f; // Multiplier for faster movement (when Shift is held)

    [Header("Rotation Settings")]
    public float mouseSensitivity = 2f;     // Mouse sensitivity for rotation
    public bool invertMouseY = false;       // Invert mouse Y-axis

    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;            // Speed of zooming
    public float minZoom = 10f;             // Minimum distance for zoom
    public float maxZoom = 100f;            // Maximum distance for zoom

    private float currentZoom = 50f;        // Current zoom level
    private Vector3 moveDirection;          // Direction for WASD movement
    private float yaw;                      // Horizontal rotation (mouse X-axis)
    private float pitch;                    // Vertical rotation (mouse Y-axis)
    private bool isMouseLocked = false;     // Tracks if the camera is locked

    void Start()
    {
        // Lock the cursor to the game window and make it invisible initially
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize yaw and pitch with current rotation
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void Update()
    {
        HandleLockToggle();  // Check for toggle input
        HandleMovement();    // Move the camera with WASD
        if (!isMouseLocked)  // Only allow rotation if the camera is unlocked
        {
            HandleRotation();
        }
        HandleZoom();         // Adjust zoom level
    }

    void HandleMovement()
    {
        // Get WASD input
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down Arrow

        // Determine movement direction
        moveDirection = (transform.right * moveX + transform.forward * moveZ).normalized;

        // Check if Shift is held for faster movement
        float speed = Input.GetKey(KeyCode.LeftShift) ? movementSpeed * fastMovementMultiplier : movementSpeed;

        // Apply movement
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    void HandleRotation()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Update yaw and pitch
        yaw += mouseX;
        pitch -= (invertMouseY ? -mouseY : mouseY);

        // Clamp the pitch to avoid extreme angles
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        // Apply rotation
        transform.eulerAngles = new Vector3(pitch, yaw, 0f);
    }

    void HandleZoom()
    {
        // Get mouse scroll input
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // Update zoom level
        currentZoom -= scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // Adjust camera's field of view or position
        Camera.main.fieldOfView = currentZoom;
    }

    void HandleLockToggle()
    {
        // Toggle camera lock when F is pressed
        if (Input.GetKeyDown(KeyCode.F))
        {
            isMouseLocked = !isMouseLocked;

            if (isMouseLocked)
            {
                // Unlock the cursor and make it visible
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                // Lock the cursor and hide it
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            Debug.Log($"Camera lock toggled. Is mouse locked: {isMouseLocked}");
        }
    }
}
