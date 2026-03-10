using UnityEngine;

public class Shuriken : MonoBehaviour
{
    public int damage = 1;
    public float rotateSpeed = 720f;

    public GameObject hitParticle;

    void Update()
    {
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BaseEnemy enemy = collision.GetComponent<BaseEnemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);

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