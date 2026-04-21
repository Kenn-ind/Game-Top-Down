using UnityEngine;

[CreateAssetMenu(fileName = "SkillUpgradeData", menuName = "Skills/SkillUpgradeData")]
public class SkillUpgradeData : ScriptableObject
{
    [Header("Skill1 Melee Upgrade Levels")]
    public int meleeDamageBonus = 0;
    public int meleeDashCountBonus = 0;

    [Header("Skill1 Range Upgrade Levels")]
    public int rangeDamageBonus = 0;
    public int rangeShurikenBonus = 0;
}