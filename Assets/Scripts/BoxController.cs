using UnityEngine;
using UnityEngine.InputSystem;

public class BoxController : MonoBehaviour
{
    // Variables
    private InputDevice device;
    private PlayerInput playerInput;
    private InputAction rotateAction;
    private InputAction zoomInAction;
    private InputAction zoomOutAction;

    private Vector2 rotationInput;
    private Vector2 accumulatedInput;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rotationTreshold;
    [SerializeField] private float mouseSensitivityFactor;

    private GameObject mainCamera;
    private float zoomInInput;
    private float zoomOutInput;
    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;
    [SerializeField] private float zoomSpeed;

    void Awake()
    {
        mainCamera = GameObject.Find("Main Camera");
        playerInput = GetComponent<PlayerInput>();
        rotateAction = playerInput.actions["Rotate"];
        zoomInAction = playerInput.actions["Zoom In"];
        zoomOutAction = playerInput.actions["Zoom Out"];
    }

    void Update()
    {
        rotationInput = rotateAction.ReadValue<Vector2>();

        // Detect the last device that sent input
        device = rotateAction.activeControl?.device;

        RotateBox();

        zoomInInput = zoomInAction.ReadValue<float>();
        zoomOutInput = zoomOutAction.ReadValue<float>();

        ZoomIn();
        ZoomOut();
    }

    // Rotation
    public void RotateBox()
    {
        if (rotationInput != Vector2.zero)
        {
            Vector2 processedInput = rotationInput;

            // Accumulate the value of the input to check when it should rotate
            accumulatedInput += processedInput;

            // Normalize and scale the mouse delta so that it behaves like a stick
            if (device is Mouse)
            {
                processedInput = rotationInput.normalized * mouseSensitivityFactor;
            }

            // Validate the dominant axis of rotation
            float absX = Mathf.Abs(processedInput.x);
            float absY = Mathf.Abs(processedInput.y);

            //if (absX > absY && Mathf.Abs(accumulatedInput.x) > rotationTreshold)
            if (absX > absY)
            {
                RotateHorizontally(processedInput);
            }
            //else if (absY > absX && Mathf.Abs(accumulatedInput.y) > rotationTreshold)
            else if (absX < absY)
            {
                RotateVertically(processedInput);
            }
        }
        else
        {
            accumulatedInput = Vector2.zero; 
        }
    }

    private void RotateHorizontally(Vector2 processedInput)
    {
        transform.Rotate(Vector3.up, processedInput.x * rotationSpeed * Time.deltaTime);

        /*if (processedInput.x > 0)
        {
            transform.Rotate(Vector3.up, 90);
        }
        else
        {
            transform.Rotate(Vector3.up, -90);
        }

        accumulatedInput = Vector2.zero; */
    }

    private void RotateVertically(Vector2 processedInput)
    {
        transform.Rotate(Vector3.right, processedInput.y * rotationSpeed * Time.deltaTime);

        /*if (processedInput.y > 0)
        {
            if (Mathf.Abs(transform.rotation.y) == 90)
            {
                transform.Rotate(Vector3.forward, 90);
            }
            else
            {
                transform.Rotate(Vector3.right, 90);
            }
        }
        else
        {
            if (Mathf.Abs(transform.rotation.y) == 90)
            {
                transform.Rotate(Vector3.forward, -90);
            }
            else
            {
                transform.Rotate(Vector3.right, -90);
            }
        }
        
        accumulatedInput = Vector2.zero; */
    }

    // Zoom
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
