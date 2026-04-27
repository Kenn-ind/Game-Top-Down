using UnityEngine;

public class ItemPlayer : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.E))
            UseSelectedItem();
    }

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

    public void UseItem(ItemUI itemUI)
    {
        if (itemUI == null || itemUI.itemData == null) return;

        ItemData data = itemUI.itemData;

        if (data.itemPrefab == null)
        {
            Debug.Log($"[ItemUser] '{data.itemName}' tidak punya itemPrefab, tidak bisa digunakan.");
            return;
        }

        IUsable usable = data.itemPrefab.GetComponent<IUsable>();
        if (usable == null)
        {
            Debug.Log($"[ItemUser] '{data.itemName}' tidak punya script IUsable di prefabnya.");
            return;
        }

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