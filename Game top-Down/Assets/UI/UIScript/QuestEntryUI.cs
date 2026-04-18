using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestEntryUI : MonoBehaviour
{
    public enum QuestStatus { Active, Completed, TurnedIn }

    [Header("UI References")]
    public TMP_Text questNameText;
    public TMP_Text progressText;
    public TMP_Text statusText;
    public Button trackButton;
    public Image backgroundImage;

    [Header("Status Colors")]
    public Color activeColor = new Color(1f, 0.85f, 0.2f);
    public Color completedColor = new Color(0.3f, 0.9f, 0.4f);
    public Color turnedInColor = new Color(0.6f, 0.6f, 0.6f);

    private QuestData questData;

    public void Setup(QuestData quest, int progress, QuestStatus status)
    {
        questData = quest;
        questNameText.text = quest.questName;
        progressText.text = $"{progress} / {quest.requiredAmount}";

        switch (status)
        {
            case QuestStatus.Active:
                statusText.text = "\u25CF Aktif";
                statusText.color = activeColor;
                trackButton.gameObject.SetActive(true);

                bool isTracked = QuestManager.Instance.GetTrackedQuest() == quest;
                trackButton.GetComponentInChildren<TMP_Text>().text =
                    isTracked ? "\u2713 Tracking" : "Track";

                trackButton.onClick.RemoveAllListeners();
                trackButton.onClick.AddListener(() =>
                {
                    QuestManager.Instance.SetTrackedQuest(questData);
                    QuestLogUI.Instance?.RefreshLog();
                });
                break;

            case QuestStatus.Completed:
                statusText.text = "\u2714 Selesai";
                statusText.color = completedColor;
                trackButton.gameObject.SetActive(false);
                break;

            case QuestStatus.TurnedIn:
                statusText.text = "\u2713 Dikumpulkan";
                statusText.color = turnedInColor;
                trackButton.gameObject.SetActive(false);
                break;
        }
    }
}
