using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public static QuestUI Instance;

    public GameObject trackerPanel;
    public TMP_Text trackerText;

    public GameObject completePanel;
    public TMP_Text completeText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        trackerPanel?.SetActive(false);
        completePanel?.SetActive(false);
    }

    public void ShowTracker(QuestData quest, int current)
    {
        trackerText.text = $"{quest.questName}\n{current} / {quest.requiredAmount}";
        trackerPanel.SetActive(true);
    }

    public void ShowCompleteNotif(string questName)
    {
        completeText.text = $"Quest Selesai!\n{questName}";
        completePanel.SetActive(true);
        StartCoroutine(HideAfter(completePanel, 1.5f));
    }

    IEnumerator HideAfter(GameObject panel, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        panel.SetActive(false);
    }


    public void HideTracker(QuestData completedQuest)
    {
        Dictionary<QuestData, int> activeQuests = QuestManager.Instance.GetActiveQuests();

        foreach (var kv in activeQuests)
        {
            if (kv.Key != completedQuest)
            {
                trackerText.text = $"{kv.Key.questName}\n{kv.Value} / {kv.Key.requiredAmount}";
                return;
            }
        }

        trackerPanel.SetActive(false);
    }
}