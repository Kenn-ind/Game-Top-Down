using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    public int startingCoins = 100;

    private int currentCoins;

    public int CurrentCoins => currentCoins;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentCoins = startingCoins;
        ShopUI.Instance?.UpdateCoinDisplay(currentCoins);
    }

    public bool HasEnough(int amount) => currentCoins >= amount;

    public bool Spend(int amount)
    {
        if (!HasEnough(amount)) return false;
        currentCoins -= amount;
        ShopUI.Instance?.UpdateCoinDisplay(currentCoins);
        return true;
    }

    public void Earn(int amount)
    {
        currentCoins += amount;
        ShopUI.Instance?.UpdateCoinDisplay(currentCoins);
    }
}