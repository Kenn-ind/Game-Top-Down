using UnityEngine;
using System.Collections;

public class BaseEnemy : MonoBehaviour
{
    protected AudioManage AudioManager;
    public int maxHealth = 5;
    protected int currentHealth;
    public string enemyID = "";
    public float knockbackForce = 10f;
    public float knockbackTime = 0.15f;

    [Header("XP Settings")]
    public int xpReward = 30; // ← XP yang diberikan saat mati

    [Header("Death Settings")]
    public float deathDelay = 0.5f;
    protected bool isDead = false;

    protected Animator animator;
    private Rigidbody2D rb;
    private static readonly int ParamDie = Animator.StringToHash("Die");

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        GameObject audioObj = GameObject.FindGameObjectWithTag("Audio");
        if (audioObj == null)
            Debug.LogError("Tidak ada GameObject dengan tag 'Audio' di scene!");
        else
        {
            AudioManager = audioObj.GetComponent<AudioManage>();
            if (AudioManager == null)
                Debug.LogError("AudioManage component tidak ditemukan!");
        }
    }

    public bool IsDead() => isDead;

    public virtual void TakeDamage(int damage, Vector2 knockbackDir, bool applyKnockback = false)
    {
        if (isDead) return;
        currentHealth -= damage;
        if (applyKnockback)
        {
            StartCoroutine(Knockback(knockbackDir));
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator Knockback(Vector2 direction)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackTime);
        rb.velocity = Vector2.zero;
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (animator != null)
            animator.SetTrigger(ParamDie);

        // ← Berikan XP ke player
        PlayerLevel playerLevel = FindObjectOfType<PlayerLevel>();
        if (playerLevel != null)
            playerLevel.AddXP(xpReward);

        PlayerUlt playerUlt = FindObjectOfType<PlayerUlt>();
        if (playerUlt != null)
            playerUlt.OnEnemyKilled();

        QuestManager.Instance?.ReportKill(enemyID);
        StartCoroutine(DestroyAfterDelay());
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject);
    }
}