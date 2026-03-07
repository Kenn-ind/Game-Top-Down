using System.Collections;
using UnityEngine;

public class skill2 : MonoBehaviour
{
    public GameObject shurikenPrefab;

    public float closeRange = 2f;
    public float detectRadius = 8f;

    public float cooldown = 3f;

    // SPIN SLASH
    public float spinDuration = 0.5f;
    public float spinSpeed = 720f;
    public float spinRadius = 1.5f;
    public int spinDamage = 1;

    // SHURIKEN
    public int shurikenAmount = 5;
    public float shurikenSpeed = 10f;
    public float shurikenLifetime = 2f;
    public float fireDelay = 0.1f;

    float nextSkillTime;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2) && Time.time >= nextSkillTime)
        {
            GameObject target = FindNearestEnemy();

            if (target != null)
            {
                float distance = Vector2.Distance(transform.position, target.transform.position);

                if (distance <= closeRange)
                {
                    StartCoroutine(SpinSlash());
                }
                else
                {
                    StartCoroutine(ShurikenBurst(target));
                }
            }

            nextSkillTime = Time.time + cooldown;
        }
    }

    IEnumerator SpinSlash()
    {
        float timer = 0;

        Quaternion originalRotation = transform.rotation;

        while (timer < spinDuration)
        {
            timer += Time.deltaTime;

            transform.Rotate(0, 0, spinSpeed * Time.deltaTime);

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, spinRadius);

            foreach (Collider2D hit in hits)
            {
                DummyEnemy enemy = hit.GetComponent<DummyEnemy>();

                if (enemy != null)
                {
                    enemy.TakeDamage(spinDamage);
                }
            }

            yield return null;
        }

        // kembalikan rotasi player
        transform.rotation = originalRotation;
    }

    IEnumerator ShurikenBurst(GameObject target)
    {
        Vector2 direction = (target.transform.position - transform.position).normalized;

        for (int i = 0; i < shurikenAmount; i++)
        {
            GameObject shuriken = Instantiate(
                shurikenPrefab,
                transform.position,
                Quaternion.identity
            );

            Rigidbody2D rb = shuriken.GetComponent<Rigidbody2D>();

            if (rb != null)
                rb.velocity = direction * shurikenSpeed;

            Destroy(shuriken, shurikenLifetime);

            yield return new WaitForSeconds(fireDelay);
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spinRadius);
    }
}