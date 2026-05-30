using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChestItemPopup : MonoBehaviour
{
    public static ChestItemPopup Instance { get; private set; }

    [Header("References")]
    public GameObject popupPanel;
    public Button takeButton;
    public Button cancelButton;
    public TMP_Text itemNameText;

    private Slot currentSlot;
    private ChestController currentChest;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        popupPanel?.SetActive(false);
        takeButton?.onClick.AddListener(OnTake);
        cancelButton?.onClick.AddListener(OnCancel);
    }

    // ─── Show Popup ───────────────────────────────────────────────────────────

    public void Show(Slot slot, ChestController chest, Vector3 worldPosition)
    {
        if (slot == null || slot.currentItem == null) return;

        currentSlot = slot;
        currentChest = chest;

        // Set nama item
        ItemUI itemUI = slot.currentItem.GetComponent<ItemUI>();
        if (itemNameText != null && itemUI != null)
            itemNameText.text = itemUI.itemData?.itemName ?? "";

        // Posisikan popup di samping item yang di-click
        popupPanel?.SetActive(true);
        PositionPopup(worldPosition);
    }

    void PositionPopup(Vector3 worldPosition)
    {
        if (popupPanel == null) return;

        RectTransform rect = popupPanel.GetComponent<RectTransform>();
        if (rect == null) return;

        RectTransform slotRect = currentSlot?.GetComponent<RectTransform>();
        if (slotRect == null)
        {
            rect.anchoredPosition = Vector2.zero;
            return;
        }

        // Dapatkan posisi slot dalam screen space
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(
            null, slotRect.position);

        // Convert ke anchored position dalam Canvas
        Canvas canvas = popupPanel.GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos, null, out localPoint);

        // Offset ke kanan item
        localPoint.x += slotRect.sizeDelta.x + 10f;

        // Clamp agar tidak keluar canvas
        float halfW = rect.sizeDelta.x / 2f;
        float halfH = rect.sizeDelta.y / 2f;
        float cHalfW = canvasRect.sizeDelta.x / 2f;
        float cHalfH = canvasRect.sizeDelta.y / 2f;

        localPoint.x = Mathf.Clamp(localPoint.x, -cHalfW + halfW, cHalfW - halfW);
        localPoint.y = Mathf.Clamp(localPoint.y, -cHalfH + halfH, cHalfH - halfH);

        rect.anchoredPosition = localPoint;
    }

    // ─── Actions ──────────────────────────────────────────────────────────────

    void OnTake()
    {
        if (currentSlot == null || currentChest == null)
        {
            Hide();
            return;
        }

        ItemUI itemUI = currentSlot.currentItem?.GetComponent<ItemUI>();
        if (itemUI == null)
        {
            Hide();
            return;
        }

        // Coba masukkan ke inventory player
        InventoryController inventory = FindObjectOfType<InventoryController>();
        if (inventory != null)
        {
            bool success = inventory.AddItem(itemUI.itemData, itemUI.stackCount);
            if (success)
            {
                currentChest.RemoveItemFromChest(currentSlot);
                Debug.Log($"[ChestPopup] Item diambil: {itemUI.itemData?.itemName}");
            }
            else
            {
                Debug.Log("[ChestPopup] Inventory penuh!");
            }
        }

        Hide();
    }

    void OnCancel()
    {
        Hide();
    }

    public void Hide()
    {
        popupPanel?.SetActive(false);
        currentSlot = null;
        currentChest = null;
    }
}