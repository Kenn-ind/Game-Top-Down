using UnityEngine;

public class Slot : MonoBehaviour
{
    public GameObject currentItem;

    public ItemUI GetItemUI()
    {
        if (currentItem == null) return null;
        return currentItem.GetComponent<ItemUI>();
    }

    public bool CanAccept(ItemUI incomingItem)
    {
        if (currentItem == null) return true;

        ItemUI existing = GetItemUI();
        if (existing == null) return false;

        return existing.itemData == incomingItem.itemData
               && existing.stackCount < existing.itemData.maxStack;
    }
}