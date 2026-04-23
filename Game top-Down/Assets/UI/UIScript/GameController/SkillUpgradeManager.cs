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

    // Skill3 specific increment values
    public float berserkerDurationIncrement = 1f;
    public float lifestealIncrement = 0.05f; // +5% per upgrade

    private ScrollInventory _scrollInventory;

    void Awake()
    {
        _scrollInventory = GetComponent<ScrollInventory>();
    }

    public enum SkillType { Skill1Melee, Skill1Range, Skill2Melee, Skill2Range, Skill3 }
    public enum UpgradeType { Damage = 0, Cooldown = 1, Stamina = 2, Unique = 3, Unique2 = 4 }

    public bool TryUpgrade(SkillType skill, UpgradeType upgrade)
    {
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

        return true;
    }

    // ─── Skill1 ───────────────────────────────────────────────
    void UpgradeSkill1Melee(UpgradeType upgrade)
    {
        switch (upgrade)
        {
            case UpgradeType.Damage:
                upgradeData.meleeDamageBonus++;
                Debug.Log($"[S1 Melee] Damage bonus: {upgradeData.meleeDamageBonus}");
                break;
            case UpgradeType.Cooldown:
                skill1SO.cooldown = Mathf.Max(minCooldown, skill1SO.cooldown - cooldownReductionPerUpgrade);
                Debug.Log($"[S1 Melee] Cooldown: {skill1SO.cooldown}s");
                break;
            case UpgradeType.Stamina:
                skill1SO.staminaCost = Mathf.Max(minStamina, skill1SO.staminaCost - staminaReductionPerUpgrade);
                Debug.Log($"[S1 Melee] Stamina: {skill1SO.staminaCost}");
                break;
            case UpgradeType.Unique:
                upgradeData.meleeDashCountBonus++;
                skill1SO.melee.dashCount++;
                Debug.Log($"[S1 Melee] Dash count: {skill1SO.melee.dashCount}");
                break;
        }
    }

    void UpgradeSkill1Range(UpgradeType upgrade)
    {
        switch (upgrade)
        {
            case UpgradeType.Damage:
                upgradeData.rangeDamageBonus++;
                Debug.Log($"[S1 Range] Damage bonus: {upgradeData.rangeDamageBonus}");
                break;
            case UpgradeType.Cooldown:
                skill1SO.cooldown = Mathf.Max(minCooldown, skill1SO.cooldown - cooldownReductionPerUpgrade);
                Debug.Log($"[S1 Range] Cooldown: {skill1SO.cooldown}s");
                break;
            case UpgradeType.Stamina:
                skill1SO.staminaCost = Mathf.Max(minStamina, skill1SO.staminaCost - staminaReductionPerUpgrade);
                Debug.Log($"[S1 Range] Stamina: {skill1SO.staminaCost}");
                break;
            case UpgradeType.Unique:
                upgradeData.rangeShurikenBonus++;
                skill1SO.range.shurikenAmount++;
                Debug.Log($"[S1 Range] Shuriken: {skill1SO.range.shurikenAmount}");
                break;
        }
    }

    // ─── Skill2 ───────────────────────────────────────────────
    void UpgradeSkill2Melee(UpgradeType upgrade)
    {
        switch (upgrade)
        {
            case UpgradeType.Damage:
                upgradeData.skill2MeleeDamageBonus++;
                Debug.Log($"[S2 Melee] Damage bonus: {upgradeData.skill2MeleeDamageBonus}");
                break;
            case UpgradeType.Cooldown:
                skill2SO.cooldown = Mathf.Max(minCooldown, skill2SO.cooldown - cooldownReductionPerUpgrade);
                Debug.Log($"[S2 Melee] Cooldown: {skill2SO.cooldown}s");
                break;
            case UpgradeType.Stamina:
                skill2SO.staminaCost = Mathf.Max(minStamina, skill2SO.staminaCost - staminaReductionPerUpgrade);
                Debug.Log($"[S2 Melee] Stamina: {skill2SO.staminaCost}");
                break;
        }
    }

    void UpgradeSkill2Range(UpgradeType upgrade)
    {
        switch (upgrade)
        {
            case UpgradeType.Damage:
                upgradeData.skill2RangeDamageBonus++;
                Debug.Log($"[S2 Range] Damage bonus: {upgradeData.skill2RangeDamageBonus}");
                break;
            case UpgradeType.Cooldown:
                skill2SO.cooldown = Mathf.Max(minCooldown, skill2SO.cooldown - cooldownReductionPerUpgrade);
                Debug.Log($"[S2 Range] Cooldown: {skill2SO.cooldown}s");
                break;
            case UpgradeType.Stamina:
                skill2SO.staminaCost = Mathf.Max(minStamina, skill2SO.staminaCost - staminaReductionPerUpgrade);
                Debug.Log($"[S2 Range] Stamina: {skill2SO.staminaCost}");
                break;
            case UpgradeType.Unique:
                upgradeData.skill2RangeShurikenBonus++;
                skill2SO.range.shurikenAmount++;
                Debug.Log($"[S2 Range] Shuriken: {skill2SO.range.shurikenAmount}");
                break;
        }
    }

    // ─── Skill3 ───────────────────────────────────────────────
    void UpgradeSkill3(UpgradeType upgrade)
    {
        switch (upgrade)
        {
            case UpgradeType.Unique: // Shuriken Amount
                upgradeData.skill3ShurikenBonus++;
                skill3SO.shurikenAmount++;
                Debug.Log($"[S3] Shuriken: {skill3SO.shurikenAmount}");
                break;
            case UpgradeType.Unique2: // Berserker Duration
                upgradeData.skill3BerserkerDurationBonus += berserkerDurationIncrement;
                skill3SO.berserkerDuration += berserkerDurationIncrement;
                Debug.Log($"[S3] Berserker duration: {skill3SO.berserkerDuration}s");
                break;
            case UpgradeType.Damage: // Lifesteal
                upgradeData.skill3LifestealBonus += lifestealIncrement;
                skill3SO.lifestealPercent = Mathf.Min(0.8f, skill3SO.lifestealPercent + lifestealIncrement);
                Debug.Log($"[S3] Lifesteal: {skill3SO.lifestealPercent * 100f}%");
                break;
            case UpgradeType.Cooldown:
                skill3SO.cooldown = Mathf.Max(minCooldown, skill3SO.cooldown - cooldownReductionPerUpgrade);
                Debug.Log($"[S3] Cooldown: {skill3SO.cooldown}s");
                break;
            case UpgradeType.Stamina:
                skill3SO.staminaCost = Mathf.Max(minStamina, skill3SO.staminaCost - staminaReductionPerUpgrade);
                Debug.Log($"[S3] Stamina: {skill3SO.staminaCost}");
                break;
        }
    }

    // ─── Preview text untuk UI ────────────────────────────────
    public string GetUpgradePreview(SkillType skill, UpgradeType upgrade)
    {
        switch (skill)
        {
            case SkillType.Skill1Melee:
                return GetSkill1MeleePreview(upgrade);
            case SkillType.Skill1Range:
                return GetSkill1RangePreview(upgrade);
            case SkillType.Skill2Melee:
                return GetSkill2MeleePreview(upgrade);
            case SkillType.Skill2Range:
                return GetSkill2RangePreview(upgrade);
            case SkillType.Skill3:
                return GetSkill3Preview(upgrade);
        }
        return "";
    }

    string GetSkill1MeleePreview(UpgradeType u)
    {
        switch (u)
        {
            case UpgradeType.Damage: return $"Damage Bonus: {upgradeData.meleeDamageBonus} → {upgradeData.meleeDamageBonus + 1}";
            case UpgradeType.Cooldown: return $"Cooldown: {skill1SO.cooldown:F1}s → {Mathf.Max(minCooldown, skill1SO.cooldown - cooldownReductionPerUpgrade):F1}s";
            case UpgradeType.Stamina: return $"Stamina Cost: {skill1SO.staminaCost} → {Mathf.Max(minStamina, skill1SO.staminaCost - staminaReductionPerUpgrade)}";
            case UpgradeType.Unique: return $"Dash Count: {skill1SO.melee.dashCount} → {skill1SO.melee.dashCount + 1}";
        }
        return "";
    }

    string GetSkill1RangePreview(UpgradeType u)
    {
        switch (u)
        {
            case UpgradeType.Damage: return $"Damage Bonus: {upgradeData.rangeDamageBonus} → {upgradeData.rangeDamageBonus + 1}";
            case UpgradeType.Cooldown: return $"Cooldown: {skill1SO.cooldown:F1}s → {Mathf.Max(minCooldown, skill1SO.cooldown - cooldownReductionPerUpgrade):F1}s";
            case UpgradeType.Stamina: return $"Stamina Cost: {skill1SO.staminaCost} → {Mathf.Max(minStamina, skill1SO.staminaCost - staminaReductionPerUpgrade)}";
            case UpgradeType.Unique: return $"Shuriken: {skill1SO.range.shurikenAmount} → {skill1SO.range.shurikenAmount + 1}";
        }
        return "";
    }

    string GetSkill2MeleePreview(UpgradeType u)
    {
        switch (u)
        {
            case UpgradeType.Damage: return $"Damage Bonus: {upgradeData.skill2MeleeDamageBonus} → {upgradeData.skill2MeleeDamageBonus + 1}";
            case UpgradeType.Cooldown: return $"Cooldown: {skill2SO.cooldown:F1}s → {Mathf.Max(minCooldown, skill2SO.cooldown - cooldownReductionPerUpgrade):F1}s";
            case UpgradeType.Stamina: return $"Stamina Cost: {skill2SO.staminaCost} → {Mathf.Max(minStamina, skill2SO.staminaCost - staminaReductionPerUpgrade)}";
        }
        return "";
    }

    string GetSkill2RangePreview(UpgradeType u)
    {
        switch (u)
        {
            case UpgradeType.Damage: return $"Damage Bonus: {upgradeData.skill2RangeDamageBonus} → {upgradeData.skill2RangeDamageBonus + 1}";
            case UpgradeType.Cooldown: return $"Cooldown: {skill2SO.cooldown:F1}s → {Mathf.Max(minCooldown, skill2SO.cooldown - cooldownReductionPerUpgrade):F1}s";
            case UpgradeType.Stamina: return $"Stamina Cost: {skill2SO.staminaCost} → {Mathf.Max(minStamina, skill2SO.staminaCost - staminaReductionPerUpgrade)}";
            case UpgradeType.Unique: return $"Shuriken: {skill2SO.range.shurikenAmount} → {skill2SO.range.shurikenAmount + 1}";
        }
        return "";
    }

    string GetSkill3Preview(UpgradeType u)
    {
        switch (u)
        {
            case UpgradeType.Unique: return $"Shuriken: {skill3SO.shurikenAmount} → {skill3SO.shurikenAmount + 1}";
            case UpgradeType.Unique2: return $"Berserker Duration: {skill3SO.berserkerDuration:F1}s → {skill3SO.berserkerDuration + berserkerDurationIncrement:F1}s";
            case UpgradeType.Damage: return $"Lifesteal: {skill3SO.lifestealPercent * 100f:F0}% → {Mathf.Min(80f, skill3SO.lifestealPercent * 100f + lifestealIncrement * 100f):F0}%";
            case UpgradeType.Cooldown: return $"Cooldown: {skill3SO.cooldown:F1}s → {Mathf.Max(minCooldown, skill3SO.cooldown - cooldownReductionPerUpgrade):F1}s";
            case UpgradeType.Stamina: return $"Stamina Cost: {skill3SO.staminaCost} → {Mathf.Max(minStamina, skill3SO.staminaCost - staminaReductionPerUpgrade)}";
        }
        return "";
    }
}