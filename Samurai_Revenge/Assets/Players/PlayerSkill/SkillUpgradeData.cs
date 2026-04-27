using UnityEngine;

[CreateAssetMenu(fileName = "SkillUpgradeData", menuName = "Skills/SkillUpgradeData")]
public class SkillUpgradeData : ScriptableObject
{
    [Header("=== MAX UPGRADE LIMITS ===")]
    public int maxDamageUpgrade = 5;
    public int maxCooldownUpgrade = 4;
    public int maxStaminaUpgrade = 4;
    public int maxUniqueUpgrade = 3;
    public int maxUnique2Upgrade = 3;

    [Header("Skill1 Melee — Bonus & Count")]
    public int meleeDamageBonus = 0;
    public int meleeDamageCount = 0;
    public int meleeDashCountBonus = 0;
    public int meleeDashCount = 0;
    public int meleeCooldownCount = 0;
    public int meleeStaminaCount = 0;

    [Header("Skill1 Range — Bonus & Count")]
    public int rangeDamageBonus = 0;
    public int rangeDamageCount = 0;
    public int rangeShurikenBonus = 0;
    public int rangeShurikenCount = 0;
    public int rangeCooldownCount = 0;
    public int rangeStaminaCount = 0;

    [Header("Skill2 Melee — Bonus & Count")]
    public int skill2MeleeDamageBonus = 0;
    public int skill2MeleeDamageCount = 0;
    public int skill2MeleeCooldownCount = 0;
    public int skill2MeleeStaminaCount = 0;

    [Header("Skill2 Range — Bonus & Count")]
    public int skill2RangeDamageBonus = 0;
    public int skill2RangeDamageCount = 0;
    public int skill2RangeShurikenBonus = 0;
    public int skill2RangeShurikenCount = 0;
    public int skill2RangeCooldownCount = 0;
    public int skill2RangeStaminaCount = 0;

    [Header("Skill3 — Bonus & Count")]
    public int skill3ShurikenBonus = 0;
    public int skill3ShurikenCount = 0;
    public float skill3BerserkerDurationBonus = 0f;
    public int skill3BerserkerCount = 0;
    public float skill3LifestealBonus = 0f;
    public int skill3LifestealCount = 0;
    public int skill3CooldownCount = 0;
    public int skill3StaminaCount = 0;

    // Reset semua — untuk testing
    [ContextMenu("Reset All Upgrades")]
    public void ResetAll()
    {
        meleeDamageBonus = 0; meleeDamageCount = 0;
        meleeDashCountBonus = 0; meleeDashCount = 0;
        meleeCooldownCount = 0; meleeStaminaCount = 0;

        rangeDamageBonus = 0; rangeDamageCount = 0;
        rangeShurikenBonus = 0; rangeShurikenCount = 0;
        rangeCooldownCount = 0; rangeStaminaCount = 0;

        skill2MeleeDamageBonus = 0; skill2MeleeDamageCount = 0;
        skill2MeleeCooldownCount = 0; skill2MeleeStaminaCount = 0;

        skill2RangeDamageBonus = 0; skill2RangeDamageCount = 0;
        skill2RangeShurikenBonus = 0; skill2RangeShurikenCount = 0;
        skill2RangeCooldownCount = 0; skill2RangeStaminaCount = 0;

        skill3ShurikenBonus = 0; skill3ShurikenCount = 0;
        skill3BerserkerDurationBonus = 0; skill3BerserkerCount = 0;
        skill3LifestealBonus = 0; skill3LifestealCount = 0;
        skill3CooldownCount = 0; skill3StaminaCount = 0;

        Debug.Log("Semua upgrade di-reset!");
    }
}