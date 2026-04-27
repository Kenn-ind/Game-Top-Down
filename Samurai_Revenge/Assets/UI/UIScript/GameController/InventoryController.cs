using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject inventoryPanel;      // InventoryPages di Menu (normal)
    public GameObject chestInventoryPanel; // InventoryPages di ChestPanel

    public GameObject slotPrefab;
    public int slotCount;
    public GameObject itemPrefab;
    public ItemData[] itemDataList;
    private Slot[] slots;

    void Start()
    {
        slots = new Slot[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, inventoryPanel.transform);
            slots[i] = slotObj.GetComponent<Slot>();
            if (slots[i] == null)
            {
                Debug.LogError("Slot prefab tidak punya komponen Slot.cs!");
                continue;
            }
            if (i < itemDataList.Length && itemDataList[i] != null)
                SpawnItem(slots[i], itemDataList[i], 1);
        }
    }
    public void MoveToChestPanel()
    {
        foreach (Slot slot in slots)
            slot.transform.SetParent(chestInventoryPanel.transform, false);

        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(
            chestInventoryPanel.GetComponent<RectTransform>());
    }

    public void MoveToInventoryPanel()
    {
        foreach (Slot slot in slots)
            slot.transform.SetParent(inventoryPanel.transform, false);

        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(
            inventoryPanel.GetComponent<RectTransform>());
    }

    void SpawnItem(Slot slot, ItemData data, int count)
    {
        GameObject item = Instantiate(itemPrefab, slot.transform);
        item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        ItemUI itemUI = item.GetComponent<ItemUI>();
        itemUI.itemData = data;
        itemUI.stackCount = count;
        itemUI.UpdateUI();
        slot.currentItem = item;
    }

    public bool AddItem(ItemData data, int count)
    {
        // 1. Coba stack ke slot yang sudah ada dulu
        foreach (Slot slot in slots)
        {
            if (slot.currentItem == null) continue;
            ItemUI itemUI = slot.currentItem.GetComponent<ItemUI>();
            if (itemUI != null && itemUI.itemData == data)
            {
                int space = data.maxStack - itemUI.stackCount;
                if (space > 0)
                {
                    int add = Mathf.Min(space, count);
                    itemUI.stackCount += add;
                    itemUI.UpdateUI();
                    count -= add;
                    if (count <= 0) return true;
                }
            }
        }

        // 2. Sisa count dipecah ke slot kosong sesuai maxStack
        while (count > 0)
        {
            Slot emptySlot = null;
            foreach (Slot slot in slots)
            {
                if (slot.currentItem == null)
                {
                    emptySlot = slot;
                    break;
                }
            }

            if (emptySlot == null)
            {
                Debug.Log($"Inventory penuh! Sisa item tidak bisa masuk: {count}");
                return false;
            }

            int spawnCount = Mathf.Min(count, data.maxStack);
            SpawnItem(emptySlot, data, spawnCount);
            count -= spawnCount;
        }

        return true;
    }

    public bool RemoveItemByID(string itemID, int amount)
    {
        int remaining = amount;
        foreach (Slot slot in slots)
        {
            if (slot.currentItem == null) continue;
            ItemUI itemUI = slot.currentItem.GetComponent<ItemUI>();
            if (itemUI == null || itemUI.itemData.itemID != itemID) continue;
            if (itemUI.stackCount >= remaining)
            {
                itemUI.stackCount -= remaining;
                remaining = 0;
                if (itemUI.stackCount <= 0)
                {
                    Destroy(slot.currentItem);
                    slot.currentItem = null;
                }
                else
                {
                    itemUI.UpdateUI();
                }
                break;
            }
            else
            {
                remaining -= itemUI.stackCount;
                Destroy(slot.currentItem);
                slot.currentItem = null;
            }
        }
        if (remaining > 0)
        {
            Debug.Log($"Item {itemID} tidak cukup di inventory!");
            return false;
        }
        return true;
    }

    public Slot[] GetSlots() => slots;
}