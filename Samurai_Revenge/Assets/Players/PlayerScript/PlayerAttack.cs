using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    AudioManage AudioManager;
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
    private Skill3SO _skill3;
    private PlayerSkillState _skillState;
    private PlayerHealth _playerHealth;
    private PlayerStats _stats;

    public Image attackButtonImage;
    public Sprite meleeSprite;
    public Sprite rangeSprite;

    private Queue<GameObject> shurikenQueue = new Queue<GameObject>();
    private bool isAttacking = false;

    public bool IsAttacking() => isAttacking;

    void Start()
    {
        AudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManage>();
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<movement>();
        _playerHealth = GetComponent<PlayerHealth>();
        _stats = GetComponent<PlayerStats>();
        _skillState = GetComponent<PlayerSkillState>();

        SkillController skillController = GetComponent<SkillController>();
        if (skillController != null)
            _skill3 = skillController.skill3 as Skill3SO;

        if (swordHitbox != null)
            swordHitbox.SetActive(false);

        if (attackButtonImage != null && meleeSprite != null)
            attackButtonImage.sprite = meleeSprite;
    }

    float GetAttackDelay()
    {
        if (_skill3 != null && _skill3.IsBerserkerActive)
            return attackDelay / _skill3.AttackSpeedMultiplier;
        return attackDelay;
    }

    void Update()
    {
        if (_skillState != null && _skillState.isUsingSkill) return;

        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextAttackTime && !isAttacking)
        {
            nextAttackTime = Time.time + GetAttackDelay();
            HandleAttack();
        }
    }

    void HandleAttack()
    {
        GameObject meleeTarget = FindNearestEnemyInRadius(meleeRadius);
        if (meleeTarget != null)
        {
            if (attackButtonImage != null && meleeSprite != null)
                attackButtonImage.sprite = meleeSprite;
            MeleeAttack(meleeTarget);
            return;
        }

        GameObject rangeTarget = FindNearestEnemyInRadius(rangeRadius);
        if (rangeTarget != null)
        {
            if (attackButtonImage != null && rangeSprite != null)
                attackButtonImage.sprite = rangeSprite;
            RangeAttack(rangeTarget);
        }
    }

    int GetFinalDamage(int baseDamage)
    {
        if (_skill3 != null && _skill3.IsBerserkerActive)
            return Mathf.RoundToInt(baseDamage * _skill3.DamageMultiplier);
        return baseDamage;
    }

    void MeleeAttack(GameObject target)
    {
        isAttacking = true;
        AudioManager.PlaySFX(AudioManager.sword);

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

            Collider2D[] hits = Physics2D.OverlapCircleAll(swordHitbox.transform.position, 0.5f);
            foreach (Collider2D hit in hits)
            {
                BaseEnemy enemy = hit.GetComponent<BaseEnemy>();
                if (enemy != null && _stats != null)
                {
                    int damage = GetFinalDamage(_stats.attackDamage);
                    enemy.TakeDamage(damage, Vector2.zero, false);
                    _skill3?.OnDamageDealt(damage, _playerHealth);
                }
            }

            yield return new WaitForSeconds(0.2f);
            swordHitbox.SetActive(false);
        }

        yield return new WaitForSeconds(GetAttackDelay());
        isAttacking = false;
    }

    void RangeAttack(GameObject target)
    {
        isAttacking = true;
        AudioManager.PlaySFX(AudioManager.shuriken);

        Vector2 direction = (target.transform.position - transform.position).normalized;
        TriggerAttackRangeAnimation(direction);

        GameObject shuriken = Instantiate(shurikenPrefab, transform.position, Quaternion.identity);

        Shuriken shurikenScript = shuriken.GetComponent<Shuriken>();
        if (shurikenScript != null)
            shurikenScript.Init(_stats, null, false);

        Rigidbody2D rb = shuriken.GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = direction * shurikenSpeed;

        shurikenQueue.Enqueue(shuriken);
        if (shurikenQueue.Count > shurikenMax)
        {
            GameObject oldest = shurikenQueue.Dequeue();
            if (oldest != null) Destroy(oldest);
        }

        StartCoroutine(RangeRoutine());
    }

    IEnumerator RangeRoutine()
    {
        yield return new WaitForSeconds(GetAttackDelay());
        isAttacking = false;
    }

    GameObject FindNearestEnemyInRadius(float radius)
    {
        BaseEnemy[] enemies = FindObjectsOfType<BaseEnemy>();
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (BaseEnemy enemy in enemies)
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
            attackDir = direction.x > 0 ? Vector2.right : Vector2.left;
        else
            attackDir = direction.y > 0 ? Vector2.up : Vector2.down;

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

    public void MobileAttack()
    {
        if (_skillState != null && _skillState.isUsingSkill) return;
        if (Time.time < nextAttackTime || isAttacking) return;

        nextAttackTime = Time.time + GetAttackDelay();
        HandleAttack();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, meleeRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangeRadius);
    }
}