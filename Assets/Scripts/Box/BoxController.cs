using UnityEngine;
using UnityEngine.InputSystem;

public class BoxController : MonoBehaviour
{
    // === Input ===
    private InputDevice device;
    private PlayerInput playerInput;
    private InputAction rotateAction;
    private InputAction zoomInAction;
    private InputAction zoomOutAction;

    // === Rotation movement ===
    [Header("Rotation")]
    [SerializeField] private float mouseSensitivityFactor;
    [SerializeField] private float minRotationInput;
    private Vector2 rotationInput;

    // === Rotation interpolation ===
    [SerializeField] private float rotationDuration;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    private bool isRotating = false;
    private float rotationTimer = 0f;

    // === Zoom ===
    [Header("Zoom")]
    [SerializeField] private float scrollSensitivityFactor;
    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;
    [SerializeField] private float zoomSpeed;
    private Camera mainCamera;
    private float zoomInInput;
    private float zoomOutInput;

    // === Getters ===
    public bool IsRotating => isRotating;

    void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        playerInput = GetComponent<PlayerInput>();
        rotateAction = playerInput.actions["Rotate"];
        zoomInAction = playerInput.actions["Zoom In"];
        zoomOutAction = playerInput.actions["Zoom Out"];
    }

    void Update()
    {
        // Smooth transition between rotations
        if (isRotating)
        {
            rotationTimer += Time.deltaTime / rotationDuration;
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, rotationTimer);

            if (rotationTimer >= 1f) // End of interpolation
            {
                transform.rotation = targetRotation;
                isRotating = false;
            }

            return;
        }

        // Reading inputs
        device = GetActiveDevice();

        rotationInput = rotateAction.ReadValue<Vector2>();
        RotateBox();

        zoomInInput = zoomInAction.ReadValue<float>();
        zoomOutInput = zoomOutAction.ReadValue<float>();

        HandleZoom(zoomInInput, maxZoom, 1);
        HandleZoom(zoomOutInput, minZoom, -1);
    }

    // Detect the last input type received
    private InputDevice GetActiveDevice()
    {
        if (rotateAction.activeControl != null) return rotateAction.activeControl.device;
        if (zoomInAction.activeControl != null) return zoomInAction.activeControl.device;
        if (zoomOutAction.activeControl != null) return zoomOutAction.activeControl.device;
        return null;
    }

    // Rotation methods
    public void RotateBox()
    {
        // Save rotationInput in a new variable to process if a mouse is used
        Vector2 processedInput = rotationInput;
        if (device is Mouse)
        {
            processedInput *= mouseSensitivityFactor; // Scale the mouse delta to behave as a control stick
        }

        // Ignore very small inputs to avoid unwanted rotations
        if (processedInput.magnitude < minRotationInput) return;

        // Validate the dominant axis of rotation
        float absX = Mathf.Abs(processedInput.x);
        float absY = Mathf.Abs(processedInput.y);

        if (absX > absY)
        {
            RotateHorizontally(Mathf.Sign(processedInput.x));
        }
        else if (absY > absX)
        {
            RotateVertically(Mathf.Sign(processedInput.y));
        }
    }

    private void RotateHorizontally(float direction)
    {
        startRotation = transform.rotation;
        targetRotation = Quaternion.AngleAxis(90f * direction, Vector3.up) * transform.rotation; // Using the global Y axis
        BeginLerp();
    }

    private void RotateVertically(float direction)
    {
        startRotation = transform.rotation;
        targetRotation = Quaternion.AngleAxis(90f * direction, Vector3.left) * transform.rotation; // Using the global X axis
        BeginLerp();
    }

    private void BeginLerp()
    {
        rotationTimer = 0f;
        isRotating = true;
    }

    // Zoom method
    private void HandleZoom(float input, float limit, int direction)
    {
        if (input == 0) return;

        float totalZoom = zoomSpeed * Time.deltaTime * direction;

        // Scale mouse scrolling to behave as a control trigger
        if (device is Mouse)
        {
            totalZoom *= scrollSensitivityFactor;
        }

        // Validate the limits
        float newZPositionCamera = mainCamera.transform.position.z + totalZoom;
        if ((direction > 0 && newZPositionCamera >= limit) || (direction < 0 && newZPositionCamera <= limit)) return;

        mainCamera.transform.Translate(0, 0, totalZoom);
    }
}