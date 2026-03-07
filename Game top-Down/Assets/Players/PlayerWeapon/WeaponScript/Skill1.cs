using System.Collections;
using UnityEngine;

public class skill1 : MonoBehaviour
{
    public GameObject shurikenPrefab;

    public float closeRange = 2f;
    public float detectRadius = 8f;

    public int shurikenAmount = 8;
    public float shurikenSpeed = 8f;
    public float shurikenLifetime = 2f;

    public float dashDistance = 2f;
    public float dashSpeed = 15f;
    public int dashDamage = 1;
    public float dashHitRadius = 0.5f;

    public float cooldown = 3f;

    private float nextSkillTime;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && Time.time >= nextSkillTime)
        {
            GameObject target = FindNearestEnemy();

            if (target != null)
            {
                float distance = Vector2.Distance(transform.position, target.transform.position);

                if (distance <= closeRange)
                {
                    StartCoroutine(DashAttack(target));
                }
                else
                {
                    ShurikenBurst();
                }
            }

            nextSkillTime = Time.time + cooldown;
        }
    }

    IEnumerator DashAttack(GameObject target)
    {
        for (int i = 0; i < 2; i++)
        {
            if (target == null) yield break;

            Vector2 direction = (target.transform.position - transform.position).normalized;

            Vector2 dashTarget = (Vector2)target.transform.position + direction * 1.5f;

            while (Vector2.Distance(transform.position, dashTarget) > 0.05f)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    dashTarget,
                    dashSpeed * Time.deltaTime
                );

                // cek musuh yang terkena dash
                Collider2D[] hits = Physics2D.OverlapCircleAll(
                    transform.position,
                    dashHitRadius
                );

                foreach (Collider2D hit in hits)
                {
                    DummyEnemy enemy = hit.GetComponent<DummyEnemy>();

                    if (enemy != null)
                    {
                        enemy.TakeDamage(dashDamage);
                    }
                }

                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    void ShurikenBurst()
    {
        float angleStep = 360f / shurikenAmount;

        for (int i = 0; i < shurikenAmount; i++)
        {
            float angle = i * angleStep;

            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            GameObject shuriken = Instantiate(
                shurikenPrefab,
                transform.position,
                Quaternion.identity
            );

            Rigidbody2D rb = shuriken.GetComponent<Rigidbody2D>();

            if (rb != null)
                rb.velocity = direction * shurikenSpeed;

            Destroy(shuriken, shurikenLifetime);
        }
    }

    GameObject FindNearestEnemy()
    {
        DummyEnemy[] enemies = FindObjectsOfType<DummyEnemy>();

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (DummyEnemy enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            if (distance < shortestDistance && distance <= detectRadius)
            {
                shortestDistance = distance;
                nearestEnemy = enemy.gameObject;
            }
        }

        return nearestEnemy;
    }
}