using System.Collections;
using UnityEngine;

public class DummyEnemy : BaseEnemy
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

    public override void TakeDamage(int damage, Vector2 knockbackDir, bool applyKnockback = false)
    {
        StartCoroutine(FlashRed());

        Debug.Log("Dummy kena hit");    
    }

    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(flashTime);
        spriteRenderer.color = originalColor;
    }
}