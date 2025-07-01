using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    // === Script references ===
    private BoxController boxControllerScript;
    private FaceInteractor faceInteractorScript;

    // === Input ===
    private PlayerInput playerInput;
    private InputAction interactAction;
    private InputAction goBackAction;

    // === Click detection ===
    [Header("Click Detection")]
    [SerializeField, Tooltip("Max duration (in seconds) to register a valid click")] private float clickThreshold;
    [SerializeField, Tooltip("Max movement (in pixels) allowed to consider as a click")] private float movementThreshold;
    private bool isClickHeld = false;
    private float clickHoldTime = 0f;
    private Vector2 initialPointerPosition;

    // === State ===
    private enum InteractionState { Rotating, Interacting };
    private InteractionState currentState = InteractionState.Rotating;
    private GameObject activeFace;

    void Awake()
    {
        boxControllerScript = GetComponent<BoxController>();
        faceInteractorScript = GetComponent<FaceInteractor>();

        playerInput = GetComponent<PlayerInput>();
        interactAction = playerInput.actions["Interact"];
        goBackAction = playerInput.actions["Go Back"];
    }

    void Update()
    {
        // Outline only if we are in rotation mode but not during a rotation
        bool shouldEnableOutline = !boxControllerScript.IsRotating && currentState == InteractionState.Rotating;
        faceInteractorScript.SetOutlineEnabled(shouldEnableOutline);

        switch (currentState)
        {
            case InteractionState.Rotating:
                ProcessClickInput();
                break;

            case InteractionState.Interacting:
                if (goBackAction.WasPressedThisFrame())
                {
                    SwitchToRotationMode();
                }
                break;
        }
    }

    // Verify whether the click input is for interacting or for rotating
    private void ProcessClickInput()
    {
        if (interactAction.WasPressedThisFrame())
        {
            isClickHeld = true;
            clickHoldTime = 0f;
            initialPointerPosition = Mouse.current.position.ReadValue();
        }

        if (!isClickHeld) return;

        clickHoldTime += Time.deltaTime;

        if (interactAction.WasReleasedThisFrame())
        {
            isClickHeld = false;

            Vector2 currentPointerPosition = Mouse.current.position.ReadValue();
            float movement = Vector2.Distance(currentPointerPosition, initialPointerPosition);

            if (clickHoldTime < clickThreshold && movement < movementThreshold)
            {
                SwitchToInteractionMode();
            }
        }
    }

    private void SwitchToInteractionMode()
    {
        // Don't allow to enter interaction mode if the box is still rotating
        if (boxControllerScript.IsRotating) return;

        activeFace = faceInteractorScript.CurrentFace;

        if (activeFace != null)
        {
            currentState = InteractionState.Interacting;
            boxControllerScript.CanRotate = false;

            // Activate the interactable content on the current face 
            if (activeFace.TryGetComponent<FaceContent>(out var faceContent))
            {
                faceContent.ActivateContent();
            }
        }
    }

    private void SwitchToRotationMode()
    {
        // Deactivate the interactable content when exiting interaction mode
        if (activeFace != null)
        {
            if (activeFace.TryGetComponent<FaceContent>(out var faceContent))
            {
                faceContent.DeactivateContent();
            }
        }
        
        currentState = InteractionState.Rotating;
        activeFace = null;
        boxControllerScript.CanRotate = true;
    }
}