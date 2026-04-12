using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int maxStack = 5;
    public string itemID = "";
    public bool isFree = false;
    public int price = 0;
    public int sellPrice = 0;

    [Header("Penggunaan Item (IUsable)")]
    [Tooltip("Isi dengan Prefab item yang punya script IUsable (misal: HealthPotionItem).\n" +
             "Jika kosong = item tidak bisa digunakan (misal: bahan crafting, key item).")]
    public GameObject itemPrefab;
}