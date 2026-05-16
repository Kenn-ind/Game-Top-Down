using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public int damage = 10;
    private Collider2D bossCollider;

    public void Init(Collider2D spawnerCollider)
    {
        bossCollider = spawnerCollider;
        if (bossCollider != null)
        {
            Collider2D myCollider = GetComponent<Collider2D>();
            if (myCollider != null)
                Physics2D.IgnoreCollision(myCollider, bossCollider);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (bossCollider != null && other == bossCollider) return;
        if (other.GetComponent<BossHealth>() != null) return;
        if (other.GetComponent<BossProjectile>() != null) return;

        PlayerHealth ph = other.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.TakeDamage(damage, Vector2.zero);
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Wall"))
            Destroy(gameObject);
    }
}