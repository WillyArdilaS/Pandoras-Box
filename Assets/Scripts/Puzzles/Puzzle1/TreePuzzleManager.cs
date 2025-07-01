using System.Collections.Generic;
using UnityEngine;

public class TreePuzzleManager : MonoBehaviour
{
    [Header("Botones del puzzle en orden")]
    [SerializeField] private List<TreeButton> buttonList = new();
    private bool[,] dependencyMatrix; // Adjacency matrix: each row represents a button and its impact on the others - [i][j] = true if button i affects button j

    void Awake()
    {

        // Validar que hay botones
        if (buttonList.Count == 0)
        {
            Debug.LogError("No se asignaron botones en TreePuzzleManager");
            return;
        }

        // Initialize adjacency matrix
        dependencyMatrix = new bool[buttonList.Count, buttonList.Count];

        // Establishing dependencies according to the puzzle design
        SetDependency(0, 0); // B1 → B1
        SetDependency(0, 1); // B1 → B2
        SetDependency(0, 4); // B1 → B5

        SetDependency(1, 0); // B2 → B1
        SetDependency(1, 1); // B2 → B2
        SetDependency(1, 2); // B2 → B3

        SetDependency(2, 1); // B3 → B2
        SetDependency(2, 2); // B3 → B3
        SetDependency(2, 3); // B3 → B4

        SetDependency(3, 2); // B4 → B3
        SetDependency(3, 3); // B4 → B4
        SetDependency(3, 4); // B4 → B5

        SetDependency(4, 0); // B5 → B1
        SetDependency(4, 3); // B5 → B4
        SetDependency(4, 4); // B5 → B5
    }

    // Method to mark a dependency between buttons
    private void SetDependency(int buttonIndex, int affectedIndex)
    {
        if (buttonIndex < buttonList.Count && affectedIndex < buttonList.Count)
        {
            dependencyMatrix[buttonIndex, affectedIndex] = true;
        }
    }

    // This method is called by the TreeButton when interacting with it
    public void OnButtonPressed(TreeButton sourceButton)
    {
        int sourceIndex = buttonList.IndexOf(sourceButton);

        if (sourceIndex == -1)
        {
            Debug.LogWarning("El botón no está registrado en el TreePuzzleManager");
            return;
        }

        for (int i = 0; i < buttonList.Count; i++)
        {
            if (dependencyMatrix[sourceIndex, i])
            {
                buttonList[i].Rotate();
            }
        }

        CheckVictoryCondition();
    }

    private void CheckVictoryCondition()
    {
        foreach (var button in buttonList)
        {
            if (button.CurrentState != 1)
            {
                return; // There is at least one button not activated
            }
        }

        OnPuzzleSolved(); // All buttons are in state 1
    }

    private void OnPuzzleSolved()
    {
        Debug.Log("¡Puzzle resuelto!");
        // Aquí más adelante podrías bloquear los botones, mostrar efectos, etc.
        //Notificar al sistema si el puzzle fue resuelto
    }
}