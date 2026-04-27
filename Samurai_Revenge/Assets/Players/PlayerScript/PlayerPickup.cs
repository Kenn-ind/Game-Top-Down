using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRadius = 1f;
    public InventoryController inventory;
    public LayerMask itemLayer;

    void Update()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, pickupRadius, itemLayer
        );
        foreach (Collider2D hit in hits)
        {
            WorldItem worldItem = hit.GetComponent<WorldItem>();
            if (worldItem != null)
            {
                bool picked = inventory.AddItem(worldItem.itemData, worldItem.stackCount);
                if (picked)
                {
                    ItemPickupPopup.Instance?.Show(worldItem.itemData, worldItem.stackCount);

                    Destroy(hit.gameObject);
                    QuestManager.Instance?.ReportCollect(worldItem.itemData.itemID);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}