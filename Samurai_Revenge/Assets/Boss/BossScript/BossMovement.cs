using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float stopDistance = 1.5f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void MoveTowardsPlayer()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= stopDistance)
        {
            StopMoving();
            return;
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;

        animator?.SetFloat("MoveX", direction.x);
        animator?.SetFloat("MoveY", direction.y);
        animator?.SetFloat("MoveMagnitude", 1f);
    }

    public void StopMoving()
    {
        rb.velocity = Vector2.zero;
        animator?.SetFloat("MoveX", 0);
        animator?.SetFloat("MoveY", 0);
        animator?.SetFloat("MoveMagnitude", 0);
    }
}