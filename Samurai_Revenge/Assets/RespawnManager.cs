using UnityEngine;
using Cinemachine;

/// <summary>
/// Taruh script ini di GameObject bernama "RespawnManager" di scene.
/// Tugasnya: simpan spawn point, panggil reset puzzle, pindahkan player.
///
/// SETUP DI INSPECTOR:
///   - Spawn Point → drag Transform GameObject spawn point (empty GameObject)
///   - Player      → drag GameObject player
/// </summary>
public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance { get; private set; }

    [Header("References")]
    [Tooltip("Empty GameObject sebagai titik spawn player")]
    [SerializeField] private Transform spawnPoint;

    [Tooltip("GameObject player")]
    [SerializeField] private GameObject player;

    [Header("Optional")]
    [Tooltip("Delay sebelum respawn (detik). 0 = langsung.")]
    [SerializeField] private float respawnDelay = 0f;

    [Header("Camera")]
    [SerializeField] private PolygonCollider2D spawnBoundary; // drag seperti di MapTransisi
    private CinemachineConfiner _confiner;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _confiner = FindObjectOfType<CinemachineConfiner>();
    }

    /// <summary>
    /// Dipanggil oleh PlayerHealth saat player mati.
    /// </summary>
    public void HandlePlayerDeath()
    {
        if (respawnDelay > 0f)
            StartCoroutine(RespawnCoroutine());
        else
            ExecuteRespawn();
    }

    private System.Collections.IEnumerator RespawnCoroutine()
    {
        // Opsional: nonaktifkan player dulu selama delay
        player.SetActive(false);
        yield return new WaitForSeconds(respawnDelay);
        player.SetActive(true);
        ExecuteRespawn();
    }

    private void ExecuteRespawn()
    {
        // 1. Reset puzzle
        PuzzleManager.Instance.ResetPuzzle();

        // 2. Pindahkan player ke spawn point
        player.transform.position = spawnPoint.position;

        // 3. Set boundary ke room tempat spawn point berada
        if (_confiner != null && spawnBoundary != null)
        {
            _confiner.m_BoundingShape2D = spawnBoundary;
            _confiner.InvalidatePathCache();
        }

        // Reset HP dan state player setelah posisi diset
        player.GetComponent<PlayerHealth>().OnRespawn();

        Debug.Log("[RespawnManager] Player respawned.");
    }

    // Helper: lihat spawn point dari scene view
    private void OnDrawGizmos()
    {
        if (spawnPoint == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(spawnPoint.position, 0.3f);
        Gizmos.DrawIcon(spawnPoint.position, "sv_icon_dot6_pix16_gizmo", true);
    }
}