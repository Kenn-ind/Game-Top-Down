using UnityEngine;

public class SkillUpgradeManager : MonoBehaviour
{
    [Header("Data")]
    public SkillUpgradeData upgradeData;
    public Skill1SO skill1SO;
    public Skill2SO skill2SO;
    public Skill3SO skill3SO;

    [Header("Upgrade Settings")]
    public float cooldownReductionPerUpgrade = 0.5f;
    public int staminaReductionPerUpgrade = 1;
    public float minCooldown = 0.5f;
    public int minStamina = 1;
    public float berserkerDurationIncrement = 1f;
    public float lifestealIncrement = 0.05f;

    private ScrollInventory _scrollInventory;

    public enum SkillType { Skill1Melee, Skill1Range, Skill2Melee, Skill2Range, Skill3 }
    public enum UpgradeType { Damage = 0, Cooldown = 1, Stamina = 2, Unique = 3, Unique2 = 4 }

    void Awake()
    {
        _scrollInventory = GetComponent<ScrollInventory>();
        upgradeData = Instantiate(upgradeData);
    }

    // ─── Ambil count langsung dari SO ─────────────────────────
    public int GetUpgradeCount(SkillType skill, UpgradeType upgrade)
    {
        switch (skill)
        {
            case SkillType.Skill1Melee:
                switch (upgrade)
                {
                    case UpgradeType.Damage: return upgradeData.meleeDamageCount;
                    case UpgradeType.Cooldown: return upgradeData.meleeCooldownCount;
                    case UpgradeType.Stamina: return upgradeData.meleeStaminaCount;
                    case UpgradeType.Unique: return upgradeData.meleeDashCount;
                }
                break;
            case SkillType.Skill1Range:
                switch (upgrade)
                {
                    case UpgradeType.Damage: return upgradeData.rangeDamageCount;
                    case UpgradeType.Cooldown: return upgradeData.rangeCooldownCount;
                    case UpgradeType.Stamina: return upgradeData.rangeStaminaCount;
                    case UpgradeType.Unique: return upgradeData.rangeShurikenCount;
                }
                break;
            case SkillType.Skill2Melee:
                switch (upgrade)
                {
                    case UpgradeType.Damage: return upgradeData.skill2MeleeDamageCount;
                    case UpgradeType.Cooldown: return upgradeData.skill2MeleeCooldownCount;
                    case UpgradeType.Stamina: return upgradeData.skill2MeleeStaminaCount;
                }
                break;
            case SkillType.Skill2Range:
                switch (upgrade)
                {
                    case UpgradeType.Damage: return upgradeData.skill2RangeDamageCount;
                    case UpgradeType.Cooldown: return upgradeData.skill2RangeCooldownCount;
                    case UpgradeType.Stamina: return upgradeData.skill2RangeStaminaCount;
                    case UpgradeType.Unique: return upgradeData.skill2RangeShurikenCount;
                }
                break;
            case SkillType.Skill3:
                switch (upgrade)
                {
                    case UpgradeType.Damage: return upgradeData.skill3LifestealCount;
                    case UpgradeType.Cooldown: return upgradeData.skill3CooldownCount;
                    case UpgradeType.Stamina: return upgradeData.skill3StaminaCount;
                    case UpgradeType.Unique: return upgradeData.skill3ShurikenCount;
                    case UpgradeType.Unique2: return upgradeData.skill3BerserkerCount;
                }
                break;
        }
        return 0;
    }

    int GetMaxUpgrade(UpgradeType upgrade)
    {
        switch (upgrade)
        {
            case UpgradeType.Damage: return upgradeData.maxDamageUpgrade;
            case UpgradeType.Cooldown: return upgradeData.maxCooldownUpgrade;
            case UpgradeType.Stamina: return upgradeData.maxStaminaUpgrade;
            case UpgradeType.Unique: return upgradeData.maxUniqueUpgrade;
            case UpgradeType.Unique2: return upgradeData.maxUnique2Upgrade;
        }
        return 0;
    }

    public bool CanUpgrade(SkillType skill, UpgradeType upgrade)
    {
        return GetUpgradeCount(skill, upgrade) < GetMaxUpgrade(upgrade);
    }

