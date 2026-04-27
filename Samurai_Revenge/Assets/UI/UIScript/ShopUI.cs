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

    public GameObject quantityRow;
    public TMP_Text quantityAmountText;
    public TMP_Text quantityPriceText;
    public Button quantityMinusButton;
    public Button quantityPlusButton;

    private ShopData currentShop;
    private List<GameObject> spawnedBuySlots = new List<GameObject>();
    private List<GameObject> spawnedSellSlots = new List<GameObject>();

    private ItemUI currentSellItemUI;
    private Slot currentSellSlot;
    private int currentSellAmount = 1;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        shopPanel?.SetActive(false);
        confirmPanel?.SetActive(false);
    }

    void Start()
    {
        quantityMinusButton?.onClick.AddListener(OnMinusClick);
        quantityPlusButton?.onClick.AddListener(OnPlusClick);
    }

    public void OpenShop(ShopData shop)
    {
        currentShop = shop;

        Debug.Log($"shopPanel: {shopPanel != null}");
        Debug.Log($"shopNameText: {shopNameText != null}");
        Debug.Log($"coinText: {coinText != null}");
        Debug.Log($"buyContainer: {buyContainer != null}");
        Debug.Log($"shopItemSlotPrefab: {shopItemSlotPrefab != null}");

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
            priceText.text = shopItem.itemData.isFree ? "Gratis" : $"{shopItem.itemData.price} Coin";

            slot.transform.Find("Stock").GetComponent<TMP_Text>().text =
                shopItem.stock == -1 ? "~" : $"{shopItem.stock}";

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
            if (itemUI.itemData.sellPrice <= 0) continue;

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
        // Buy tidak pakai quantity row
        quantityRow?.SetActive(false);
        quantityPriceText?.gameObject.SetActive(false);

        string priceInfo = shopItem.itemData.isFree ? "gratis" : $"seharga {shopItem.itemData.price} Coin";
        ShowConfirm(
            $"Beli {shopItem.itemData.itemName} {priceInfo}?",
            () => ExecuteBuy(shopItem)
        );
    }

    void OnClickSell(ItemUI itemUI, Slot slot)
    {
        currentSellItemUI = itemUI;
        currentSellSlot = slot;
        currentSellAmount = 1;

        // Sell pakai quantity row
        quantityRow?.SetActive(true);
        quantityPriceText?.gameObject.SetActive(true);
        quantityAmountText.text = "1";
        UpdateQuantityPrice();
        UpdateQuantityButtons();

        ShowConfirm(
            $"Jual {itemUI.itemData.itemName}?",
            () => ExecuteSell()
        );
    }

    void OnMinusClick()
    {
        if (currentSellAmount <= 1) return;
        currentSellAmount--;
        quantityAmountText.text = currentSellAmount.ToString();
        UpdateQuantityPrice();
        UpdateQuantityButtons();
    }

    void OnPlusClick()
    {
        if (currentSellItemUI == null) return;
        if (currentSellAmount >= currentSellItemUI.stackCount) return;
        currentSellAmount++;
        quantityAmountText.text = currentSellAmount.ToString();
        UpdateQuantityPrice();
        UpdateQuantityButtons();
    }

    void UpdateQuantityPrice()
    {
        if (currentSellItemUI == null || quantityPriceText == null) return;
        int total = currentSellItemUI.itemData.sellPrice * currentSellAmount;
        quantityPriceText.text = $"Total: {total} Coin";
    }

    void UpdateQuantityButtons()
    {
        if (currentSellItemUI == null) return;
        quantityMinusButton.interactable = currentSellAmount > 1;
        quantityPlusButton.interactable = currentSellAmount < currentSellItemUI.stackCount;
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

    void ExecuteSell()
    {
        if (currentSellItemUI == null) return;

        int totalEarn = currentSellItemUI.itemData.sellPrice * currentSellAmount;
        CoinManager.Instance.Earn(totalEarn);

        currentSellItemUI.stackCount -= currentSellAmount;

        if (currentSellItemUI.stackCount <= 0)
        {
            Destroy(currentSellSlot.currentItem);
            currentSellSlot.currentItem = null;
        }
        else
        {
            currentSellItemUI.UpdateUI();
        }

        currentSellItemUI = null;
        currentSellSlot = null;
        confirmPanel.SetActive(false);
        RefreshSellList();
        UpdateCoinDisplay(CoinManager.Instance.CurrentCoins);
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