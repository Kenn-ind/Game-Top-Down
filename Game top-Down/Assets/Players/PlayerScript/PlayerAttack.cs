using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject shurikenPrefab;
    public float shurikenSpeed = 10f;
    public int shurikenMax = 5;
    public float aimRadius = 5f;
    public float attackDelay = 0.5f;
    private float nextAttackTime;

    private Queue<GameObject> shurikenQueue = new Queue<GameObject>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackDelay;
            FireShuriken();
        }
    }

    void FireShuriken()
    {
        GameObject target = FindNearestEnemy();

        if (target == null)
        {
            Debug.Log("Tidak ada musuh!");
            return;
        }

        Vector2 direction = (target.transform.position - transform.position).normalized;

        GameObject shuriken = Instantiate(
            shurikenPrefab,
            transform.position,
            Quaternion.identity
        );

        Rigidbody2D rb = shuriken.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = direction * shurikenSpeed;
        }

        shurikenQueue.Enqueue(shuriken);

        if (shurikenQueue.Count > shurikenMax)
        {
            GameObject oldestShuriken = shurikenQueue.Dequeue();
            if (oldestShuriken != null)
            {
                Destroy(oldestShuriken);
            }
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

            if (distance < shortestDistance && distance <= aimRadius)
            {
                shortestDistance = distance;
                nearestEnemy = enemy.gameObject;
            }
        }

        return nearestEnemy;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aimRadius);
    }
}