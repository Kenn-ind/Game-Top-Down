using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private Dictionary<QuestData, int> activeQuests = new Dictionary<QuestData, int>();
    private HashSet<QuestData> completedQuests = new HashSet<QuestData>();
    private HashSet<QuestData> turnedInQuests = new HashSet<QuestData>();

    public Dictionary<QuestData, int> GetActiveQuests() => activeQuests;

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

    public void AcceptQuest(QuestData quest)
    {
        if (IsActive(quest) || IsCompleted(quest) || IsTurnedIn(quest)) return;
        activeQuests[quest] = 0;
        Debug.Log($"Quest diterima: {quest.questName}");
        QuestUI.Instance?.ShowTracker(quest, 0);
    }

    public void ReportKill(string enemyID) => Report(QuestType.Kill, enemyID);
    public void ReportCollect(string itemID) => Report(QuestType.Collect, itemID);
    public void ReportReach(string zoneID) => Report(QuestType.Reach, zoneID);
    public void ReportTalk(string npcID) => Report(QuestType.Talk, npcID);

    void Report(QuestType type, string id)
    {
        Debug.Log($"Report dipanggil: type={type}, id={id}, activeQuests={activeQuests.Count}");

        List<QuestData> toUpdate = new List<QuestData>();

        foreach (var kv in activeQuests)
        {
            if (kv.Key.questType == type && kv.Key.targetID == id)
                toUpdate.Add(kv.Key);
        }

        List<QuestData> toComplete = new List<QuestData>();

        foreach (QuestData q in toUpdate)
        {
            activeQuests[q]++;
            Debug.Log($"Progress {q.questName}: {activeQuests[q]}/{q.requiredAmount}");
            QuestUI.Instance?.ShowTracker(q, activeQuests[q]);

            if (activeQuests[q] >= q.requiredAmount)
                toComplete.Add(q);
        }

        foreach (QuestData q in toComplete)
            CompleteQuest(q);
    }

    void CompleteQuest(QuestData quest)
    {
        activeQuests.Remove(quest);
        completedQuests.Add(quest);
        Debug.Log($"Quest selesai: {quest.questName}");
        QuestUI.Instance?.HideTracker(quest);
        QuestUI.Instance?.ShowCompleteNotif(quest.questName);
    }

    public bool TryTurnIn(QuestData quest, Transform playerTransform, InventoryController inventory)
    {
        if (!completedQuests.Contains(quest)) return false;
        if (turnedInQuests.Contains(quest)) return false;

        if (quest.questType == QuestType.Collect && inventory != null)
        {
            bool removed = inventory.RemoveItemByID(quest.targetID, quest.requiredAmount);
            if (!removed)
            {
                Debug.Log("Item tidak cukup di inventory!");
                return false;
            }
        }

        if (quest.rewardItemPrefab != null)
        {
            for (int i = 0; i < quest.rewardItemAmount; i++)
                Instantiate(quest.rewardItemPrefab, playerTransform.position, Quaternion.identity);
        }

        turnedInQuests.Add(quest);
        Debug.Log($"Reward diberikan: {quest.questName}");
        return true;
    }
}