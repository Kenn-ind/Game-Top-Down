using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 200;
    public int currentHealth;

    [Header("UI")]
    public Slider healthBar;

    private Animator animator;
    private BossAI bossAI;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        bossAI = GetComponent<BossAI>();

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
            healthBar.value = currentHealth;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (bossAI != null) bossAI.enabled = false;
        animator?.SetTrigger("Die");
        BossFightManager.Instance?.OnBossDefeated();
        Destroy(gameObject, 2f);
    }
}