using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public static QuestUI Instance;

    [Header("Tracker HUD")]
    public GameObject trackerPanel;
    public TMP_Text trackerText;
    public Button trackerButton;   // Button di TrackerPanel untuk buka popup

    [Header("Complete Notif")]
    public GameObject completePanel;
    public TMP_Text completeText;

    [Header("Quest Select Popup")]
    public GameObject questSelectPanel;
    public Transform questSelectContent;
    public GameObject questSelectButtonPrefab;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        trackerPanel?.SetActive(false);
        completePanel?.SetActive(false);
        questSelectPanel?.SetActive(false);

        trackerButton?.onClick.AddListener(ToggleQuestSelectPanel);
    }

    // Dipanggil QuestManager setiap ada perubahan
    public void RefreshTracker()
    {
        QuestData tracked = QuestManager.Instance.GetTrackedQuest();
        var active = QuestManager.Instance.GetActiveQuests();

        if (tracked != null && active.ContainsKey(tracked))
        {
            int cur = active[tracked];
            string arrow = active.Count > 1 ? " \u25BC" : ""; // ▼ jika >1 quest
            trackerText.text = $"{tracked.questName}{arrow}\n{cur} / {tracked.requiredAmount}";
            trackerPanel.SetActive(true);
        }
        else if (active.Count > 0)
        {
            foreach (var kv in active)
            { QuestManager.Instance.SetTrackedQuest(kv.Key); return; }
        }
        else
        {
            trackerPanel.SetActive(false);
        }
    }

    public void ToggleQuestSelectPanel()
    {
        var active = QuestManager.Instance.GetActiveQuests();
        if (active.Count <= 1) { questSelectPanel?.SetActive(false); return; }

        bool isOpen = questSelectPanel.activeSelf;
        questSelectPanel.SetActive(!isOpen);
        if (!isOpen) RebuildSelectPanel(active);
    }

    void RebuildSelectPanel(Dictionary<QuestData, int> active)
    {
        foreach (Transform child in questSelectContent) Destroy(child.gameObject);

        QuestData tracked = QuestManager.Instance.GetTrackedQuest();
        foreach (var kv in active)
        {
            QuestData quest = kv.Key;
            int cur = kv.Value;
            GameObject btn = Instantiate(questSelectButtonPrefab, questSelectContent);
            TMP_Text label = btn.GetComponentInChildren<TMP_Text>();
            bool isTracked = quest == tracked;
            label.text = $"{(isTracked ? "\u25CF " : "\u25CB ")}{quest.questName}  {cur}/{quest.requiredAmount}";
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                QuestManager.Instance.SetTrackedQuest(quest);
                questSelectPanel.SetActive(false);
                RefreshTracker();
            });
        }
    }

    public void ShowCompleteNotif(string questName)
    {
        completeText.text = $"Quest Selesai!\n{questName}";
        completePanel.SetActive(true);
        StartCoroutine(HideAfter(completePanel, 2.5f));
    }

    IEnumerator HideAfter(GameObject panel, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        panel.SetActive(false);
    }

    // Backward compat
    public void ShowTracker(QuestData quest, int current) => RefreshTracker();
    public void HideTracker(QuestData completedQuest) => RefreshTracker();
}
