using UnityEngine;
using UnityEngine.UI;
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

        canvasGroup.blocksRaycasts = true;
    }

    // ─── Shift + Click ────────────────────────────────────────────────────────

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        Slot thisSlot = transform.parent.GetComponent<Slot>();
        Debug.Log($"[Click] thisSlot={thisSlot?.name ?? "NULL"}, chestOpen={ChestController.CurrentOpenChest != null}, isChestSlot={thisSlot?.CompareTag("ChestSlot")}");

        if (thisSlot == null) return;

        bool chestIsOpen = ChestController.CurrentOpenChest != null;
        bool isChestSlot = thisSlot.CompareTag("ChestSlot");

        if (chestIsOpen && isChestSlot
            && !Input.GetKey(KeyCode.LeftShift)
            && !Input.GetKey(KeyCode.RightShift))
        {
            Debug.Log($"[Click] Show popup, ChestItemPopup.Instance={ChestItemPopup.Instance != null}");
            ChestItemPopup.Instance?.Show(thisSlot, ChestController.CurrentOpenChest,
                transform.position);
            return;
        }

        // ── Shift + Click ────────────────────────────────────────
        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)) return;

        HotbarSlot hotbarSlot = transform.parent.GetComponent<HotbarSlot>();
        if (hotbarSlot != null) return;

        if (chestIsOpen)
        {
            ChestUI chestUI = ChestController.CurrentOpenChest.chestUI;

            if (isChestSlot)
                chestUI.ShiftClickFromChest(thisSlot);
            else
                chestUI.ShiftClickFromInventory(thisSlot);
        }
        else
        {
            if (isChestSlot) return;

            ItemUI itemUI = GetComponent<ItemUI>();
            if (itemUI == null || itemUI.itemData == null) return;

            bool added = HotbarController.Instance.AddItem(itemUI.itemData, itemUI.stackCount);
            if (added)
            {
                Slot slot = transform.parent.GetComponent<Slot>();
                if (slot != null)
                {
                    Destroy(slot.currentItem);
                    slot.currentItem = null;
                }
            }
            else
            {
                Debug.Log("[ShiftClick] Hotbar penuh!");
            }
        }
    }

    // ─── Drag & Drop ──────────────────────────────────────────────────────────

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
                    if (originalSlot != null)
                        originalSlot.currentItem = null;

                    Destroy(gameObject);
                    return;
                }
                else
                {
                    transform.SetParent(originalParent);
                    GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    transform.SetAsLastSibling();

                    draggedItemUI.UpdateUI();
                }
            }
            else if (targetItemUI != null && originalSlot != null)
            {
                // ─── Swap ─────────────────────────────────────────────

                GameObject swappedItem = dropSlot.currentItem;

                swappedItem.transform.SetParent(originalSlot.transform);
                swappedItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                swappedItem.transform.SetAsFirstSibling();

                originalSlot.currentItem = swappedItem;

                transform.SetParent(dropSlot.transform);
                GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                transform.SetAsLastSibling();

                dropSlot.currentItem = gameObject;

                // ─── Update Raycast ─────────────────────────────────

                draggedItemUI.SetRaycast(!(dropSlot is HotbarSlot));
                targetItemUI.SetRaycast(!(originalSlot is HotbarSlot));
            }
            else
            {
                // ─── Pindah biasa ───────────────────────────────────

                if (originalSlot != null)
                    originalSlot.currentItem = null;

                transform.SetParent(dropSlot.transform);
                GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                transform.SetAsLastSibling();

                dropSlot.currentItem = gameObject;

                // ─── Update Raycast ─────────────────────────────────

                draggedItemUI.SetRaycast(!(dropSlot is HotbarSlot));
            }
        }
        else
        {
            // ─── Drop di luar slot ─────────────────────────────────

            transform.SetParent(originalParent);
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            transform.SetAsLastSibling();
        }
    }
}