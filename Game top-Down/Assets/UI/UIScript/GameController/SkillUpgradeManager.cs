using UnityEngine;

public class SkillUpgradeManager : MonoBehaviour
{
    [Header("Data")]
    public SkillUpgradeData upgradeData;
    public Skill1SO skill1SO;

    [Header("Upgrade Settings")]
    public float cooldownReductionPerUpgrade = 0.5f;
    public int staminaReductionPerUpgrade = 1;
    public float minCooldown = 0.5f;
    public int minStamina = 1;

    private ScrollInventory _scrollInventory;

    void Awake()
    {
        _scrollInventory = GetComponent<ScrollInventory>();
    }

    public enum SkillType { Melee, Range }

    // Index harus sama dengan urutan dropdown options di UI
    // 0 = Damage, 1 = Cooldown, 2 = Stamina, 3 = Unique (Dash/Shuriken)
    public enum UpgradeType { Damage = 0, Cooldown = 1, Stamina = 2, Unique = 3 }

    public bool TryUpgrade(SkillType skill, UpgradeType upgrade)
    {
        if (_scrollInventory == null)
        {
            Debug.LogError("ScrollInventory tidak ditemukan!");
            return false;
        }

        if (!_scrollInventory.UseScroll())
        {
            Debug.Log("Scroll tidak cukup!");
            return false;
        }

        switch (upgrade)
        {
            case UpgradeType.Damage:
                if (skill == SkillType.Melee)
                {
                    upgradeData.meleeDamageBonus++;
                    Debug.Log($"[Melee] Damage bonus: +{upgradeData.meleeDamageBonus}");
                }
                else
                {
                    upgradeData.rangeDamageBonus++;
                    Debug.Log($"[Range] Damage bonus: +{upgradeData.rangeDamageBonus}");
                }
                break;

            case UpgradeType.Cooldown:
                // Cooldown di SkillSO, shared untuk skill ini
                skill1SO.cooldown = Mathf.Max(minCooldown, skill1SO.cooldown - cooldownReductionPerUpgrade);
                Debug.Log($"[{skill}] Cooldown sekarang: {skill1SO.cooldown}s");
                break;

            case UpgradeType.Stamina:
                skill1SO.staminaCost = Mathf.Max(minStamina, skill1SO.staminaCost - staminaReductionPerUpgrade);
                Debug.Log($"[{skill}] Stamina cost sekarang: {skill1SO.staminaCost}");
                break;

            case UpgradeType.Unique:
                if (skill == SkillType.Melee)
                {
                    upgradeData.meleeDashCountBonus++;
                    skill1SO.melee.dashCount++;
                    Debug.Log($"[Melee] Dash count: {skill1SO.melee.dashCount}");
                }
                else
                {
                    upgradeData.rangeShurikenBonus++;
                    skill1SO.range.shurikenAmount++;
                    Debug.Log($"[Range] Shuriken: {skill1SO.range.shurikenAmount}");
                }
                break;
        }

        return true;
    }

    // Getter untuk UI info
    public string GetUpgradePreview(SkillType skill, UpgradeType upgrade)
    {
        switch (upgrade)
        {
            case UpgradeType.Damage:
                int currentBonus = skill == SkillType.Melee ? upgradeData.meleeDamageBonus : upgradeData.rangeDamageBonus;
                return $"Damage Bonus: {currentBonus} → {currentBonus + 1}";

            case UpgradeType.Cooldown:
                float newCD = Mathf.Max(minCooldown, skill1SO.cooldown - cooldownReductionPerUpgrade);
                return $"Cooldown: {skill1SO.cooldown:F1}s → {newCD:F1}s";

            case UpgradeType.Stamina:
                int newST = Mathf.Max(minStamina, skill1SO.staminaCost - staminaReductionPerUpgrade);
                return $"Stamina Cost: {skill1SO.staminaCost} → {newST}";

            case UpgradeType.Unique:
                if (skill == SkillType.Melee)
                    return $"Dash Count: {skill1SO.melee.dashCount} → {skill1SO.melee.dashCount + 1}";
                else
                    return $"Shuriken: {skill1SO.range.shurikenAmount} → {skill1SO.range.shurikenAmount + 1}";
        }
        return "";
    }
}