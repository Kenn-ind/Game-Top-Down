using UnityEngine;
using UnityEngine.UI;

public class HotbarController : MonoBehaviour
{
    [Header("References")]
    public GameObject hotbarPanel;
    public GameObject hotbarSlotPrefab;
    public GameObject itemPrefab;

    [Header("Settings")]
    public int slotCount = 4;
    public float doubleTapThreshold = 0.35f;

    private HotbarSlot[] slots;
    private int selectedIndex = 0;

    // Double tap tracking
    private int lastTappedIndex = -1;
    private float lastTapTime = 0f;

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

            int index = i;

            // Coba ambil Button yang ada di slot
            Button btn = slotObj.GetComponent<Button>();

            // Kalau tidak ada, tambahkan otomatis
            if (btn == null)
                btn = slotObj.AddComponent<Button>();

            // Pastikan transition none agar tidak mengubah tampilan
            btn.transition = Selectable.Transition.None;

            btn.onClick.AddListener(() => OnSlotTapped(index));
        }

        UpdateHighlight();
    }

    void Update()
    {
        // Keyboard: tombol 1-4
        for (int i = 0; i < slotCount; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                selectedIndex = i;
                UpdateHighlight();
            }
        }
    }

    // ─── Mobile Tap Logic ─────────────────────────────────────────────────────

    public void OnSlotTapped(int index)
    {
        Debug.Log($"[Hotbar] OnSlotTapped index={index}");

        float now = Time.unscaledTime;

        bool isDoubleTap =
            (index == lastTappedIndex)
            && (index == selectedIndex)
            && (now - lastTapTime <= doubleTapThreshold);

        if (isDoubleTap)
        {
            UseSelectedItem();

            lastTappedIndex = -1;
            lastTapTime = 0f;
        }
        else
        {
            selectedIndex = index;

            UpdateHighlight();

            lastTappedIndex = index;
            lastTapTime = now;
        }
    }

    void UseSelectedItem()
    {
        ItemPlayer itemPlayer = FindObjectOfType<ItemPlayer>();

        if (itemPlayer != null)
            itemPlayer.UseSelectedItem();
        else
            Debug.LogWarning("[HotbarController] ItemPlayer tidak ditemukan di scene!");
    }

    // ─── Visibility ──────────────────────────────────────────────────────────

    public void SetHotbarVisible(bool visible)
    {
        hotbarPanel.SetActive(visible);
    }

    // ─── Highlight ───────────────────────────────────────────────────────────

    void UpdateHighlight()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetHighlight(i == selectedIndex);

            Debug.Log($"[Hotbar] Slot {i}: highlight={i == selectedIndex}, selectedIndex={selectedIndex}");
        }
    }

    // ─── Tambah Item ─────────────────────────────────────────────────────────

    public bool AddItem(ItemData data, int count)
    {
        // 1. Stack ke slot yang sudah ada
        foreach (HotbarSlot slot in slots)
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

                    if (count <= 0)
                        return true;
                }
            }
        }

        // 2. Sisa ke slot kosong
        while (count > 0)
        {
            HotbarSlot emptySlot = null;

            foreach (HotbarSlot slot in slots)
            {
                if (slot.currentItem == null)
                {
                    emptySlot = slot;
                    break;
                }
            }

            if (emptySlot == null)
            {
                Debug.Log("[HotbarController] Hotbar penuh!");
                return false;
            }

            int spawnCount = Mathf.Min(count, data.maxStack);

            SpawnItem(emptySlot, data, spawnCount);

            count -= spawnCount;
        }

        return true;
    }

    void SpawnItem(HotbarSlot slot, ItemData data, int count)
    {
        GameObject item = Instantiate(itemPrefab, slot.transform);

        item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        // Item jadi child pertama
        item.transform.SetAsFirstSibling();

        ItemUI itemUI = item.GetComponent<ItemUI>();

        itemUI.itemData = data;
        itemUI.stackCount = count;

        itemUI.UpdateUI();

        // Hotbar = raycast mati
        itemUI.SetRaycast(false);

        slot.currentItem = item;

        Debug.Log($"[Hotbar] SpawnItem selesai: slot={slot.name}, currentItem={slot.currentItem?.name ?? "NULL"}, itemData={data.itemName}");
    }

    public HotbarSlot GetSelectedSlot()
    {
        return slots[selectedIndex];
    }

    public ItemUI GetSelectedItem()
    {
        HotbarSlot slot = GetSelectedSlot();

        if (slot == null)
            return null;

        // Cek missing reference
        if (slot.currentItem != null && !slot.currentItem)
        {
            slot.currentItem = null;
            return null;
        }

        if (slot.currentItem == null)
            return null;

        return slot.currentItem.GetComponent<ItemUI>();
    }

    public void RemoveFromSelectedSlot()
    {
        HotbarSlot slot = GetSelectedSlot();

        if (slot.currentItem != null)
        {
            Destroy(slot.currentItem);
            slot.currentItem = null;
        }
    }

    public HotbarSlot[] GetSlots()
    {
        return slots;
    }
}