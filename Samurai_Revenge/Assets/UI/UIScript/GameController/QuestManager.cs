using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private Dictionary<QuestData, int> activeQuests = new Dictionary<QuestData, int>();
    private HashSet<QuestData> completedQuests = new HashSet<QuestData>();
    private HashSet<QuestData> turnedInQuests = new HashSet<QuestData>();
    private QuestData trackedQuest = null;

    public Dictionary<QuestData, int> GetActiveQuests() => activeQuests;
    public HashSet<QuestData> GetCompletedQuests() => completedQuests;
    public HashSet<QuestData> GetTurnedInQuests() => turnedInQuests;
    public QuestData GetTrackedQuest() => trackedQuest;

    public QuestData[] allQuestDataList;


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool IsCompleted(QuestData quest) => completedQuests.Contains(quest);
    public bool IsActive(QuestData quest) => activeQuests.ContainsKey(quest);
    public bool IsTurnedIn(QuestData quest) => turnedInQuests.Contains(quest);

    public int GetProgress(QuestData quest)
    {
        return activeQuests.TryGetValue(quest, out int val) ? val : 0;
    }

    public void SetTrackedQuest(QuestData quest)
    {
        trackedQuest = quest;
        QuestUI.Instance?.RefreshTracker();
        QuestLogUI.Instance?.RefreshLog();
    }

    void OpenQuestTab()
    {
        MenuController menu = FindObjectOfType<MenuController>();
        if (menu != null && !menu.menuCanvas.activeSelf)
            menu.menuCanvas.SetActive(true);

        TabController tab = FindObjectOfType<TabController>();
        tab?.ActivateTab(3); // index 3 = QuestTab
    }

    public void AcceptQuest(QuestData quest, InventoryController inventory = null)
    {
        if (IsActive(quest) || IsCompleted(quest) || IsTurnedIn(quest)) return;

        activeQuests[quest] = 0;
        Debug.Log($"Quest diterima: {quest.questName}");

        if (quest.questType == QuestType.Collect && inventory != null)
        {
            int existingCount = 0;
            foreach (Slot slot in inventory.GetSlots())
            {
                if (slot.currentItem == null) continue;
                ItemUI itemUI = slot.currentItem.GetComponent<ItemUI>();
                if (itemUI != null && itemUI.itemData.itemID == quest.targetID)
                    existingCount += itemUI.stackCount;
            }
            if (existingCount > 0)
            {
                int progress = Mathf.Min(existingCount, quest.requiredAmount);
                activeQuests[quest] = progress;
                if (progress >= quest.requiredAmount)
                {
                    SetTrackedQuest(quest);
                    OpenQuestTab();
                    CompleteQuest(quest);
                    return;
                }
            }
        }

        SetTrackedQuest(quest);
        OpenQuestTab();
        QuestUI.Instance?.RefreshTracker();
    }

    public void ReportKill(string enemyID) => Report(QuestType.Kill, enemyID);
    public void ReportCollect(string itemID) => Report(QuestType.Collect, itemID);
    public void ReportReach(string zoneID) => Report(QuestType.Reach, zoneID);
    public void ReportTalk(string npcID) => Report(QuestType.Talk, npcID);

    void Report(QuestType type, string id)
    {
        List<QuestData> toUpdate = new List<QuestData>();
        foreach (var kv in activeQuests)
            if (kv.Key.questType == type && kv.Key.targetID == id)
                toUpdate.Add(kv.Key);

        List<QuestData> toComplete = new List<QuestData>();
        foreach (QuestData q in toUpdate)
        {
            activeQuests[q] = Mathf.Min(activeQuests[q] + 1, q.requiredAmount);
            if (q == trackedQuest) QuestUI.Instance?.RefreshTracker();
            QuestLogUI.Instance?.RefreshLog();
            if (activeQuests[q] >= q.requiredAmount) toComplete.Add(q);
        }
        foreach (QuestData q in toComplete) CompleteQuest(q);
    }

    void CompleteQuest(QuestData quest)
    {
        activeQuests.Remove(quest);
        completedQuests.Add(quest);
        Debug.Log($"Quest selesai: {quest.questName}");

        if (trackedQuest == quest)
        {
            trackedQuest = null;
            foreach (var kv in activeQuests) { trackedQuest = kv.Key; break; }
        }

        QuestUI.Instance?.RefreshTracker();
        QuestUI.Instance?.ShowCompleteNotif(quest.questName);
        QuestLogUI.Instance?.RefreshLog();
    }

    public bool TryTurnIn(QuestData quest, Transform playerTransform, InventoryController inventory)
    {
        if (!completedQuests.Contains(quest)) return false;
        if (turnedInQuests.Contains(quest)) return false;

        if (quest.questType == QuestType.Collect && inventory != null)
        {
            bool removed = inventory.RemoveItemByID(quest.targetID, quest.requiredAmount);
            if (!removed) { Debug.Log("Item tidak cukup!"); return false; }
        }

        if (quest.rewardItemPrefab != null)
        {
            WorldItem worldItem = quest.rewardItemPrefab.GetComponent<WorldItem>();
            if (worldItem != null && inventory != null)
            {
                inventory.AddItem(worldItem.itemData, quest.rewardItemAmount);
                ItemPickupPopup.Instance?.Show(worldItem.itemData, quest.rewardItemAmount);
            }
            else
            {
                for (int i = 0; i < quest.rewardItemAmount; i++)
                    Instantiate(quest.rewardItemPrefab, playerTransform.position, Quaternion.identity);
            }
        }

        turnedInQuests.Add(quest);
        QuestLogUI.Instance?.RefreshLog();
        Debug.Log($"Reward diberikan: {quest.questName}");
        return true;
    }

    public void LoadQuests(
        List<QuestSaveData> savedActive,
        List<string> savedCompleted,
        List<string> savedTurnedIn,
        string savedTrackedName)
    {
        activeQuests.Clear();
        completedQuests.Clear();
        turnedInQuests.Clear();
        trackedQuest = null;

        foreach (QuestSaveData qs in savedActive)
        {
            QuestData found = FindQuestByName(qs.questName);
            if (found != null) activeQuests[found] = qs.progress;
        }
        foreach (string name in savedCompleted)
        {
            QuestData found = FindQuestByName(name);
            if (found != null) completedQuests.Add(found);
        }
        foreach (string name in savedTurnedIn)
        {
            QuestData found = FindQuestByName(name);
            if (found != null) turnedInQuests.Add(found);
        }
        if (!string.IsNullOrEmpty(savedTrackedName))
            trackedQuest = FindQuestByName(savedTrackedName);

        QuestUI.Instance?.RefreshTracker();
        QuestLogUI.Instance?.RefreshLog();
    }

    QuestData FindQuestByName(string name)
    {
        foreach (QuestData q in allQuestDataList)
            if (q != null && q.questName == name) return q;
        Debug.LogWarning($"[QuestManager] QuestData '{name}' tidak ditemukan!");
        return null;
    }
}
