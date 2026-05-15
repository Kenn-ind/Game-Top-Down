using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillController : MonoBehaviour
{
    [Header("Skill Ring")]
    public GameObject shurikenPrefab;
    public int shurikenAmount = 6;
    public float orbitRadius = 1.5f;
    public float rotateSpeed = 200f;
    public float ringDuration = 5f;

    [Header("Berserker")]
    public float berserkerDuration = 5f;
    public float damageMultiplier = 2f;
    public float attackSpeedMultiplier = 2f;

    private bool isBerserkerActive = false;
    private bool isUsingSkill = false;
    private BossAttack bossAttack;
    private SpriteRenderer spriteRenderer;

    public bool IsBusy => isUsingSkill || isBerserkerActive;

    void Start()
    {
        bossAttack = GetComponent<BossAttack>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void UseRandomSkill()
    {
        if (IsBusy) return;

        int skill = Random.Range(0, 2);
        if (skill == 0)
            StartCoroutine(ActivateRing());
        else
            StartCoroutine(BerserkerMode());
    }

    IEnumerator ActivateRing()
    {
        isUsingSkill = true;

        List<GameObject> shurikens = new List<GameObject>();
        for (int i = 0; i < shurikenAmount; i++)
        {
            float angle = i * Mathf.PI * 2 / shurikenAmount;
            Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * orbitRadius;
            GameObject s = Instantiate(shurikenPrefab,
                transform.position + (Vector3)pos, Quaternion.identity);

            BossProjectile bp = s.GetComponent<BossProjectile>();
            if (bp == null) bp = s.AddComponent<BossProjectile>();
            bp.damage = 8;

            shurikens.Add(s);
        }

        float timer = 0;
        while (timer < ringDuration)
        {
            timer += Time.deltaTime;
            for (int i = 0; i < shurikens.Count; i++)
            {
                if (shurikens[i] == null) continue;
                float angle = (Time.time * rotateSpeed + i * 360f / shurikenAmount)
                              * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle))
                                 * orbitRadius;
                shurikens[i].transform.position =
                    transform.position + (Vector3)offset;
            }
            yield return null;
        }

        foreach (GameObject s in shurikens)
            if (s != null) Destroy(s);

        isUsingSkill = false;
    }

    IEnumerator BerserkerMode()
    {
        isBerserkerActive = true;

        if (spriteRenderer != null)
            spriteRenderer.color = Color.red;

        if (bossAttack != null)
        {
            bossAttack.meleeDamage = Mathf.RoundToInt(
                bossAttack.meleeDamage * damageMultiplier);
            bossAttack.attackCooldown /= attackSpeedMultiplier;
        }

        yield return new WaitForSeconds(berserkerDuration);

        if (bossAttack != null)
        {
            bossAttack.meleeDamage = Mathf.RoundToInt(
                bossAttack.meleeDamage / damageMultiplier);
            bossAttack.attackCooldown *= attackSpeedMultiplier;
        }

        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;

        isBerserkerActive = false;
    }

    public void ResetState()
    {
        StopAllCoroutines();
        isUsingSkill = false;
        isBerserkerActive = false;
        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;
    }
}