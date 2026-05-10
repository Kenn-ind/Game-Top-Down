using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Taruh script ini di setiap GameObject tombol (pressure plate).
/// GameObject butuh: Collider2D (set Is Trigger = true).
/// 
/// SETUP DI INSPECTOR:
///   - Tilemap      → drag Tilemap yang berisi tile tombol
///   - Unpressed Tile → TileBase tile tombol belum diinjak
///   - Pressed Tile   → TileBase tile tombol sudah diinjak
/// </summary>
public class PressurePlate : MonoBehaviour
{
    [Header("Tile References")]
    [Tooltip("Tilemap tempat tile tombol berada")]
    [SerializeField] private Tilemap tilemap;

    [Tooltip("Tile saat tombol belum diinjak (original)")]
    [SerializeField] private TileBase unpressedTile;

    [Tooltip("Tile saat tombol sudah diinjak")]
    [SerializeField] private TileBase pressedTile;

    private Vector3Int tilePosition;
    private bool isPressed = false;

    private void Awake()
    {
        // Hitung posisi tile berdasarkan posisi world GameObject ini
        tilePosition = tilemap.WorldToCell(transform.position);
    }

    private void OnEnable()
    {
        PuzzleManager.OnPuzzleReset += ResetPlate;
    }

    private void OnDisable()
    {
        PuzzleManager.OnPuzzleReset -= ResetPlate;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Abaikan jika sudah ditekan atau bukan player
        if (isPressed) return;
        if (!other.CompareTag("Player")) return;

        ActivatePlate();
    }

    private void ActivatePlate()
    {
        isPressed = true;

        // Swap tile ke sprite "sudah ditekan"
        tilemap.SetTile(tilePosition, pressedTile);

        // Lapor ke PuzzleManager
        PuzzleManager.Instance.RegisterButtonPressed();

        Debug.Log($"[PressurePlate] '{gameObject.name}' diaktifkan.");
    }

    /// <summary>
    /// Dipanggil saat puzzle reset (player mati).
    /// </summary>
    private void ResetPlate()
    {
        isPressed = false;

        // Kembalikan tile ke sprite original
        tilemap.SetTile(tilePosition, unpressedTile);

        Debug.Log($"[PressurePlate] '{gameObject.name}' di-reset.");
    }
}