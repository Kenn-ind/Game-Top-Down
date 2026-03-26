using UnityEngine;

public class Shuriken : MonoBehaviour
{
    public float rotateSpeed = 720f;

    public GameObject hitParticle;
    public PlayerStats stats;

    void Update()
    {
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BaseEnemy enemy = collision.GetComponent<BaseEnemy>();

        if (enemy != null && stats != null)
        {
            enemy.TakeDamage(stats.attackDamage, Vector2.zero, false);

            if (hitParticle != null)
            {
                GameObject particle = Instantiate(
                    hitParticle,
                    transform.position,
                    Quaternion.identity
                );

                Destroy(particle, 1f);
            }

            Destroy(gameObject);
        }
    }
}