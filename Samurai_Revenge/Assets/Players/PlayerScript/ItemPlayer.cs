using UnityEngine;

public class ItemPlayer : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0))
            UseSelectedItem();
    }

    public void UseSelectedItem()
    {
        if (HotbarController.Instance == null)
        {
            Debug.LogWarning("[ItemPlayer] HotbarController.Instance tidak ditemukan!");
            return;
        }

        ItemUI itemUI = HotbarController.Instance.GetSelectedItem();
        if (itemUI == null)
        {
            Debug.Log("[ItemPlayer] Slot hotbar kosong.");
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
            Debug.Log($"[ItemPlayer] '{data.itemName}' tidak punya itemPrefab.");
            return;
        }

        IUsable usable = data.itemPrefab.GetComponent<IUsable>();
        if (usable == null)
        {
            Debug.Log($"[ItemPlayer] '{data.itemName}' tidak punya script IUsable.");
            return;
        }

        if (!usable.CanUse(gameObject))
        {
            Debug.Log($"[ItemPlayer] '{data.itemName}' tidak bisa digunakan sekarang.");
            return;
        }

        usable.Use(gameObject);

        itemUI.stackCount--;
        if (itemUI.stackCount <= 0)
        {
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