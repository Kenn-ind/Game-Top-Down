using UnityEngine;

// ============================================================
//  ScrollItem.cs
//  Attach ke Prefab Scroll, sama seperti HealthPotionItem.cs
//
//  SETUP:
//  1. Buat GameObject bernama "ScrollItem"
//  2. Attach script ini
//  3. Buat ItemData SO baru (klik kanan di Project →
//     Create → Inventory → Item)
//     - itemName  : "Upgrade Scroll"
//     - itemID    : "scroll"
//     - maxStack  : 99
//     - itemPrefab: drag prefab ScrollItem ini
//  4. Assign ItemData SO ke field itemData di Inspector
// ============================================================

public class ScrollItem : MonoBehaviour, IUsable
{
    public int scrollAmount = 1; // berapa scroll yang ditambah saat dipakai

    public bool CanUse(GameObject user)
    {
        // Scroll selalu bisa dipakai
        return true;
    }

    public void Use(GameObject user)
    {
        ScrollInventory scrollInventory = user.GetComponent<ScrollInventory>();
        if (scrollInventory == null)
        {
            Debug.LogWarning("[ScrollItem] ScrollInventory tidak ditemukan di Player!");
            return;
        }

        scrollInventory.AddScroll(scrollAmount);
        Debug.Log($"[ScrollItem] Menambah {scrollAmount} scroll. Total: {scrollInventory.scrollCount}");
    }
}