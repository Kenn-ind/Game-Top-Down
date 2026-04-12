using UnityEngine;

/// <summary>
/// Mengontrol hotbar: spawn slot, highlight, tambah/hapus item.
/// PERUBAHAN dari versi lama:
/// - Tidak ada perubahan logika — ItemUser yang menangani pemakaian item.
/// - RemoveFromSelectedSlot() ditambah agar ItemUser bisa hapus item dengan mudah.
/// </summary>
public class HotbarController : MonoBehaviour
{
    public GameObject hotbarPanel;
    public GameObject hotbarSlotPrefab;
    public int slotCount = 4;
    public GameObject itemPrefab;

    private HotbarSlot[] slots;
    private int selectedIndex = 0;

    public static HotbarController Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        slots = new HotbarSlot[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObj = Instantiate(hotbarSlotPrefab, hotbarPanel.transform);
            slots[i] = slotObj.GetComponent<HotbarSlot>();
        }
        UpdateHighlight();
    }

    void Update()
    {
        for (int i = 0; i < slotCount; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                selectedIndex = i;
                UpdateHighlight();
            }
        }
    }

    public void SetHotbarVisible(bool visible)
    {
        hotbarPanel.SetActive(visible);
    }

    void UpdateHighlight()
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].SetHighlight(i == selectedIndex);
    }

    // ─── Tambah Item ────────────────────────────────────────────────────────────

    public bool AddItem(ItemData data, int count)
    {
        // Coba stack ke slot yang sudah ada
        foreach (HotbarSlot slot in slots)
        {
            if (slot.currentItem == null) continue;
            ItemUI itemUI = slot.currentItem.GetComponent<ItemUI>();
            if (itemUI != null && itemUI.itemData == data)
            {
                int space = data.maxStack - itemUI.stackCount;
                if (space > 0)
                {
                    itemUI.stackCount += Mathf.Min(space, count);
                    itemUI.UpdateUI();
                    return true;
                }
            }
        }

        // Slot kosong
        foreach (HotbarSlot slot in slots)
        {
            if (slot.currentItem == null)
            {
                SpawnItem(slot, data, count);
                return true;
            }
        }

        Debug.Log("Hotbar penuh!");
        return false;
    }

    void SpawnItem(HotbarSlot slot, ItemData data, int count)
    {
        GameObject item = Instantiate(itemPrefab, slot.transform);
        item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        ItemUI itemUI = item.GetComponent<ItemUI>();
        itemUI.itemData = data;
        itemUI.stackCount = count;
        itemUI.UpdateUI();
        slot.currentItem = item;
    }

    // ─── Akses Slot ─────────────────────────────────────────────────────────────

    public HotbarSlot GetSelectedSlot() => slots[selectedIndex];

    public ItemUI GetSelectedItem()
    {
        HotbarSlot slot = GetSelectedSlot();
        if (slot.currentItem == null) return null;
        return slot.currentItem.GetComponent<ItemUI>();
    }

    /// <summary>
    /// Hapus item dari slot yang sedang dipilih (dipanggil oleh ItemUser setelah pakai).
    /// </summary>
    public void RemoveFromSelectedSlot()
    {
        HotbarSlot slot = GetSelectedSlot();
        if (slot.currentItem != null)
        {
            Object.Destroy(slot.currentItem);
            slot.currentItem = null;
        }
    }

    public HotbarSlot[] GetSlots() => slots;
}