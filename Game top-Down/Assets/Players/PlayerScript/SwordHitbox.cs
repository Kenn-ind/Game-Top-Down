using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    public int damage = 2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        DummyEnemy enemy = other.GetComponent<DummyEnemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }
}