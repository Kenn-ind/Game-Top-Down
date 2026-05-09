using UnityEngine;

public class ScrollInventory : MonoBehaviour
{
    public int scrollCount = 0;

    // ── TAMBAH INI ───────────────────────────────────────────
    public event System.Action OnScrollChanged;
    // ────────────────────────────────────────────────────────

    public bool UseScroll()
    {
        if (scrollCount <= 0) return false;
        scrollCount--;
        Debug.Log($"Scroll digunakan. Sisa: {scrollCount}");
        // ── TAMBAH INI ──────────────────────────────────────
        OnScrollChanged?.Invoke();
        // ────────────────────────────────────────────────────
        return true;
    }

    public void AddScroll(int amount)
    {
        scrollCount += amount;
        Debug.Log($"Dapat {amount} scroll. Total: {scrollCount}");
        // ── TAMBAH INI ──────────────────────────────────────
        OnScrollChanged?.Invoke();
        // ────────────────────────────────────────────────────
    }
}