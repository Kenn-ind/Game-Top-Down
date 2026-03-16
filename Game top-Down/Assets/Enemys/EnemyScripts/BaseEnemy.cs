using UnityEngine;
using System.Collections;

public class BaseEnemy : MonoBehaviour
{
    AudioManage AudioManager;
    public int maxHealth = 5;
    protected int currentHealth;

    public float knockbackForce = 10f;
    public float knockbackTime = 0.15f;

    private Rigidbody2D rb;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        AudioManager=GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManage>();
    }

    public virtual void TakeDamage(int damage, Vector2 knockbackDir, bool applyKnockback = false)
    {
        currentHealth -= damage;

        if (applyKnockback)
        {
            AudioManager.PlaySFX(AudioManager.enemyHurt);
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
        Destroy(gameObject);
    }
}