using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skill1 : MonoBehaviour
{
    public GameObject shurikenPrefab;
    public KeyCode KeyBindSkill1;

    public float closeRange = 2f;
    public float detectRadius = 8f;

    public int shurikenAmount = 8;
    public float shurikenSpeed = 8f;
    public float shurikenLifetime = 2f;

    public float dashDistance = 2f;
    public float dashSpeed = 15f;
    public int dashDamage = 1;
    public float dashHitRadius = 0.5f;
    public float dashCount = 1;

    public float cooldown = 3f;

    private float nextSkillTime;

    void Update()
    {
        if (Input.GetKeyDown(KeyBindSkill1) && Time.time >= nextSkillTime)
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
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Collider2D col = GetComponent<Collider2D>();

        if (col != null) col.enabled = false;
        if (rb != null) rb.isKinematic = true;

        for (int i = 0; i < dashCount; i++)
        {
            if (target == null)
            {
                target = FindNearestEnemy();
                if (target == null) break;
            }

            HashSet<BaseEnemy> hitEnemies = new HashSet<BaseEnemy>();

            Vector2 direction = (target.transform.position - transform.position).normalized;
            Vector2 dashTarget = (Vector2)target.transform.position + direction * dashDistance;

            while (Vector2.Distance(transform.position, dashTarget) > 0.05f)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    dashTarget,
                    dashSpeed * Time.deltaTime
                );

                Collider2D[] hits = Physics2D.OverlapCircleAll(
                    transform.position,
                    dashHitRadius
                );

                foreach (Collider2D hit in hits)
                {
                    BaseEnemy enemy = hit.GetComponent<BaseEnemy>();
                    if (enemy != null && !hitEnemies.Contains(enemy))
                    {
                        enemy.TakeDamage(1, Vector2.zero, false);
                        hitEnemies.Add(enemy);
                    }
                }

                yield return null;
            }

            yield return new WaitForSeconds(0.1f);

            if (target == null)
                target = FindNearestEnemy();
        }

        if (col != null) col.enabled = true;
        if (rb != null) rb.isKinematic = false;
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
        BaseEnemy[] enemies = FindObjectsOfType<BaseEnemy>();

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (BaseEnemy enemy in enemies)
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