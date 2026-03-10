using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    public int damage = 1;
    private Transform player;

    void Start()
    {
        player = transform.root;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BaseEnemy enemy = collision.GetComponent<BaseEnemy>();

        if (enemy != null)
        {
            Vector2 direction = (collision.transform.position - player.position).normalized;

            enemy.TakeDamage(damage, direction, true);
        }
    }
}