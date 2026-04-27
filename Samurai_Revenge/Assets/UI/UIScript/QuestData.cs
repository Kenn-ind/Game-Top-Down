using UnityEngine;

public enum QuestType { Kill, Collect, Reach, Talk }

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest/QuestData")]
public class QuestData : ScriptableObject
{
    public string questName = "Quest Baru";
    [TextArea] public string description = "";
    public QuestType questType;
    public string targetID = "";
    public int requiredAmount = 1;
    public GameObject rewardItemPrefab;
    public int rewardItemAmount = 1;
}