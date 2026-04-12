using UnityEngine;

/// <summary>
/// Taruh script ini di GameObject PLAYER.
/// HotbarController boleh ada di tempat lain (Canvas/HotbarPanel),
/// karena diambil lewat HotbarController.Instance.
/// </summary>
public class ItemPlayer : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.E))
            UseSelectedItem();
    }

    /// <summary>
    /// Gunakan item dari slot hotbar yang sedang dipilih.
    /// Bisa juga dipanggil dari UI Button.
    /// </summary>
    public void UseSelectedItem()
    {
        if (HotbarController.Instance == null)
        {
            Debug.LogWarning("[ItemUser] HotbarController.Instance tidak ditemukan!");
            return;
        }

        ItemUI itemUI = HotbarController.Instance.GetSelectedItem();

        if (itemUI == null)
        {
            Debug.Log("[ItemUser] Slot hotbar kosong.");
            return;
        }

        UseItem(itemUI);
    }

    /// <summary>
    /// Gunakan item dari ItemUI tertentu.
    /// </summary>
    public void UseItem(ItemUI itemUI)
    {
        if (itemUI == null || itemUI.itemData == null) return;

        ItemData data = itemUI.itemData;

        // Cek prefab
        if (data.itemPrefab == null)
        {
            Debug.Log($"[ItemUser] '{data.itemName}' tidak punya itemPrefab, tidak bisa digunakan.");
            return;
        }

        // Cari IUsable di prefab
        IUsable usable = data.itemPrefab.GetComponent<IUsable>();
        if (usable == null)
        {
            Debug.Log($"[ItemUser] '{data.itemName}' tidak punya script IUsable di prefabnya.");
            return;
        }

        // Cek kondisi (misal HP sudah penuh)
        if (!usable.CanUse(gameObject))
        {
            Debug.Log($"[ItemUser] '{data.itemName}' tidak bisa digunakan sekarang.");
            return;
        }

        // Gunakan!
        usable.Use(gameObject);

        // Kurangi stack
        itemUI.stackCount--;
        if (itemUI.stackCount <= 0)
        {
            // Hapus item dari slot hotbar
            HotbarSlot slot = itemUI.GetComponentInParent<HotbarSlot>();
            if (slot != null)
            {
                Destroy(slot.currentItem);
                slot.currentItem = null;
            }
        }
        else
        {
            itemUI.UpdateUI();
        }
    }
}