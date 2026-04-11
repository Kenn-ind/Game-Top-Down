using System.Collections;
using UnityEngine;

public class Enemy : BaseEnemy
{
    public float flashTime = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public override void TakeDamage(int damage, Vector2 knockbackDir, bool applyKnockback = true)
    {
        if (isDead) return; // Jangan flash jika sudah mati

        StartCoroutine(FlashRed());
        AudioManager.PlaySFX(AudioManager.enemyHurt);
        Debug.Log("Enemy kena hit");
        base.TakeDamage(damage, knockbackDir, applyKnockback);
    }

    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(flashTime);
        spriteRenderer.color = originalColor;
    }
}