    // ─── Increment count di SO ────────────────────────────────
    void IncrementCount(SkillType skill, UpgradeType upgrade)
    {
        switch (skill)
        {
            case SkillType.Skill1Melee:
                switch (upgrade)
                {
                    case UpgradeType.Damage: upgradeData.meleeDamageCount++; break;
                    case UpgradeType.Cooldown: upgradeData.meleeCooldownCount++; break;
                    case UpgradeType.Stamina: upgradeData.meleeStaminaCount++; break;
                    case UpgradeType.Unique: upgradeData.meleeDashCount++; break;
                }
                break;
            case SkillType.Skill1Range:
                switch (upgrade)
                {
                    case UpgradeType.Damage: upgradeData.rangeDamageCount++; break;
                    case UpgradeType.Cooldown: upgradeData.rangeCooldownCount++; break;
                    case UpgradeType.Stamina: upgradeData.rangeStaminaCount++; break;
                    case UpgradeType.Unique: upgradeData.rangeShurikenCount++; break;
                }
                break;
            case SkillType.Skill2Melee:
                switch (upgrade)
                {
                    case UpgradeType.Damage: upgradeData.skill2MeleeDamageCount++; break;
                    case UpgradeType.Cooldown: upgradeData.skill2MeleeCooldownCount++; break;
                    case UpgradeType.Stamina: upgradeData.skill2MeleeStaminaCount++; break;
                }
                break;
            case SkillType.Skill2Range:
                switch (upgrade)
                {
                    case UpgradeType.Damage: upgradeData.skill2RangeDamageCount++; break;
                    case UpgradeType.Cooldown: upgradeData.skill2RangeCooldownCount++; break;
                    case UpgradeType.Stamina: upgradeData.skill2RangeStaminaCount++; break;
                    case UpgradeType.Unique: upgradeData.skill2RangeShurikenCount++; break;
                }
                break;
            case SkillType.Skill3:
                switch (upgrade)
                {
                    case UpgradeType.Damage: upgradeData.skill3LifestealCount++; break;
                    case UpgradeType.Cooldown: upgradeData.skill3CooldownCount++; break;
                    case UpgradeType.Stamina: upgradeData.skill3StaminaCount++; break;
                    case UpgradeType.Unique: upgradeData.skill3ShurikenCount++; break;
                    case UpgradeType.Unique2: upgradeData.skill3BerserkerCount++; break;
                }
                break;
        }
    }

    // ─── Try Upgrade ──────────────────────────────────────────
    public bool TryUpgrade(SkillType skill, UpgradeType upgrade)
    {
        if (!CanUpgrade(skill, upgrade))
        {
            Debug.Log($"[{skill} {upgrade}] Sudah maksimal!");
            return false;
        }
        if (!_scrollInventory.UseScroll())
        {
            Debug.Log("Scroll tidak cukup!");
            return false;
        }

        switch (skill)
        {
            case SkillType.Skill1Melee: UpgradeSkill1Melee(upgrade); break;
            case SkillType.Skill1Range: UpgradeSkill1Range(upgrade); break;
            case SkillType.Skill2Melee: UpgradeSkill2Melee(upgrade); break;
            case SkillType.Skill2Range: UpgradeSkill2Range(upgrade); break;
            case SkillType.Skill3: UpgradeSkill3(upgrade); break;
        }

        IncrementCount(skill, upgrade);
        return true;
    }

    void UpgradeSkill1Melee(UpgradeType upgrade)
    {
        switch (upgrade)
        {
            case UpgradeType.Damage: upgradeData.meleeDamageBonus++; break;
            case UpgradeType.Cooldown: skill1SO.cooldown = Mathf.Max(minCooldown, skill1SO.cooldown - cooldownReductionPerUpgrade); break;
            case UpgradeType.Stamina: skill1SO.staminaCost = Mathf.Max(minStamina, skill1SO.staminaCost - staminaReductionPerUpgrade); break;
            case UpgradeType.Unique: upgradeData.meleeDashCountBonus++; skill1SO.melee.dashCount++; break;
        }
    }

    void UpgradeSkill1Range(UpgradeType upgrade)
    {
        switch (upgrade)
        {
            case UpgradeType.Damage: upgradeData.rangeDamageBonus++; break;
            case UpgradeType.Cooldown: skill1SO.cooldown = Mathf.Max(minCooldown, skill1SO.cooldown - cooldownReductionPerUpgrade); break;
            case UpgradeType.Stamina: skill1SO.staminaCost = Mathf.Max(minStamina, skill1SO.staminaCost - staminaReductionPerUpgrade); break;
            case UpgradeType.Unique: upgradeData.rangeShurikenBonus++; skill1SO.range.shurikenAmount++; break;
        }
    }

