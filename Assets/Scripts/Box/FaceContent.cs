using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FaceContent : MonoBehaviour
{
    // === Input ===
    private PlayerInput playerInput;
    private InputAction navigateAction;
    private InputAction interactAction;

    // === Interactable object management ===
    private List<GameObject> interactables = new();
    private GameObject currentInteractable;
    private bool isActive = false;

    // === Navigation (stick) ===
    [Header("Stick Navigation")]
    [SerializeField] private float navigationCooldown;
    private float navigationTimer = 0f;

    // === Navigation (mouse) ===
    [Header("Mouse Navigation")]
    [SerializeField] private LayerMask interactableLayer;
    private Camera mainCamera;

    // === Getter ===
    public bool IsActive => isActive;

    void Awake()
    {
        // Search for interactables objects on the face and add them to the list
        Transform interactableContainer = transform.Find("Interactables");
        if (interactableContainer != null)
        {
            foreach (Transform child in interactableContainer.transform)
            {
                interactables.Add(child.gameObject);
            }
        }

        if (interactables.Count > 0)
        {
            currentInteractable = interactables[0];
        }

        mainCamera = Camera.main;

        playerInput = GetComponentInParent<PlayerInput>();
        navigateAction = playerInput.actions["Navigate"];
        interactAction = playerInput.actions["Interact"];
    }

    void Update()
    {
        if (!isActive) return;

        // For stick
        navigationTimer += Time.deltaTime;
        Vector2 navigateInput = navigateAction.ReadValue<Vector2>();

        // Only processed if sufficient time has passed since the last navigation
        if (navigationTimer >= navigationCooldown && navigateInput.magnitude > 0.5f)
        {
            NavigateWithStick(navigateInput);
            navigationTimer = 0f;
        }

        // For mouse
        NavigationWithMouse();

        // Detect if an interactive object is pressed
        if (interactAction.WasPressedThisFrame())
        {
            TryInteractWithCurrent();
        }
    }

    public void ActivateContent()
    {
        if (interactables.Count == 0) return;
        isActive = true;
        Highlight(currentInteractable);
    }

    public void DeactivateContent()
    {
        isActive = false;
        RemoveHighlight(currentInteractable);
    }

    private void Highlight(GameObject obj)
    {
        if (obj != null && obj.TryGetComponent<Outline>(out var outline))
        {
            outline.enabled = true;
        }
    }

    private void RemoveHighlight(GameObject obj)
    {
        if (obj != null && obj.TryGetComponent<Outline>(out var outline))
        {
            outline.enabled = false;
        }
    }

    // Navigates to the nearest interacting object in the direction of the stick
    private void NavigateWithStick(Vector2 input)
    {
        if (input == Vector2.zero || currentInteractable == null) return;

        Vector3 currentPos = currentInteractable.transform.position;

        // Determine dominant axis (horizontal or vertical)
        Vector3 axis;
        float directionSign;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            axis = Vector3.right;
            directionSign = Mathf.Sign(input.x);
        }
        else
        {
            axis = Vector3.up;
            directionSign = Mathf.Sign(input.y);
        }

        GameObject closestSelectable = null;
        float shortestDistance = float.MaxValue;

        // Search for the nearest object in the desired direction
        foreach (var obj in interactables)
        {
            if (obj == currentInteractable) continue;

            Vector3 toCandidate = obj.transform.position - currentPos;

            // Projection on the principal axis (X or Y)
            float axisProjection = Vector3.Dot(axis, toCandidate.normalized);

            // Filtering if the object is not sufficiently aligned with the desired direction
            if (axisProjection * directionSign < 0.5f) continue;

            float distance = toCandidate.sqrMagnitude;

            if (distance < shortestDistance)
            {
                closestSelectable = obj;
                shortestDistance = distance;
            }
        }

        // If a valid object was found, the highlight is updated
        if (closestSelectable != null)
        {
            RemoveHighlight(currentInteractable);
            currentInteractable = closestSelectable;
            Highlight(currentInteractable);
        }
    }

    // Can navigate between interacting objects by hovering the mouse over them
    private void NavigationWithMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 20f, interactableLayer))
        {
            GameObject hovered = hit.collider.gameObject;

            // Only change if it's different from the current one
            if (hovered != currentInteractable && interactables.Contains(hovered))
            {
                RemoveHighlight(currentInteractable);
                currentInteractable = hovered;
                Highlight(currentInteractable);
            }
        }
    }

    // Interacts with the currently selected object if it implements the IInteractable interface
    private void TryInteractWithCurrent()
    {
        if (currentInteractable == null) return;

        if (currentInteractable.TryGetComponent<IInteractable>(out var interactable))
        {
            interactable.Interact();
        }
        else
        {
            Debug.LogWarning($"{currentInteractable.name} no implementa IInteractable");
        }
    }
}