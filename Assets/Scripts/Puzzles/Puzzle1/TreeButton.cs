using UnityEngine;

public class TreeButton : MonoBehaviour, IInteractable
{
    // === Script references ===
    private TreePuzzleManager treePuzzleManagerScript;

    // === State ===
    private int currentState = 0; // 0: Horizontal - 1: Vertical

    // === Action ===
    private Transform movablePart;

    // === Leaf lights ===
    [SerializeField] private GameObject leafReference;
    [SerializeField] private Material leafMaterial;

    // === Getter ===
    public int CurrentState => currentState;

    void Awake()
    {
        // Search TreePuzzleManager in common parent
        treePuzzleManagerScript = GetComponentInParent<FaceContent>()?.GetComponentInChildren<TreePuzzleManager>();

        if (treePuzzleManagerScript == null)
        {
            Debug.LogError("No se encontró TreePuzzleManager en la cara correspondiente");
        }

        movablePart = transform.Find("levelTop");

        leafMaterial = leafReference.GetComponent<Renderer>().sharedMaterial;
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
        movablePart.Rotate(0f, 90f, 0f);
    }

    public void ToggleLight()
    {
        if (currentState == 1)
        {
            leafMaterial.EnableKeyword(("_EMISSION"));
            Color lightColor = leafMaterial.GetColor("_EmissionColor");
            leafMaterial.SetColor("_EmissionColor", lightColor);
        }
        else
        {
            leafMaterial.DisableKeyword("_EMISSION");
        }
    }
}