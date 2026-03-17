using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public GameObject inventoryPanel;
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
            {
                SpawnItem(slots[i], itemDataList[i], 1);
            }
        }
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

        foreach (Slot slot in slots)
        {
            if (slot.currentItem == null)
            {
                SpawnItem(slot, data, count);
                return true;
            }
        }

        Debug.Log("Inventory penuh!");
        return false;
    }
}