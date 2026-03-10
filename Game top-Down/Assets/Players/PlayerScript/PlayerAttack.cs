using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject shurikenPrefab;
    public float shurikenSpeed = 10f;
    public int shurikenMax = 5;
    public float rangeRadius = 5f;

    public GameObject swordHitbox;
    public float meleeRadius = 2f;
    public float meleeDuration = 0.4f;

    public float attackDelay = 0.5f;

    private float nextAttackTime;
    private Animator animator;
    private movement playerMovement;

    private Queue<GameObject> shurikenQueue = new Queue<GameObject>();

    private bool isAttacking = false;

    public bool IsAttacking()
    {
        return isAttacking;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<movement>();

        if (swordHitbox != null)
            swordHitbox.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) &&
            Time.time >= nextAttackTime &&
            !isAttacking)
        {
            nextAttackTime = Time.time + attackDelay;
            HandleAttack();
        }
    }

    void HandleAttack()
    {
        GameObject meleeTarget = FindNearestEnemyInRadius(meleeRadius);

        if (meleeTarget != null)
        {
            MeleeAttack(meleeTarget);
            return;
        }

        GameObject rangeTarget = FindNearestEnemyInRadius(rangeRadius);

        if (rangeTarget != null)
        {
            RangeAttack(rangeTarget);
        }
    }

    void MeleeAttack(GameObject target)
    {
        isAttacking = true;

        Vector2 direction = (target.transform.position - transform.position).normalized;

        TriggerAttackMeleeAnimation(direction);
        PositionSwordHitbox(direction);

        StartCoroutine(MeleeRoutine());
    }

    IEnumerator MeleeRoutine()
    {
        if (swordHitbox != null)
        {
            swordHitbox.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            swordHitbox.SetActive(false);
        }

        yield return new WaitForSeconds(meleeDuration);

        isAttacking = false;
    }

    void RangeAttack(GameObject target)
    {
        isAttacking = true;

        Vector2 direction = (target.transform.position - transform.position).normalized;

        TriggerAttackRangeAnimation(direction);

        GameObject shuriken = Instantiate(
            shurikenPrefab,
            transform.position,
            Quaternion.identity
        );

        Rigidbody2D rb = shuriken.GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.velocity = direction * shurikenSpeed;

        shurikenQueue.Enqueue(shuriken);

        if (shurikenQueue.Count > shurikenMax)
        {
            GameObject oldest = shurikenQueue.Dequeue();
            if (oldest != null)
                Destroy(oldest);
        }

        StartCoroutine(RangeRoutine());
    }
    IEnumerator RangeRoutine()
    {
        yield return new WaitForSeconds(0.45f);

        isAttacking = false;
    }

    GameObject FindNearestEnemyInRadius(float radius)
    {
        DummyEnemy[] dummyEnemies = FindObjectsOfType<DummyEnemy>();
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (DummyEnemy enemy in dummyEnemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            if (distance < shortestDistance && distance <= radius)
            {
                shortestDistance = distance;
                nearestEnemy = enemy.gameObject;
            }
        }

        foreach (Enemy enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            if (distance < shortestDistance && distance <= radius)
            {
                shortestDistance = distance;
                nearestEnemy = enemy.gameObject;
            }
        }

        return nearestEnemy;
    }

    void PositionSwordHitbox(Vector2 direction)
    {
        float offset = 1f;
        Vector2 attackDir;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            attackDir = direction.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            attackDir = direction.y > 0 ? Vector2.up : Vector2.down;
        }

        swordHitbox.transform.position = (Vector2)transform.position + attackDir * offset;
    }

    void TriggerAttackMeleeAnimation(Vector2 direction)
    {
        animator.ResetTrigger("MeleeUp");
        animator.ResetTrigger("MeleeDown");
        animator.ResetTrigger("MeleeLeft");
        animator.ResetTrigger("MeleeRight");

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            animator.SetTrigger(direction.x > 0 ? "MeleeRight" : "MeleeLeft");
        else
            animator.SetTrigger(direction.y > 0 ? "MeleeUp" : "MeleeDown");
    }

    void TriggerAttackRangeAnimation(Vector2 direction)
    {
        animator.ResetTrigger("ShuUp");
        animator.ResetTrigger("ShuDown");
        animator.ResetTrigger("ShuLeft");
        animator.ResetTrigger("ShuRight");

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            animator.SetTrigger(direction.x > 0 ? "ShuRight" : "ShuLeft");
        else
            animator.SetTrigger(direction.y > 0 ? "ShuUp" : "ShuDown");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, meleeRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangeRadius);
    }
}