using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestLogUI : MonoBehaviour
{
    public static QuestLogUI Instance;

    [Header("Scroll Content")]
    public Transform questListContent;       // Content di ScrollView
    public GameObject questEntryPrefab;       // Prefab tiap baris quest

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Dipanggil saat Quest Tab dibuka atau ada update
    public void RefreshLog()
    {
        foreach (Transform child in questListContent) Destroy(child.gameObject);

        var active = QuestManager.Instance.GetActiveQuests();
        var completed = QuestManager.Instance.GetCompletedQuests();
        var turnedIn = QuestManager.Instance.GetTurnedInQuests();

        foreach (var kv in active)
            SpawnEntry(kv.Key, kv.Value, QuestEntryUI.QuestStatus.Active);

        foreach (QuestData quest in completed)
            SpawnEntry(quest, quest.requiredAmount, QuestEntryUI.QuestStatus.Completed);

        foreach (QuestData quest in turnedIn)
            SpawnEntry(quest, quest.requiredAmount, QuestEntryUI.QuestStatus.TurnedIn);
    }

    void SpawnEntry(QuestData quest, int progress, QuestEntryUI.QuestStatus status)
    {
        GameObject go = Instantiate(questEntryPrefab, questListContent);
        go.GetComponent<QuestEntryUI>().Setup(quest, progress, status);
    }
}
