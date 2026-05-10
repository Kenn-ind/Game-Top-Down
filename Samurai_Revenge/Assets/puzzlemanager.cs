using UnityEngine;
using System;

/// <summary>
/// Otak utama puzzle. Taruh di GameObject kosong bernama "PuzzleManager" di scene.
/// </summary>
public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    // Event didengarkan oleh StairController dan komponen lain
    public static event Action OnAllButtonsPressed;
    public static event Action OnPuzzleReset;

    [Header("Puzzle Settings")]
    [Tooltip("Jumlah tombol yang harus diinjak untuk membuka tangga")]
    [SerializeField] private int totalButtons = 2;

    private int pressedCount = 0;
    private bool puzzleSolved = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Dipanggil oleh PressurePlate saat player menginjak tombol.
    /// </summary>
    public void RegisterButtonPressed()
    {
        if (puzzleSolved) return;

        pressedCount++;
        Debug.Log($"[PuzzleManager] Tombol ditekan: {pressedCount}/{totalButtons}");

        if (pressedCount >= totalButtons)
        {
            puzzleSolved = true;
            Debug.Log("[PuzzleManager] Semua tombol aktif! Membuka tangga...");
            OnAllButtonsPressed?.Invoke();
        }
    }

    /// <summary>
    /// Dipanggil oleh RespawnManager saat player mati. Reset seluruh puzzle.
    /// </summary>
    public void ResetPuzzle()
    {
        pressedCount = 0;
        puzzleSolved = false;
        Debug.Log("[PuzzleManager] Puzzle di-reset.");
        OnPuzzleReset?.Invoke();
    }

    public bool IsSolved() => puzzleSolved;
}