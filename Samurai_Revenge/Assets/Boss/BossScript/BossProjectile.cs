using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public int damage = 10;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Jangan damage boss sendiri
        if (other.GetComponent<BossHealth>() != null) return;

        PlayerHealth ph = other.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Wall"))
            Destroy(gameObject);
    }
}