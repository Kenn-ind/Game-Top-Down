using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public float knockbackTime = 0.2f;

    private int currentHealth;
    private Rigidbody2D rb;

    public bool isKnockback;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        currentHealth -= damage;

        Debug.Log("Player kena damage");

        StartCoroutine(ApplyKnockback(knockback));

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator ApplyKnockback(Vector2 force)
    {
        isKnockback = true;

        rb.velocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackTime);

        isKnockback = false;
    }

    void Die()
    {
        Debug.Log("Player mati");
        Destroy(gameObject);
    }
}