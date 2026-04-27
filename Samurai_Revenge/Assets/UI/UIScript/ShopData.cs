using UnityEngine;

[CreateAssetMenu(fileName = "NewShop", menuName = "Shop/ShopData")]
public class ShopData : ScriptableObject
{
    public string shopName = "Toko";
    public ShopItem[] items;
}

[System.Serializable]
public class ShopItem
{
    public ItemData itemData;
    public int stock = -1;
}