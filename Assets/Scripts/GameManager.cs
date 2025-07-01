using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // === UI ===
    [SerializeField] private GameObject puzzleCompletedUI;

    public void ShowPuzzleCompletedUI()
    {
        puzzleCompletedUI.SetActive(true);
        Time.timeScale = 0;
    }
}
