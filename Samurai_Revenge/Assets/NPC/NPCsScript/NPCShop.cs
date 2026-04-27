using UnityEngine;

public class NPCShop : MonoBehaviour
{
    public ShopData shopData;

    public void OpenShop()
    {
        ShopUI.Instance?.OpenShop(shopData);
    }
}