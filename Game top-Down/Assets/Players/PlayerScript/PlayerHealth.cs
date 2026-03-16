using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 10;
    public float knockbackTime = 0.2f;
    private int currentHealth;
    private Rigidbody2D rb;
    public bool isKnockback;

    [Header("Health Bar")]
    public Sprite[] healthFrames;
    public Image healthImage;
    private int _currentFrame = 0;
    private Coroutine _hpAnim;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        _currentFrame = 0;
        healthImage.sprite = healthFrames[0];
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log("Player kena damage");
        UpdateHealthBar();
        StartCoroutine(ApplyKnockback(knockback));
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        float ratio = 1f - ((float)currentHealth / maxHealth);
        int target = Mathf.RoundToInt(ratio * (healthFrames.Length - 1));
        if (_hpAnim != null) StopCoroutine(_hpAnim);
        _hpAnim = StartCoroutine(AnimateBar(target));
    }

    IEnumerator AnimateBar(int target)
    {
        int dir = target > _currentFrame ? 1 : -1;
        while (_currentFrame != target)
        {
            _currentFrame += dir;
            healthImage.sprite = healthFrames[_currentFrame];
            yield return new WaitForSeconds(1f / 12f);
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