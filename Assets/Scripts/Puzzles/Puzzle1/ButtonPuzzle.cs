using UnityEngine;

public class ButtonPuzzle : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log($"¡{gameObject.name} presionado!");
        // Lógica del botón
    }
}
