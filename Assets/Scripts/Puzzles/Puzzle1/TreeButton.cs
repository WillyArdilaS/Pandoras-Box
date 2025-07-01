using UnityEngine;

public class TreeButton : MonoBehaviour, IInteractable
{
    // === Script references ===
    private TreePuzzleManager treePuzzleManagerScript;

    // === State ===
    private int currentState = 0; // 0: Horizontal - 1: Vertical

    // === Getter ===
    public int CurrentState => currentState;

    void Awake()
    {
        // Buscar el TreePuzzleManager en el padre común (FrontFace)
        treePuzzleManagerScript = GetComponentInParent<FaceContent>()?.GetComponentInChildren<TreePuzzleManager>();

        if (treePuzzleManagerScript == null)
        {
            Debug.LogError("No se encontró TreePuzzleManager en la cara correspondiente");
        }
    }

    public void Interact()
    {
        if (treePuzzleManagerScript != null)
        {
            treePuzzleManagerScript.OnButtonPressed(this);
        }
        else
        {
            Debug.LogWarning($"TreeButton en {gameObject.name} no tiene asignado un TreePuzzleManager");
        }
    }

    public void Rotate()
    {
        currentState = 1 - currentState; // Toggle state
        transform.Rotate(0f, 0f, 90f);
    }
}