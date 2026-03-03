using UnityEngine;

public class Shuriken : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DummyEnemy enemy = collision.GetComponent<DummyEnemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(this.gameObject);
        }
    }
}