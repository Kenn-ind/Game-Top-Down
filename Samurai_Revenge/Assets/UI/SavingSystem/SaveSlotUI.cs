using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ============================================================
//  SaveSlotUI.cs  (versi Tab — menyatu dengan TabController)
//
//  SETUP DI UNITY:
//  1. Attach script ini ke GameObject "SavePages"
//  2. Di dalam SavePages, buat struktur:
//
//     SavePages
//     ├── ScrollView
//     │   └── Viewport → Content  ← assign ke slotListContent
//     └── ConfirmPanel (awalnya non-aktif)
//         ├── ConfirmTitleText  (TextMeshProUGUI)
//         ├── BtnLoad           (Button)
//         ├── BtnOverwrite      (Button)
//         ├── BtnDelete         (Button)
//         └── BtnCancel         (Button)
//
//  3. Buat prefab SlotEntryPrefab:
//     SlotEntryPrefab (Button)
//     ├── SlotNameText  (TextMeshProUGUI)  → "Save 1"
//     ├── SlotDateText  (TextMeshProUGUI)  → "27/04/2025 20:30"
//     └── SlotInfoText  (TextMeshProUGUI)  → "Lv.5 | 250 coins"
// ============================================================

public class SaveSlotUI : MonoBehaviour
{
    public static SaveSlotUI Instance;

    [Header("Slot List")]
    public Transform slotListContent;       // Content di ScrollView
    public GameObject slotEntryPrefab;      // Prefab tiap baris slot

    [Header("Confirm Panel")]
    public GameObject confirmPanel;
    public TextMeshProUGUI confirmTitleText;
    public Button btnLoad;
    public Button btnOverwrite;
    public Button btnDelete;
    public Button btnCancel;

    private int selectedSlotIndex = -1;
    private List<GameObject> spawnedEntries = new List<GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        confirmPanel.SetActive(false);

        btnLoad.onClick.AddListener(OnClickLoad);
        btnOverwrite.onClick.AddListener(OnClickOverwrite);
        btnDelete.onClick.AddListener(OnClickDelete);
        btnCancel.onClick.AddListener(() =>
        {
            confirmPanel.SetActive(false);
            selectedSlotIndex = -1;
        });

    }

    public void RefreshSlotList()
    {
        confirmPanel.SetActive(false);
        selectedSlotIndex = -1;

        // Hapus entry lama
        foreach (GameObject go in spawnedEntries) Destroy(go);
        spawnedEntries.Clear();

        for (int i = 0; i < SaveManager.Instance.maxSlots; i++)
        {
            int index = i; // capture untuk closure lambda
            GameObject entry = Instantiate(slotEntryPrefab, slotListContent);
            spawnedEntries.Add(entry);

            TextMeshProUGUI nameText = entry.transform.Find("SlotNameText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI dateText = entry.transform.Find("SlotDateText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI infoText = entry.transform.Find("SlotInfoText")?.GetComponent<TextMeshProUGUI>();
            Button btn = entry.GetComponent<Button>();

            if (SaveManager.Instance.SlotExists(index))
            {
                SaveData meta = SaveManager.Instance.ReadSlotMeta(index);
                if (nameText) nameText.text = meta.slotName;
                if (dateText) dateText.text = meta.saveDateTime;
                if (infoText) infoText.text = $"Lv.{meta.level}  |  {meta.coins} koin";
            }
            else
            {
                if (nameText) nameText.text = $"--- Slot {index + 1} ---";
                if (dateText) dateText.text = "Kosong";
                if (infoText) infoText.text = "";
            }

            btn?.onClick.AddListener(() => OnClickSlot(index));
        }
    }

    // ── Klik slot ──────────────────────────────────────────
    void OnClickSlot(int index)
    {
        selectedSlotIndex = index;

        if (!SaveManager.Instance.SlotExists(index))
        {
            // Slot kosong → langsung save baru
            SaveManager.Instance.Save(index);
            RefreshSlotList();
            return;
        }

        // Slot ada isi → tampil confirm panel
        SaveData meta = SaveManager.Instance.ReadSlotMeta(index);
        if (confirmTitleText)
            confirmTitleText.text = $"{meta.slotName}\n{meta.saveDateTime}\nLv.{meta.level}  |  {meta.coins} koin";

        btnLoad.gameObject.SetActive(true);
        btnOverwrite.gameObject.SetActive(true);
        btnDelete.gameObject.SetActive(true);

        confirmPanel.SetActive(true);
    }

    // ── Aksi tombol ────────────────────────────────────────
    void OnClickLoad()
    {
        if (selectedSlotIndex < 0) return;
        SaveManager.Instance.Load(selectedSlotIndex);
        confirmPanel.SetActive(false);

        MenuController menu = FindObjectOfType<MenuController>();
        if (menu != null) menu.menuCanvas.SetActive(false);

        MobileInput.Instance?.SetMobileUIVisible(true);

    }

    void OnClickOverwrite()
    {
        if (selectedSlotIndex < 0) return;
        SaveData meta = SaveManager.Instance.ReadSlotMeta(selectedSlotIndex);
        SaveManager.Instance.Save(selectedSlotIndex, meta.slotName);
        RefreshSlotList();
    }

    void OnClickDelete()
    {
        if (selectedSlotIndex < 0) return;
        SaveManager.Instance.DeleteSlot(selectedSlotIndex);
        RefreshSlotList();
    }
}