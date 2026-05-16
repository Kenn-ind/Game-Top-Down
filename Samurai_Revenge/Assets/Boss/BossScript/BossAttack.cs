using System.Collections;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [Header("Melee")]
    public GameObject swordHitbox;
    public float meleeRadius = 2f;
    public int meleeDamage = 15;

    [Header("Range")]
    public GameObject shurikenPrefab;
    public float shurikenSpeed = 8f;
    public float rangeRadius = 5f;
    public int rangeDamage = 10;

    [Header("Timing")]
    public float attackCooldown = 1.5f;

    private Animator animator;
    private bool isAttacking = false;
    public bool IsAttacking => isAttacking;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (swordHitbox != null)
            swordHitbox.SetActive(false);
    }

    public void TryAttack(Transform player)
    {
        if (isAttacking || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= meleeRadius)
            StartCoroutine(MeleeAttack(player));
        else if (distance <= rangeRadius)
            StartCoroutine(RangeAttack(player));
    }

    IEnumerator MeleeAttack(Transform player)
    {
        isAttacking = true;

        Vector2 direction = (player.position - transform.position).normalized;

        TriggerMeleeAnim(direction);
        PositionSwordHitbox(direction);

        if (swordHitbox != null)
        {
            swordHitbox.SetActive(true);

            Collider2D[] hits = Physics2D.OverlapCircleAll(
                swordHitbox.transform.position,
                0.5f
            );

            foreach (Collider2D hit in hits)
            {
                PlayerHealth ph = hit.GetComponent<PlayerHealth>();

                if (ph != null)
                {
                    Vector2 knockback =
                        (ph.transform.position - transform.position).normalized;

                    ph.TakeDamage(meleeDamage, knockback);
                }
            }

            yield return new WaitForSeconds(0.2f);

            swordHitbox.SetActive(false);
        }

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }

    IEnumerator RangeAttack(Transform player)
    {
        isAttacking = true;

        Vector2 direction = (player.position - transform.position).normalized;
        TriggerRangeAnim(direction);

        if (shurikenPrefab != null)
        {
            GameObject shuriken = Instantiate(
                shurikenPrefab, transform.position, Quaternion.identity);

            BossProjectile bp = shuriken.GetComponent<BossProjectile>();
            if (bp == null) bp = shuriken.AddComponent<BossProjectile>();
            bp.damage = rangeDamage;

            // Pass collider boss agar shuriken tidak langsung hit boss sendiri
            Collider2D bossCol = GetComponent<Collider2D>();
            bp.Init(bossCol);

            Rigidbody2D rb = shuriken.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = direction * shurikenSpeed;

            Destroy(shuriken, 3f);
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    void PositionSwordHitbox(Vector2 direction)
    {
        float offset = 1f;
        Vector2 attackDir;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            attackDir = direction.x > 0 ? Vector2.right : Vector2.left;
        else
            attackDir = direction.y > 0 ? Vector2.up : Vector2.down;

        swordHitbox.transform.position =
            (Vector2)transform.position + attackDir * offset;
    }

    void TriggerMeleeAnim(Vector2 direction)
    {
        animator?.ResetTrigger("MeleeUp");
        animator?.ResetTrigger("MeleeDown");
        animator?.ResetTrigger("MeleeLeft");
        animator?.ResetTrigger("MeleeRight");

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            animator?.SetTrigger(
                direction.x > 0 ? "MeleeRight" : "MeleeLeft"
            );
        else
            animator?.SetTrigger(
                direction.y > 0 ? "MeleeUp" : "MeleeDown"
            );
    }

    void TriggerRangeAnim(Vector2 direction)
    {
        animator?.ResetTrigger("ShuUp");
        animator?.ResetTrigger("ShuDown");
        animator?.ResetTrigger("ShuLeft");
        animator?.ResetTrigger("ShuRight");

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            animator?.SetTrigger(
                direction.x > 0 ? "ShuRight" : "ShuLeft"
            );
        else
            animator?.SetTrigger(
                direction.y > 0 ? "ShuUp" : "ShuDown"
            );
    }

    public void ResetState()
    {
        StopAllCoroutines();

        isAttacking = false;

        if (swordHitbox != null)
            swordHitbox.SetActive(false);
    }
}