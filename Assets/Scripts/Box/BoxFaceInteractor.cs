using UnityEngine;

public class BoxFaceInteractor : MonoBehaviour
{
    // === Face tracking ===
    [SerializeField] private LayerMask faceLayer;
    private Camera mainCamera;
    private GameObject currentFace;

    // === Highlighting ===
    private GameObject lastHighlightedFace;
    private bool isOutlineEnabled = true;

    // === Getters ===
    public GameObject CurrentFace => currentFace;

    void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        if (!isOutlineEnabled) return;

        DetectFaceUnderCamera();
    }

    private void DetectFaceUnderCamera()
    {
        // Raycast from center of the screen to detect the front face of the box
        Ray cameraRay = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Debug.DrawRay(cameraRay.origin, cameraRay.direction * 20f, Color.red);

        // Detect if the ray hits a collider on the layer corresponding to the faces of the box
        if (Physics.Raycast(cameraRay, out RaycastHit hit, 20f, faceLayer))
        {
            currentFace = hit.collider.gameObject;

            HighlightFace(currentFace);
        }
    }

    private void HighlightFace(GameObject newFace)
    {
        // Skip if the same face is already highlighted
        if (lastHighlightedFace == newFace) return;

        // Disable outline on previous face
        if (lastHighlightedFace != null)
        {
            lastHighlightedFace.GetComponent<Outline>().enabled = false;
        }

        // Enable outline on new face
        newFace.GetComponent<Outline>().enabled = true;
        lastHighlightedFace = newFace;
    }

    // Manage outline state
    public void SetOutlineEnabled(bool enabledState)
    {
        isOutlineEnabled = enabledState;

        if (!enabledState && lastHighlightedFace != null)
        {
            lastHighlightedFace.GetComponent<Outline>().enabled = false;
        }
    }
}