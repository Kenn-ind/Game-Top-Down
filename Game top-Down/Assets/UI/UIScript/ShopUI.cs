using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;

    public GameObject shopPanel;

    public TMP_Text shopNameText;
    public TMP_Text coinText;

    public Transform buyContainer;
    public GameObject shopItemSlotPrefab;

    public Transform sellContainer;
    public GameObject sellItemSlotPrefab;

    public GameObject confirmPanel;
    public TMP_Text confirmText;
    public Button confirmYesButton;
    public Button confirmNoButton;

    private ShopData currentShop;
    private List<GameObject> spawnedBuySlots = new List<GameObject>();
    private List<GameObject> spawnedSellSlots = new List<GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        shopPanel?.SetActive(false);
        confirmPanel?.SetActive(false);
    }

    public void OpenShop(ShopData shop)
    {
        currentShop = shop;
        shopPanel.SetActive(true);
        shopNameText.text = shop.shopName;
        UpdateCoinDisplay(CoinManager.Instance.CurrentCoins);
        RefreshBuyList();
        RefreshSellList();
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        confirmPanel.SetActive(false);
    }

    public void UpdateCoinDisplay(int amount)
    {
        if (coinText != null)
            coinText.text = $"Coin: {amount}";
    }

    void RefreshBuyList()
    {
        foreach (GameObject obj in spawnedBuySlots)
            Destroy(obj);
        spawnedBuySlots.Clear();

        foreach (ShopItem shopItem in currentShop.items)
        {
            if (shopItem.itemData == null) continue;
            if (shopItem.stock == 0) continue;

            GameObject slot = Instantiate(shopItemSlotPrefab, buyContainer);
            spawnedBuySlots.Add(slot);

            slot.transform.Find("Icon").GetComponent<Image>().sprite = shopItem.itemData.icon;

            slot.transform.Find("ItemName").GetComponent<TMP_Text>().text = shopItem.itemData.itemName;

            TMP_Text priceText = slot.transform.Find("Price").GetComponent<TMP_Text>();
            if (shopItem.itemData.isFree)
                priceText.text = "Gratis";
            else
                priceText.text = $"{shopItem.itemData.price} Coin";

            TMP_Text stockText = slot.transform.Find("Stock").GetComponent<TMP_Text>();
            stockText.text = shopItem.stock == -1 ? "Stok: ~" : $"Stok: {shopItem.stock}";

            Button buyBtn = slot.transform.Find("BuyButton").GetComponent<Button>();
            ShopItem captured = shopItem;
            buyBtn.onClick.AddListener(() => OnClickBuy(captured));
        }
    }

    void RefreshSellList()
    {
        foreach (GameObject obj in spawnedSellSlots)
            Destroy(obj);
        spawnedSellSlots.Clear();

        InventoryController inventory = FindObjectOfType<InventoryController>();
        if (inventory == null) return;

        foreach (Slot slot in inventory.GetSlots())
        {
            if (slot.currentItem == null) continue;

            ItemUI itemUI = slot.currentItem.GetComponent<ItemUI>();
            if (itemUI == null || itemUI.itemData == null) continue;
            if (itemUI.itemData.sellPrice <= 0) continue; // tidak bisa dijual

            GameObject sellSlot = Instantiate(sellItemSlotPrefab, sellContainer);
            spawnedSellSlots.Add(sellSlot);

            sellSlot.transform.Find("Icon").GetComponent<Image>().sprite = itemUI.itemData.icon;
            sellSlot.transform.Find("ItemName").GetComponent<TMP_Text>().text = itemUI.itemData.itemName;
            sellSlot.transform.Find("Price").GetComponent<TMP_Text>().text = $"{itemUI.itemData.sellPrice} Coin";
            sellSlot.transform.Find("Stack").GetComponent<TMP_Text>().text = $"x{itemUI.stackCount}";

            Button sellBtn = sellSlot.transform.Find("SellButton").GetComponent<Button>();
            ItemUI capturedUI = itemUI;
            Slot capturedSlot = slot;
            sellBtn.onClick.AddListener(() => OnClickSell(capturedUI, capturedSlot));
        }
    }

    void OnClickBuy(ShopItem shopItem)
    {
        string priceInfo = shopItem.itemData.isFree ? "gratis" : $"seharga {shopItem.itemData.price} Coin";
        ShowConfirm(
            $"Beli {shopItem.itemData.itemName} {priceInfo}?",
            () => ExecuteBuy(shopItem)
        );
    }

    void OnClickSell(ItemUI itemUI, Slot slot)
    {
        ShowConfirm(
            $"Jual {itemUI.itemData.itemName} x{itemUI.stackCount} seharga {itemUI.itemData.sellPrice * itemUI.stackCount} Coin?",
            () => ExecuteSell(itemUI, slot)
        );
    }

    void ExecuteBuy(ShopItem shopItem)
    {
        InventoryController inventory = FindObjectOfType<InventoryController>();
        if (inventory == null) return;

        if (!shopItem.itemData.isFree)
        {
            if (!CoinManager.Instance.HasEnough(shopItem.itemData.price))
            {
                Debug.Log("Coin tidak cukup!");
                confirmPanel.SetActive(false);
                return;
            }
            CoinManager.Instance.Spend(shopItem.itemData.price);
        }

        bool added = inventory.AddItem(shopItem.itemData, 1);
        if (!added)
        {
            if (!shopItem.itemData.isFree)
                CoinManager.Instance.Earn(shopItem.itemData.price);
            Debug.Log("Inventory penuh!");
        }
        else
        {
            if (shopItem.stock > 0) shopItem.stock--;
        }

        confirmPanel.SetActive(false);
        RefreshBuyList();
        RefreshSellList();
    }

    void ExecuteSell(ItemUI itemUI, Slot slot)
    {
        int totalEarn = itemUI.itemData.sellPrice * itemUI.stackCount;
        CoinManager.Instance.Earn(totalEarn);

        Destroy(slot.currentItem);
        slot.currentItem = null;

        confirmPanel.SetActive(false);
        RefreshSellList();
    }

    void ShowConfirm(string message, System.Action onConfirm)
    {
        confirmText.text = message;
        confirmPanel.SetActive(true);

        confirmYesButton.onClick.RemoveAllListeners();
        confirmYesButton.onClick.AddListener(() => onConfirm?.Invoke());

        confirmNoButton.onClick.RemoveAllListeners();
        confirmNoButton.onClick.AddListener(() => confirmPanel.SetActive(false));
    }
}