using UnityEngine;

public class Shuriken : MonoBehaviour
{
    public float rotateSpeed = 720f;
    public GameObject hitParticle;

    private PlayerStats _stats;
    private SkillUpgradeData _upgradeData;
    private bool _isRangeSkill = false; // true = pakai rangeDamageBonus

    public void Init(PlayerStats stats, SkillUpgradeData data, bool isRangeSkill = false)
    {
        _stats = stats;
        _upgradeData = data;
        _isRangeSkill = isRangeSkill;
    }

    void Update()
    {
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BaseEnemy enemy = collision.GetComponent<BaseEnemy>();
        if (enemy != null)
        {
            int baseDamage = _stats != null ? _stats.attackDamage : 1;
            int bonus = 0;

            if (_upgradeData != null)
                bonus = _isRangeSkill ? _upgradeData.rangeDamageBonus : _upgradeData.meleeDamageBonus;

            enemy.TakeDamage(baseDamage + bonus, Vector2.zero, false);

            if (hitParticle != null)
            {
                GameObject particle = Instantiate(hitParticle, transform.position, Quaternion.identity);
                Destroy(particle, 1f);
            }

            Destroy(gameObject);
        }
    }
}