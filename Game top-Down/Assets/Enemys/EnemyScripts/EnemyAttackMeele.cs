using UnityEngine;

public class EnemyAttackMeele : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2f;
    public float detectRange = 6f;

    public float attackRange = 1.5f;
    public int damage = 10;
    public float attackCooldown = 1.5f;
    public float knockbackForce = 8f;

    private float cooldownTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectRange)
        {
            if (distance > attackRange)
            {
                ChasePlayer();
            }
            else
            {
                TryAttack();
            }
        }

        cooldownTimer -= Time.deltaTime;
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }

    void TryAttack()
    {
        if (cooldownTimer <= 0)
        {
            Attack();
            cooldownTimer = attackCooldown;
        }
    }

    void Attack()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            playerHealth.TakeDamage(damage, direction * knockbackForce);
        }
    }
}