using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using System.Collections;

/// <summary>
/// Taruh script ini di GameObject bernama "StairController" di scene.
/// Mengatur tile penutup tangga: dibuka saat semua tombol aktif,
/// ditutup kembali saat puzzle di-reset.
///
/// SETUP DI INSPECTOR:
///   - Tilemap    → drag Tilemap yang berisi tile tangga
///   - Cover Tile → TileBase tile penutup tangga (sebelum terbuka)
///   - Stair Tile → TileBase tile tangga asli (setelah terbuka)
///   - Stair World Position → posisi world tile tangga di scene
/// </summary>
public class StairController : MonoBehaviour
{
    AudioManage AudioManager;
    [Header("Tile References")]
    [Tooltip("Tilemap tempat tile tangga berada")]
    [SerializeField] private Tilemap tilemap;

    [Tooltip("Tile penutup tangga (tampil saat puzzle belum selesai)")]
    [SerializeField] private TileBase coverTile;

    [Tooltip("Tile tangga asli (tampil saat puzzle selesai)")]
    [SerializeField] private TileBase stairTile;

    [Header("Position")]
    [Tooltip("Posisi world dari tile tangga. Bisa di-drag dari scene view.")]
    [SerializeField] private Vector3 stairWorldPosition;

    [Header("Notification")]
    [SerializeField] private TextMeshProUGUI notifText;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float fadeDuration = 0.4f;

    public GameObject WPBoss;

    private Vector3Int tilePosition;

    private void Awake()
    {
        AudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManage>();
        tilePosition = tilemap.WorldToCell(stairWorldPosition);
    }

    private void OnEnable()
    {
        PuzzleManager.OnAllButtonsPressed += OpenStairs;
        PuzzleManager.OnPuzzleReset += CloseStairs;
    }

    private void OnDisable()
    {
        PuzzleManager.OnAllButtonsPressed -= OpenStairs;
        PuzzleManager.OnPuzzleReset -= CloseStairs;
    }

    /// <summary>
    /// Hapus tile penutup, tampilkan tile tangga.
    /// </summary>
    private void OpenStairs()
    {
        AudioManager.PlaySFX(AudioManager.DoorSfx);
        tilemap.SetTile(tilePosition, stairTile);
        Debug.Log("[StairController] Tangga terbuka!");
        if (notifText != null)
            StartCoroutine(ShowNotification());
        WPBoss.SetActive(true);
    }

    /// <summary>
    /// Kembalikan tile penutup di atas tangga.
    /// </summary>
    private void CloseStairs()
    {
        tilemap.SetTile(tilePosition, coverTile);
        Debug.Log("[StairController] Tangga tertutup kembali.");
        WPBoss.SetActive(false);
    }

    // Helper: klik kanan GameObject di Inspector → "Copy Stair Position from Transform"
    // Berguna saat setup posisi tangga di scene
    [ContextMenu("Set Stair Position from Transform")]
    private void SetPositionFromTransform()
    {
        stairWorldPosition = transform.position;
        Debug.Log($"[StairController] Stair position set to {stairWorldPosition}");
    }

    private IEnumerator ShowNotification()
    {
        // Reset
        notifText.gameObject.SetActive(true);
        Color c = notifText.color;

        // Fade in
        c.a = 0f;
        notifText.color = c;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(t / fadeDuration);
            notifText.color = c;
            yield return null;
        }

        // Tahan
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(1f - (t / fadeDuration));
            notifText.color = c;
            yield return null;
        }

        notifText.gameObject.SetActive(false);
    }
}