    void UpgradeSkill2Melee(UpgradeType upgrade)
    {
        switch (upgrade)
        {
            case UpgradeType.Damage: upgradeData.skill2MeleeDamageBonus++; break;
            case UpgradeType.Cooldown: skill2SO.cooldown = Mathf.Max(minCooldown, skill2SO.cooldown - cooldownReductionPerUpgrade); break;
            case UpgradeType.Stamina: skill2SO.staminaCost = Mathf.Max(minStamina, skill2SO.staminaCost - staminaReductionPerUpgrade); break;
        }
    }

    void UpgradeSkill2Range(UpgradeType upgrade)
    {
        switch (upgrade)
        {
            case UpgradeType.Damage: upgradeData.skill2RangeDamageBonus++; break;
            case UpgradeType.Cooldown: skill2SO.cooldown = Mathf.Max(minCooldown, skill2SO.cooldown - cooldownReductionPerUpgrade); break;
            case UpgradeType.Stamina: skill2SO.staminaCost = Mathf.Max(minStamina, skill2SO.staminaCost - staminaReductionPerUpgrade); break;
            case UpgradeType.Unique: upgradeData.skill2RangeShurikenBonus++; skill2SO.range.shurikenAmount++; break;
        }
    }

    void UpgradeSkill3(UpgradeType upgrade)
    {
        switch (upgrade)
        {
            case UpgradeType.Damage: upgradeData.skill3LifestealBonus += lifestealIncrement; skill3SO.lifestealPercent = Mathf.Min(0.8f, skill3SO.lifestealPercent + lifestealIncrement); break;
            case UpgradeType.Cooldown: skill3SO.cooldown = Mathf.Max(minCooldown, skill3SO.cooldown - cooldownReductionPerUpgrade); break;
            case UpgradeType.Stamina: skill3SO.staminaCost = Mathf.Max(minStamina, skill3SO.staminaCost - staminaReductionPerUpgrade); break;
            case UpgradeType.Unique: upgradeData.skill3ShurikenBonus++; skill3SO.shurikenAmount++; break;
            case UpgradeType.Unique2: upgradeData.skill3BerserkerDurationBonus += berserkerDurationIncrement; skill3SO.berserkerDuration += berserkerDurationIncrement; break;
        }
    }

    // ─── Preview ──────────────────────────────────────────────
    public string GetUpgradePreview(SkillType skill, UpgradeType upgrade)
    {
        int current = GetUpgradeCount(skill, upgrade);
        int max = GetMaxUpgrade(upgrade);

        if (current >= max)
            return "<color=red>✗ MAXED</color>";

        string progress = $"({current}/{max})";

        switch (skill)
        {
            case SkillType.Skill1Melee: return GetSkill1MeleePreview(upgrade, progress);
            case SkillType.Skill1Range: return GetSkill1RangePreview(upgrade, progress);
            case SkillType.Skill2Melee: return GetSkill2MeleePreview(upgrade, progress);
            case SkillType.Skill2Range: return GetSkill2RangePreview(upgrade, progress);
            case SkillType.Skill3: return GetSkill3Preview(upgrade, progress);
        }
        return "";
    }

    string GetSkill1MeleePreview(UpgradeType u, string p)
    {
        switch (u)
        {
            case UpgradeType.Damage: return $"Damage Bonus: {upgradeData.meleeDamageBonus} → {upgradeData.meleeDamageBonus + 1} {p}";
            case UpgradeType.Cooldown: return $"Cooldown: {skill1SO.cooldown:F1}s → {Mathf.Max(minCooldown, skill1SO.cooldown - cooldownReductionPerUpgrade):F1}s {p}";
            case UpgradeType.Stamina: return $"Stamina: {skill1SO.staminaCost} → {Mathf.Max(minStamina, skill1SO.staminaCost - staminaReductionPerUpgrade)} {p}";
            case UpgradeType.Unique: return $"Dash Count: {skill1SO.melee.dashCount} → {skill1SO.melee.dashCount + 1} {p}";
        }
        return "";
    }

