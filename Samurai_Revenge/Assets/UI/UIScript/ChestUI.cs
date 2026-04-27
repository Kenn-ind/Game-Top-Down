using System.Collections.Generic;
using UnityEngine;

public class ChestUI : MonoBehaviour
{
    public GameObject panel;

    public TabController tabController;
    public GameObject inventoryPages;
    public GameObject chestSlotContainer;

    public GameObject slotPrefab;
    public GameObject itemPrefab;

    public int slotCount = 20;

    private Slot[] slots;
    private InventoryController playerInventory;
    private ChestController currentChest;

    void Awake()
    {
        playerInventory = FindObjectOfType<InventoryController>();
        panel.SetActive(false);
    }

    public void Open(List<ChestData.ChestItem> runtimeItems, ChestController chest)
    {
        currentChest = chest;

        chestSlotContainer.SetActive(true);
        inventoryPages.SetActive(true);

        foreach (Transform child in chestSlotContainer.transform)
            Destroy(child.gameObject);

        slots = new Slot[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, chestSlotContainer.transform);
            slots[i] = slotObj.GetComponent<Slot>();
            slots[i].gameObject.tag = "ChestSlot";
        }

        for (int i = 0; i < runtimeItems.Count && i < slotCount; i++)
        {
            var entry = runtimeItems[i];
            if (entry.itemData == null || entry.count <= 0) continue;
            SpawnItem(slots[i], entry.itemData, entry.count);
        }

        panel.SetActive(true);
        tabController.ActivateTab(0);
    }

    public void Close()
    {
        panel.SetActive(false);
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

    public void ShiftClickFromChest(Slot chestSlot)
    {
        if (chestSlot.currentItem == null) return;
        ItemUI itemUI = chestSlot.currentItem.GetComponent<ItemUI>();
        if (itemUI == null) return;

        bool success = playerInventory.AddItem(itemUI.itemData, itemUI.stackCount);
        if (success)
        {
            var runtimeItems = currentChest.GetRuntimeItems();
            for (int i = 0; i < runtimeItems.Count; i++)
            {
                if (runtimeItems[i].itemData == itemUI.itemData)
                {
                    runtimeItems[i] = new ChestData.ChestItem
                    {
                        itemData = runtimeItems[i].itemData,
                        count = 0
                    };
                    break;
                }
            }

            Destroy(chestSlot.currentItem);
            chestSlot.currentItem = null;
        }
        else
        {
            Debug.Log("Inventory penuh!");
        }
    }

    public void ShiftClickFromInventory(Slot inventorySlot)
    {
        if (inventorySlot.currentItem == null) return;
        ItemUI itemUI = inventorySlot.currentItem.GetComponent<ItemUI>();
        if (itemUI == null) return;

        // Coba stack dulu
        foreach (Slot slot in slots)
        {
            if (slot.currentItem == null) continue;
            ItemUI existing = slot.GetItemUI();
            if (existing != null && existing.itemData == itemUI.itemData)
            {
                int space = existing.itemData.maxStack - existing.stackCount;
                if (space > 0)
                {
                    int transfer = Mathf.Min(space, itemUI.stackCount);
                    existing.stackCount += transfer;
                    existing.UpdateUI();
                    itemUI.stackCount -= transfer;
                    UpdateRuntimeItems(existing.itemData, existing.stackCount);

                    if (itemUI.stackCount <= 0)
                    {
                        Destroy(inventorySlot.currentItem);
                        inventorySlot.currentItem = null;
                        return;
                    }
                }
            }
        }

        Slot emptySlot = null;
        foreach (Slot slot in slots)
        {
            if (slot.currentItem == null)
            {
                emptySlot = slot;
                break;
            }
        }

        if (emptySlot != null)
        {
            SpawnItem(emptySlot, itemUI.itemData, itemUI.stackCount);

            AddToRuntimeItems(itemUI.itemData, itemUI.stackCount);

            Destroy(inventorySlot.currentItem);
            inventorySlot.currentItem = null;
        }
        else
        {
            Debug.Log("Chest penuh!");
        }
    }

    void AddToRuntimeItems(ItemData data, int count)
    {
        var runtimeItems = currentChest.GetRuntimeItems();

        for (int i = 0; i < runtimeItems.Count; i++)
        {
            if (runtimeItems[i].itemData == data)
            {
                runtimeItems[i] = new ChestData.ChestItem
                {
                    itemData = data,
                    count = runtimeItems[i].count + count
                };
                return;
            }
        }

        runtimeItems.Add(new ChestData.ChestItem
        {
            itemData = data,
            count = count
        });
    }
    void UpdateRuntimeItems(ItemData data, int newCount)
    {
        var runtimeItems = currentChest.GetRuntimeItems();
        for (int i = 0; i < runtimeItems.Count; i++)
        {
            if (runtimeItems[i].itemData == data)
            {
                runtimeItems[i] = new ChestData.ChestItem
                {
                    itemData = data,
                    count = newCount
                };
                return;
            }
        }
    }


    public Slot[] GetSlots() => slots;
}