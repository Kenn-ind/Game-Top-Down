using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skill2 : MonoBehaviour
{
    AudioManage AudioManager;
    public GameObject shurikenPrefab;
    public KeyCode KeyBindSkill2;
    public float closeRange = 2f;
    public float detectRadius = 8f;
    public float cooldown = 3f;
    public int staminaCost = 3; 

    public float spinDuration = 0.5f;
    public float spinSpeed = 720f;
    public float spinRadius = 1.5f;
    public int spinDamage = 1;

    public int shurikenAmount = 5;
    public float shurikenSpeed = 10f;
    public float shurikenLifetime = 2f;
    public float fireDelay = 0.1f;

    float nextSkillTime;
    private PlayerStamina _stamina;

    void Start()
    {
        _stamina = GetComponent<PlayerStamina>();
        AudioManager=GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManage>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyBindSkill2) && Time.time >= nextSkillTime)
        {
            if (!_stamina.HasEnough(staminaCost))
            {
                Debug.Log("Stamina tidak cukup!");
                return;
            }

            GameObject target = FindNearestEnemy();
            if (target != null)
            {
                float distance = Vector2.Distance(transform.position, target.transform.position);
                if (distance <= closeRange)
                    StartCoroutine(SpinSlash());
                else
                    StartCoroutine(ShurikenBurst());
            }

            nextSkillTime = Time.time + cooldown;
        }
    }

    IEnumerator SpinSlash()
    {
        _stamina.UseStamina(staminaCost);

        float timer = 0;
        Quaternion originalRotation = transform.rotation;
        HashSet<BaseEnemy> hitEnemies = new HashSet<BaseEnemy>();

        while (timer < spinDuration)
        {
            timer += Time.deltaTime;
            transform.Rotate(0, 0, spinSpeed * Time.deltaTime);

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, spinRadius);
            foreach (Collider2D hit in hits)
            {
                BaseEnemy enemy = hit.GetComponent<BaseEnemy>();
                if (enemy != null && !hitEnemies.Contains(enemy))
                {
                    AudioManager.PlaySFX(AudioManager.S2Sword);
                    enemy.TakeDamage(1, Vector2.zero, false);
                    hitEnemies.Add(enemy);
                }
            }
            yield return null;
        }

        transform.rotation = originalRotation;
    }

    IEnumerator ShurikenBurst()
    {
        _stamina.UseStamina(staminaCost);

        for (int i = 0; i < shurikenAmount; i++)
        {
            GameObject target = FindNearestEnemy();
            if (target == null) yield break;

            AudioManager.PlaySFX(AudioManager.S2Shu);
            Vector2 direction = (target.transform.position - transform.position).normalized;
            GameObject shuriken = Instantiate(shurikenPrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = shuriken.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = direction * shurikenSpeed;
            Destroy(shuriken, shurikenLifetime);

            yield return new WaitForSeconds(fireDelay);
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spinRadius);
    }
}