    string GetSkill1RangePreview(UpgradeType u, string p)
    {
        switch (u)
        {
            case UpgradeType.Damage: return $"Damage Bonus: {upgradeData.rangeDamageBonus} → {upgradeData.rangeDamageBonus + 1} {p}";
            case UpgradeType.Cooldown: return $"Cooldown: {skill1SO.cooldown:F1}s → {Mathf.Max(minCooldown, skill1SO.cooldown - cooldownReductionPerUpgrade):F1}s {p}";
            case UpgradeType.Stamina: return $"Stamina: {skill1SO.staminaCost} → {Mathf.Max(minStamina, skill1SO.staminaCost - staminaReductionPerUpgrade)} {p}";
            case UpgradeType.Unique: return $"Shuriken: {skill1SO.range.shurikenAmount} → {skill1SO.range.shurikenAmount + 1} {p}";
        }
        return "";
    }

    string GetSkill2MeleePreview(UpgradeType u, string p)
    {
        switch (u)
        {
            case UpgradeType.Damage: return $"Damage Bonus: {upgradeData.skill2MeleeDamageBonus} → {upgradeData.skill2MeleeDamageBonus + 1} {p}";
            case UpgradeType.Cooldown: return $"Cooldown: {skill2SO.cooldown:F1}s → {Mathf.Max(minCooldown, skill2SO.cooldown - cooldownReductionPerUpgrade):F1}s {p}";
            case UpgradeType.Stamina: return $"Stamina: {skill2SO.staminaCost} → {Mathf.Max(minStamina, skill2SO.staminaCost - staminaReductionPerUpgrade)} {p}";
        }
        return "";
    }

    string GetSkill2RangePreview(UpgradeType u, string p)
    {
        switch (u)
        {
            case UpgradeType.Damage: return $"Damage Bonus: {upgradeData.skill2RangeDamageBonus} → {upgradeData.skill2RangeDamageBonus + 1} {p}";
            case UpgradeType.Cooldown: return $"Cooldown: {skill2SO.cooldown:F1}s → {Mathf.Max(minCooldown, skill2SO.cooldown - cooldownReductionPerUpgrade):F1}s {p}";
            case UpgradeType.Stamina: return $"Stamina: {skill2SO.staminaCost} → {Mathf.Max(minStamina, skill2SO.staminaCost - staminaReductionPerUpgrade)} {p}";
            case UpgradeType.Unique: return $"Shuriken: {skill2SO.range.shurikenAmount} → {skill2SO.range.shurikenAmount + 1} {p}";
        }
        return "";
    }

    string GetSkill3Preview(UpgradeType u, string p)
    {
        switch (u)
        {
            case UpgradeType.Damage: return $"Lifesteal: {skill3SO.lifestealPercent * 100f:F0}% → {Mathf.Min(80f, skill3SO.lifestealPercent * 100f + lifestealIncrement * 100f):F0}% {p}";
            case UpgradeType.Cooldown: return $"Cooldown: {skill3SO.cooldown:F1}s → {Mathf.Max(minCooldown, skill3SO.cooldown - cooldownReductionPerUpgrade):F1}s {p}";
            case UpgradeType.Stamina: return $"Stamina: {skill3SO.staminaCost} → {Mathf.Max(minStamina, skill3SO.staminaCost - staminaReductionPerUpgrade)} {p}";
            case UpgradeType.Unique: return $"Shuriken: {skill3SO.shurikenAmount} → {skill3SO.shurikenAmount + 1} {p}";
            case UpgradeType.Unique2: return $"Berserker: {skill3SO.berserkerDuration:F1}s → {skill3SO.berserkerDuration + berserkerDurationIncrement:F1}s {p}";
        }
        return "";
    }

    public void ReApplyUpgrades()
    {
        // Re-initialize semua skill SO agar bonus teraplikasi ulang
        skill1SO?.Initialize(gameObject);
        skill2SO?.Initialize(gameObject);
        skill3SO?.Initialize(gameObject);

        // Apply semua bonus dari upgradeData ke SO
        SkillUpgradeData su = upgradeData;

        // Skill1
        skill1SO.melee.dashCount = su.meleeDashCountBonus;
        skill1SO.range.shurikenAmount += su.rangeShurikenBonus;

        // Skill2
        skill2SO.range.shurikenAmount += su.skill2RangeShurikenBonus;

        // Skill3
        skill3SO.shurikenAmount += su.skill3ShurikenBonus;
        skill3SO.berserkerDuration += su.skill3BerserkerDurationBonus;
        skill3SO.lifestealPercent = Mathf.Min(0.8f, skill3SO.lifestealPercent + su.skill3LifestealBonus);

        Debug.Log("[SkillUpgradeManager] Upgrades re-applied!");
    }
}