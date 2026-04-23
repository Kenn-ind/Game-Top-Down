using UnityEngine;

[CreateAssetMenu(fileName = "SkillUpgradeData", menuName = "Skills/SkillUpgradeData")]
public class SkillUpgradeData : ScriptableObject
{
    [Header("Skill1 Melee")]
    public int meleeDamageBonus = 0;
    public int meleeDashCountBonus = 0;

    [Header("Skill1 Range")]
    public int rangeDamageBonus = 0;
    public int rangeShurikenBonus = 0;

    [Header("Skill2 Melee")]
    public int skill2MeleeDamageBonus = 0;

    [Header("Skill2 Range")]
    public int skill2RangeDamageBonus = 0;
    public int skill2RangeShurikenBonus = 0;

    [Header("Skill3")]
    public int skill3ShurikenBonus = 0;
    public float skill3BerserkerDurationBonus = 0f;
    public float skill3LifestealBonus = 0f;
}