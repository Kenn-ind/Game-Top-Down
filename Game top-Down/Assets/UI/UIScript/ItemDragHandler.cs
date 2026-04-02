using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    Transform originalParent;
    CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    // ─── Shift + Click ────────────────────────────────────────────────────────

    public void OnPointerClick(PointerEventData eventData)
    {
        // Hanya proses shift + klik kiri
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)) return;

        ChestController openChest = ChestController.CurrentOpenChest;
        if (openChest == null) return; // Chest tidak sedang terbuka, abaikan

        ChestUI chestUI = openChest.chestUI;
        Slot thisSlot = transform.parent.GetComponent<Slot>();
        if (thisSlot == null) return;

        bool isChestSlot = thisSlot.CompareTag("ChestSlot");

        if (isChestSlot)
        {
            // Item ada di chest → pindah ke inventory
            chestUI.ShiftClickFromChest(thisSlot);
        }
        else
        {
            // Item ada di inventory → pindah ke chest
            chestUI.ShiftClickFromInventory(thisSlot);
        }
    }

    // ─── Drag & Drop (tidak berubah dari versi kamu) ──────────────────────────

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        Slot dropSlot = null;
        if (eventData.pointerEnter != null)
        {
            dropSlot = eventData.pointerEnter.GetComponent<Slot>();
            if (dropSlot == null)
                dropSlot = eventData.pointerEnter.GetComponentInParent<Slot>();
        }

        Slot originalSlot = originalParent?.GetComponent<Slot>();
        ItemUI draggedItemUI = GetComponent<ItemUI>();

        if (dropSlot != null && draggedItemUI != null)
        {
            ItemUI targetItemUI = dropSlot.GetItemUI();
            bool sameItem = targetItemUI != null
                            && targetItemUI.itemData == draggedItemUI.itemData;

            if (sameItem)
            {
                int space = targetItemUI.itemData.maxStack - targetItemUI.stackCount;
                int transfer = Mathf.Min(space, draggedItemUI.stackCount);
                targetItemUI.stackCount += transfer;
                targetItemUI.UpdateUI();
                draggedItemUI.stackCount -= transfer;

                if (draggedItemUI.stackCount <= 0)
                {
                    if (originalSlot != null) originalSlot.currentItem = null;
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    transform.SetParent(originalParent);
                    draggedItemUI.UpdateUI();
                }
            }
            else if (targetItemUI != null && originalSlot != null)
            {
                GameObject swappedItem = dropSlot.currentItem;
                swappedItem.transform.SetParent(originalSlot.transform);
                swappedItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                originalSlot.currentItem = swappedItem;
                transform.SetParent(dropSlot.transform);
                dropSlot.currentItem = gameObject;
            }
            else
            {
                if (originalSlot != null) originalSlot.currentItem = null;
                transform.SetParent(dropSlot.transform);
                dropSlot.currentItem = gameObject;
            }
        }
        else
        {
            transform.SetParent(originalParent);
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
}