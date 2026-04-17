using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public QuestData quest;
    public string npcID = "";
    public int ongoingDialogueIndex = 5;
    public int turnInDialogueIndex = 5;

    private Transform player;
    private InventoryController inventory;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        inventory = FindObjectOfType<InventoryController>();
        Debug.Log($"Inventory found at Start: {inventory != null}");
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

        QuestManager.Instance.AcceptQuest(quest, inventory);
    }

    public void TryTurnIn()
    {
        if (!QuestManager.Instance.IsCompleted(quest)) return;

        Debug.Log($"Inventory found: {inventory != null}");
        Debug.Log($"targetID: {quest.targetID}, requiredAmount: {quest.requiredAmount}");

        if (inventory != null)
        {
            foreach (Slot slot in inventory.GetSlots())
            {
                if (slot.currentItem == null) continue;
                ItemUI itemUI = slot.currentItem.GetComponent<ItemUI>();
                if (itemUI != null)
                    Debug.Log($"Slot berisi: {itemUI.itemData.itemID} x{itemUI.stackCount}");
            }
        }

        QuestManager.Instance.TryTurnIn(quest, player, inventory);
    }

    public bool IsCompleted() => QuestManager.Instance.IsCompleted(quest);
    public bool IsActive() => QuestManager.Instance.IsActive(quest);
    public bool IsTurnedIn() => QuestManager.Instance.IsTurnedIn(quest);
}