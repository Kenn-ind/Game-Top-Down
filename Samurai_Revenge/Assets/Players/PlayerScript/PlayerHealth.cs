using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 10;
    public float knockbackTime = 0.2f;
    private int currentHealth;
    private Rigidbody2D rb;
    public bool isKnockback;
    public Sprite[] healthFrames;
    public Image healthImage;
    private int _currentFrame = 0;
    private Coroutine _hpAnim;
    private PlayerStats _stats;
    private PlayerUlt _ult;

    public bool IsFullHealth => currentHealth >= maxHealth;

    public int CurrentHealth => currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        _currentFrame = 0;
        healthImage.sprite = healthFrames[0];
        _stats = GetComponent<PlayerStats>();
        _ult = GetComponent<PlayerUlt>();
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        if (_ult != null) _ult.CancelCharge();
        int finalDamage = _stats != null ? _stats.CalculateDamage(damage) : damage;
        currentHealth -= finalDamage;
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log($"Damage masuk: {damage} → setelah armor: {finalDamage}");
        UpdateHealthBar();
        StartCoroutine(ApplyKnockback(knockback));
        if (currentHealth <= 0) Die();
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

    public void LoadHealth(int value)
    {
        currentHealth = Mathf.Clamp(value, 0, maxHealth);
        // Reset frame bar langsung tanpa animasi
        if (_hpAnim != null) StopCoroutine(_hpAnim);
        float ratio = 1f - ((float)currentHealth / maxHealth);
        _currentFrame = Mathf.RoundToInt(ratio * (healthFrames.Length - 1));
        if (healthImage != null && healthFrames.Length > 0)
            healthImage.sprite = healthFrames[_currentFrame];
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