using UnityEngine;

public class BossEnemy : BaseEnemy
{
    private BossHealth bossHealth;

    protected override void Start()
    {
        base.Start();
        bossHealth = GetComponent<BossHealth>();
    }

    public override void TakeDamage(int damage, Vector2 knockbackDir, bool applyKnockback = false)
    {
        if (isDead) return;

        // Teruskan damage ke BossHealth
        if (bossHealth != null)
            bossHealth.TakeDamage(damage);
        else
            base.TakeDamage(damage, knockbackDir, applyKnockback);
    }

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        // BossHealth yang handle Die(), bukan BaseEnemy
        // Tidak perlu destroy di sini
    }
}