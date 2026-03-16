using System.Collections;
using UnityEngine;

public class DummyEnemy : BaseEnemy
{
    AudioManage AudioManager;
    public float flashTime = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    protected override void Start()
    {
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        AudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManage>();
    }

    public override void TakeDamage(int damage, Vector2 knockbackDir, bool applyKnockback = false)
    {
        StartCoroutine(FlashRed());
        AudioManager.PlaySFX(AudioManager.enemyHurt);
        Debug.Log("Dummy kena hit");    
    }

    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(flashTime);
        spriteRenderer.color = originalColor;
    }
}