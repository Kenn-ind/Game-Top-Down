using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// ============================================================
//  SaveManager.cs
//  Core sistem save / load JSON dengan multiple slot.
//
//  CARA PAKAI:
//    - Attach ke GameObject kosong bernama "SaveManager"
//    - Pastikan GameObject ini DontDestroyOnLoad
//    - Panggil SaveManager.Instance.Save(slotIndex) untuk save
//    - Panggil SaveManager.Instance.Load(slotIndex) untuk load
// ============================================================

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    [Header("Referensi — drag dari Inspector")]
    public PlayerStats playerStats;
    public PlayerHealth playerHealth;
    public PlayerStamina playerStamina;
    public PlayerLevel playerLevel;
    public InventoryController inventory;
    public CoinManager coinManager;
    public QuestManager questManager;
    public SkillUpgradeManager skillUpgradeManager;
    public ScrollInventory scrollInventory;

    [Header("ItemData List (sama persis dengan InventoryController)")]
    public ItemData[] allItemDataList;

    [Header("Settings")]
    public int maxSlots = 5;

    // Chest di-find otomatis saat runtime
    private ChestController[] allChests;

    private string SaveFolder => Path.Combine(Application.persistentDataPath, "Saves");

    // ── Singleton ────────────────────────────────────────────
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        // Auto-find semua chest di scene
        allChests = FindObjectsOfType<ChestController>();
        Debug.Log($"[SaveManager] Ditemukan {allChests.Length} chest di scene.");
    }

    // ============================================================
    //  SAVE
    // ============================================================
    public void Save(int slotIndex, string slotName = "")
    {
        SaveData data = new SaveData();

        // ── Meta ────────────────────────────────────────────
        data.slotName = string.IsNullOrEmpty(slotName) ? $"Save {slotIndex + 1}" : slotName;
        data.saveDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        data.currentScene = SceneManager.GetActiveScene().name;

        // ── Player Stats ────────────────────────────────────
        data.level = playerStats.level;
        data.availablePoints = playerStats.availablePoints;
        data.maxHP = playerStats.maxHP;
        data.maxStamina = playerStats.maxStamina;
        data.armor = playerStats.armor;
        data.attackDamage = playerStats.attackDamage;
        data.currentHP = playerHealth.CurrentHealth;
        data.currentXP = playerLevel.CurrentXP;

        // ── Player Position ─────────────────────────────────
        Vector3 pos = playerStats.transform.position;
        data.playerX = pos.x;
        data.playerY = pos.y;

        // ── Economy ─────────────────────────────────────────
        data.coins = coinManager.CurrentCoins;
        data.scrollCount = scrollInventory.scrollCount;

        // ── Inventory ───────────────────────────────────────
        data.inventoryItems.Clear();
        Slot[] slots = inventory.GetSlots();
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].currentItem == null) continue;
            ItemUI itemUI = slots[i].currentItem.GetComponent<ItemUI>();
            if (itemUI == null || itemUI.itemData == null) continue;

            data.inventoryItems.Add(new ItemSaveData
            {
                slotIndex = i,
                itemID = itemUI.itemData.itemID,
                stackCount = itemUI.stackCount
            });
        }

        // ── Quest ───────────────────────────────────────────
        data.activeQuests.Clear();
        foreach (var kv in questManager.GetActiveQuests())
            data.activeQuests.Add(new QuestSaveData
            {
                questName = kv.Key.questName,
                progress = kv.Value
            });

        data.completedQuests.Clear();
        foreach (var q in questManager.GetCompletedQuests())
            data.completedQuests.Add(q.questName);

        data.turnedInQuests.Clear();
        foreach (var q in questManager.GetTurnedInQuests())
            data.turnedInQuests.Add(q.questName);

        data.trackedQuestName = questManager.GetTrackedQuest()?.questName ?? "";

        // ── Skill Upgrades ──────────────────────────────────
        SkillUpgradeData su = skillUpgradeManager.upgradeData;
        data.skillUpgrades = new SkillUpgradeSaveData
        {
            meleeDamageBonus = su.meleeDamageBonus,
            meleeDamageCount = su.meleeDamageCount,
            meleeDashCountBonus = su.meleeDashCountBonus,
            meleeDashCount = su.meleeDashCount,
            meleeCooldownCount = su.meleeCooldownCount,
            meleeStaminaCount = su.meleeStaminaCount,
            rangeDamageBonus = su.rangeDamageBonus,
            rangeDamageCount = su.rangeDamageCount,
            rangeShurikenBonus = su.rangeShurikenBonus,
            rangeShurikenCount = su.rangeShurikenCount,
            rangeCooldownCount = su.rangeCooldownCount,
            rangeStaminaCount = su.rangeStaminaCount,
            skill2MeleeDamageBonus = su.skill2MeleeDamageBonus,
            skill2MeleeDamageCount = su.skill2MeleeDamageCount,
            skill2MeleeCooldownCount = su.skill2MeleeCooldownCount,
            skill2MeleeStaminaCount = su.skill2MeleeStaminaCount,
            skill2RangeDamageBonus = su.skill2RangeDamageBonus,
            skill2RangeDamageCount = su.skill2RangeDamageCount,
            skill2RangeShurikenBonus = su.skill2RangeShurikenBonus,
            skill2RangeShurikenCount = su.skill2RangeShurikenCount,
            skill2RangeCooldownCount = su.skill2RangeCooldownCount,
            skill2RangeStaminaCount = su.skill2RangeStaminaCount,
            skill3ShurikenBonus = su.skill3ShurikenBonus,
            skill3ShurikenCount = su.skill3ShurikenCount,
            skill3BerserkerDurationBonus = su.skill3BerserkerDurationBonus,
            skill3BerserkerCount = su.skill3BerserkerCount,
            skill3LifestealBonus = su.skill3LifestealBonus,
            skill3LifestealCount = su.skill3LifestealCount,
            skill3CooldownCount = su.skill3CooldownCount,
            skill3StaminaCount = su.skill3StaminaCount
        };

        // ── Chest ───────────────────────────────────────────
        data.openedChestIDs.Clear();
        foreach (ChestController chest in allChests)
            if (chest != null && chest.IsOpened)
                data.openedChestIDs.Add(chest.gameObject.name);

        // ── Camera Boundary ─────────────────────────────────
        // ⚠️ HARUS sebelum WriteToFile agar tersimpan ke JSON!
        data.activeBoundaryName = MapTransisi.ActiveBoundary != null
            ? MapTransisi.ActiveBoundary.gameObject.name
            : "";

        // ── Tulis ke file ───────────────────────────────────
        WriteToFile(slotIndex, data);
        Debug.Log($"[SaveManager] Saved ke slot {slotIndex}: {GetFilePath(slotIndex)}");
        Debug.Log($"[SaveManager] Boundary tersimpan: '{data.activeBoundaryName}'");
    }

    // ============================================================
    //  LOAD
    // ============================================================
    public void Load(int slotIndex)
    {
        SaveData data = ReadFromFile(slotIndex);
        if (data == null)
        {
            Debug.LogWarning($"[SaveManager] Tidak ada save di slot {slotIndex}");
            return;
        }

        // ── Player Stats ────────────────────────────────────
        playerStats.level = data.level;
        playerStats.availablePoints = data.availablePoints;
        playerStats.maxHP = data.maxHP;
        playerStats.maxStamina = data.maxStamina;
        playerStats.armor = data.armor;
        playerStats.attackDamage = data.attackDamage;

        // ── Health & Stamina ─────────────────────────────────
        playerHealth.maxHealth = data.maxHP;
        playerHealth.LoadHealth(data.currentHP);
        playerStamina.maxStamina = data.maxStamina;
        playerStamina.LoadStamina(data.maxStamina);

        // ── XP ───────────────────────────────────────────────
        playerLevel.LoadXP(data.currentXP);

        // ── Player Position ─────────────────────────────────
        playerStats.transform.position = new Vector3(data.playerX, data.playerY, 0f);

        // ── Economy ─────────────────────────────────────────
        coinManager.LoadCoins(data.coins);
        scrollInventory.scrollCount = data.scrollCount;

        // ── Inventory ───────────────────────────────────────
        inventory.ClearAllSlots();
        foreach (ItemSaveData item in data.inventoryItems)
        {
            ItemData found = FindItemDataByID(item.itemID);
            if (found != null)
                inventory.AddItemToSlot(item.slotIndex, found, item.stackCount);
            else
                Debug.LogWarning($"[SaveManager] ItemData ID '{item.itemID}' tidak ditemukan!");
        }

        // ── Quest ───────────────────────────────────────────
        questManager.LoadQuests(data.activeQuests, data.completedQuests,
                                data.turnedInQuests, data.trackedQuestName);

        // ── Skill Upgrades ──────────────────────────────────
        SkillUpgradeSaveData su = data.skillUpgrades;
        SkillUpgradeData soData = skillUpgradeManager.upgradeData;
        soData.meleeDamageBonus = su.meleeDamageBonus;
        soData.meleeDamageCount = su.meleeDamageCount;
        soData.meleeDashCountBonus = su.meleeDashCountBonus;
        soData.meleeDashCount = su.meleeDashCount;
        soData.meleeCooldownCount = su.meleeCooldownCount;
        soData.meleeStaminaCount = su.meleeStaminaCount;
        soData.rangeDamageBonus = su.rangeDamageBonus;
        soData.rangeDamageCount = su.rangeDamageCount;
        soData.rangeShurikenBonus = su.rangeShurikenBonus;
        soData.rangeShurikenCount = su.rangeShurikenCount;
        soData.rangeCooldownCount = su.rangeCooldownCount;
        soData.rangeStaminaCount = su.rangeStaminaCount;
        soData.skill2MeleeDamageBonus = su.skill2MeleeDamageBonus;
        soData.skill2MeleeDamageCount = su.skill2MeleeDamageCount;
        soData.skill2MeleeCooldownCount = su.skill2MeleeCooldownCount;
        soData.skill2MeleeStaminaCount = su.skill2MeleeStaminaCount;
        soData.skill2RangeDamageBonus = su.skill2RangeDamageBonus;
        soData.skill2RangeDamageCount = su.skill2RangeDamageCount;
        soData.skill2RangeShurikenBonus = su.skill2RangeShurikenBonus;
        soData.skill2RangeShurikenCount = su.skill2RangeShurikenCount;
        soData.skill2RangeCooldownCount = su.skill2RangeCooldownCount;
        soData.skill2RangeStaminaCount = su.skill2RangeStaminaCount;
        soData.skill3ShurikenBonus = su.skill3ShurikenBonus;
        soData.skill3ShurikenCount = su.skill3ShurikenCount;
        soData.skill3BerserkerDurationBonus = su.skill3BerserkerDurationBonus;
        soData.skill3BerserkerCount = su.skill3BerserkerCount;
        soData.skill3LifestealBonus = su.skill3LifestealBonus;
        soData.skill3LifestealCount = su.skill3LifestealCount;
        soData.skill3CooldownCount = su.skill3CooldownCount;
        soData.skill3StaminaCount = su.skill3StaminaCount;

        foreach (ChestController chest in allChests)
        {
            if (chest == null) continue;
            bool wasOpened = data.openedChestIDs.Contains(chest.gameObject.name);
            chest.SetOpenedState(wasOpened);
        }

        skillUpgradeManager.ReApplyUpgrades();
        StartCoroutine(RefreshSkillUINextFrame());

        StartCoroutine(LoadCameraBoundary(data));
        Debug.Log($"[SaveManager] Load dari slot {slotIndex} berhasil!");
    }

    IEnumerator LoadCameraBoundary(SaveData data)
    {
        yield return null;

        if (string.IsNullOrEmpty(data.activeBoundaryName))
        {
            Debug.LogWarning("[SaveManager] activeBoundaryName kosong, skip camera boundary.");
            yield break;
        }

        Debug.Log($"[SaveManager] Mencari boundary: '{data.activeBoundaryName}'");

        PolygonCollider2D[] allBoundaries = FindObjectsOfType<PolygonCollider2D>();
        foreach (PolygonCollider2D boundary in allBoundaries)
        {
            if (boundary.gameObject.name == data.activeBoundaryName)
            {
                CinemachineConfiner confiner = FindObjectOfType<CinemachineConfiner>();
                CinemachineVirtualCamera vcam = FindObjectOfType<CinemachineVirtualCamera>();
                CinemachineBrain brain = Camera.main?.GetComponent<CinemachineBrain>();

                // 1. Matikan brain sementara
                if (brain != null) brain.enabled = false;

                // 2. Set boundary baru
                if (confiner != null)
                {
                    confiner.m_Damping = 0f;
                    confiner.m_BoundingShape2D = boundary;
                    confiner.InvalidatePathCache();
                }

                // 3. Snap posisi kamera & vcam ke player
                Vector3 targetPos = new Vector3(data.playerX, data.playerY,
                    Camera.main != null ? Camera.main.transform.position.z : -10f);

                if (Camera.main != null)
                    Camera.main.transform.position = targetPos;

                if (vcam != null)
                {
                    vcam.transform.position = targetPos;
                    vcam.PreviousStateIsValid = false;
                }

                yield return null;
                yield return null;

                // 4. Nyalakan brain lagi
                if (brain != null) brain.enabled = true;

                // 5. Kembalikan damping
                if (confiner != null)
                    confiner.m_Damping = 0.5f;

                Debug.Log($"[SaveManager] Camera boundary berhasil di-set ke: '{data.activeBoundaryName}'");
                yield break;
            }
        }

        Debug.LogWarning($"[SaveManager] Boundary '{data.activeBoundaryName}' tidak ditemukan di scene!");
    }

    IEnumerator RefreshSkillUINextFrame()
    {
        yield return null; // tunggu 1 frame

        SkillUpgradeUI skillUpgradeUI = FindObjectOfType<SkillUpgradeUI>(true); // true = cari yang inactive juga
        if (skillUpgradeUI != null)
        {
            skillUpgradeUI.gameObject.SetActive(true); // aktifkan sementara
            skillUpgradeUI.ForceRefresh();
            skillUpgradeUI.gameObject.SetActive(false); // matikan lagi
            Debug.Log("[SaveManager] SkillUpgradeUI refreshed!");
        }
        else
        {
            Debug.LogWarning("[SaveManager] SkillUpgradeUI tidak ditemukan!");
        }
    }

    // ============================================================
    //  DELETE SLOT
    // ============================================================
    public void DeleteSlot(int slotIndex)
    {
        string path = GetFilePath(slotIndex);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[SaveManager] Slot {slotIndex} dihapus.");
        }
    }

    // ============================================================
    //  CEK APAKAH SLOT ADA
    // ============================================================
    public bool SlotExists(int slotIndex) => File.Exists(GetFilePath(slotIndex));

    // ============================================================
    //  BACA METADATA
    // ============================================================
    public SaveData ReadSlotMeta(int slotIndex) => ReadFromFile(slotIndex);

    // ============================================================
    //  HELPER — File I/O
    // ============================================================
    string GetFilePath(int slotIndex)
    {
        if (!Directory.Exists(SaveFolder))
            Directory.CreateDirectory(SaveFolder);
        return Path.Combine(SaveFolder, $"save_slot_{slotIndex}.json");
    }

    void WriteToFile(int slotIndex, SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetFilePath(slotIndex), json);
    }

    SaveData ReadFromFile(int slotIndex)
    {
        string path = GetFilePath(slotIndex);
        if (!File.Exists(path)) return null;
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }

    ItemData FindItemDataByID(string id)
    {
        foreach (ItemData item in allItemDataList)
            if (item != null && item.itemID == id) return item;
        return null;
    }
}