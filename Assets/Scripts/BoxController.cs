using UnityEngine;
using UnityEngine.InputSystem;

public class BoxController : MonoBehaviour
{
    // Input
    private InputDevice device;
    private PlayerInput playerInput;
    private InputAction rotateAction;
    private InputAction zoomInAction;
    private InputAction zoomOutAction;

    // Rotation movement
    [SerializeField] private float mouseSensitivityFactor;
    [SerializeField] private float minInputValue;
    private Vector2 rotationInput;

    // Rotation interpolation
    private Quaternion startRotation;
    private Quaternion targetRotation;
    private bool isRotating = false;
    private float rotationTimer = 0f;
    [SerializeField] private float rotationDuration;

    // Zoom
    private Camera mainCamera;
    private float zoomInInput;
    private float zoomOutInput;
    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;
    [SerializeField] private float zoomSpeed;

    void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        playerInput = GetComponent<PlayerInput>();
        rotateAction = playerInput.actions["Rotate"];
        zoomInAction = playerInput.actions["Zoom In"];
        zoomOutAction = playerInput.actions["Zoom Out"];
    }

    void Update()
    {
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
        rotationInput = rotateAction.ReadValue<Vector2>();
        device = rotateAction.activeControl?.device; // Detect the last device that sent input
        RotateBox();

        zoomInInput = zoomInAction.ReadValue<float>();
        zoomOutInput = zoomOutAction.ReadValue<float>();

        ZoomIn();
        ZoomOut();
    }

    // Rotation methods
    public void RotateBox()
    {
        // Save rotationInput in a new variable to process if a mouse is used
        Vector2 processedInput = rotationInput;
        if (device is Mouse)
        processedInput *= mouseSensitivityFactor; // Scale the mouse delta so that it behaves like a stick

        // Ignore very small inputs to avoid unwanted rotations
        if (processedInput.magnitude < minInputValue) return;

        Debug.Log(processedInput.magnitude);

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

    // Zoom methods
    private void ZoomIn()
    {
        if (zoomInInput != 0 && mainCamera.transform.position.z < maxZoom)
        {
            mainCamera.transform.Translate(0, 0, zoomSpeed * Time.deltaTime);
        }
    }

    private void ZoomOut()
    {
        if (zoomOutInput != 0 && mainCamera.transform.position.z > minZoom)
        {
            mainCamera.transform.Translate(0, 0, -zoomSpeed * Time.deltaTime);
        }
    }
}