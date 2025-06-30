using UnityEngine;

public class BoxInteractionManager : MonoBehaviour
{
    /*
        Player presiona "Interactuar"
        → BoxInteractionManager verifica si hay una cara visible
        → Consulta a BoxFaceInteractor
            → Determina cuál es la cara mirando a la cámara
        → Si hay una cara válida:
        → BoxInteractionManager entra en modo interacción
            → Desactiva rotación
            → Activa contenido de esa cara (BoxFaceContent)
    */

    // === Script references ===
    private BoxController boxControllerScript;
    private BoxFaceInteractor faceInteractorScript;

    void Awake()
    {
        boxControllerScript = GetComponent<BoxController>();
        faceInteractorScript = GetComponent<BoxFaceInteractor>();
    }

    void Update()
    {
        // Disable outline during rotation
        faceInteractorScript.SetOutlineEnabled(!boxControllerScript.IsRotating);
    }
}
