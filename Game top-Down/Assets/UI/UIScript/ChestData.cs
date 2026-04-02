using UnityEngine;

[CreateAssetMenu(fileName = "NewChest", menuName = "Inventory/Chest")]
public class ChestData : ScriptableObject
{
    [System.Serializable]
    public class ChestItem
    {
        public ItemData itemData;
        public int count = 1;
    }

    public ChestItem[] items;
}