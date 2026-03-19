using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public QuestData quest;
    public string npcID = "";
    public int ongoingDialogueIndex = 5;
    public int turnInDialogueIndex = 5;


    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public bool HasQuest()
    {
        if (quest == null) return false;
        return !QuestManager.Instance.IsTurnedIn(quest);
    }

    public void TryAccept()
    {
        if (QuestManager.Instance.IsActive(quest) ||
            QuestManager.Instance.IsTurnedIn(quest)) return;
        QuestManager.Instance.AcceptQuest(quest);
    }

    public void TryTurnIn()
    {
        if (!QuestManager.Instance.IsCompleted(quest)) return;
        QuestManager.Instance.TryTurnIn(quest, player);
    }

    public bool IsCompleted() => QuestManager.Instance.IsCompleted(quest);
    public bool IsActive() => QuestManager.Instance.IsActive(quest);
    public bool IsTurnedIn() => QuestManager.Instance.IsTurnedIn(quest);
}