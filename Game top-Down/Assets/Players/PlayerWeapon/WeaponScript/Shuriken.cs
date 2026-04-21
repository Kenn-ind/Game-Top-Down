using UnityEngine;

public class Shuriken : MonoBehaviour
{
    public float rotateSpeed = 720f;
    public GameObject hitParticle;
    public PlayerStats stats;
    public SkillUpgradeData upgradeData;

    void Update()
    {
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BaseEnemy enemy = collision.GetComponent<BaseEnemy>();
        if (enemy != null)
        {
            int damage = stats != null ? stats.attackDamage : 1;
            enemy.TakeDamage(stats.attackDamage + (upgradeData != null ? upgradeData.meleeDamageBonus : 0), Vector2.zero, false);

            if (hitParticle != null)
            {
                GameObject particle = Instantiate(hitParticle, transform.position, Quaternion.identity);
                Destroy(particle, 1f);
            }

            Destroy(gameObject);
        }
    }
}