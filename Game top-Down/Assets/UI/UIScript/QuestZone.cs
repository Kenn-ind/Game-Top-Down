using UnityEngine;

public class QuestZone : MonoBehaviour
{
    public string zoneID;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
            QuestManager.Instance?.ReportReach(zoneID);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, GetComponent<CircleCollider2D>()?.radius ?? 1f);
    }
}