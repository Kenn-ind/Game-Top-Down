using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int maxHP = 10;
    public int maxStamina = 10;
    public float armor = 0f;
    public int attackDamage = 1;

    public int hpPerPoint = 2;
    public int staminaPerPoint = 2; 
    public float armorPerPoint = 5f;
    public int attackPerPoint = 1; 

    public int level = 1;
    public int statPointsPerLevel = 3;
    public int availablePoints = 0;

    private PlayerHealth _health;
    private PlayerStamina _stamina;

    void Awake()
    {
        _health = GetComponent<PlayerHealth>();
        _stamina = GetComponent<PlayerStamina>();
    }


    public void GainLevel()
    {
        level++;
        availablePoints += statPointsPerLevel;
        Debug.Log($"Level up! Sekarang level {level}. Poin tersedia: {availablePoints}");
    }


    public bool UpgradeHP()
    {
        if (availablePoints <= 0) return false;
        availablePoints--;
        maxHP += hpPerPoint;
        _health.maxHealth = maxHP;
        _health.Heal(hpPerPoint);
        Debug.Log($"Max HP naik jadi {maxHP}");
        return true;
    }

    public bool UpgradeStamina()
    {
        if (availablePoints <= 0) return false;
        availablePoints--;
        maxStamina += staminaPerPoint;
        _stamina.maxStamina = maxStamina;
        Debug.Log($"Max Stamina naik jadi {maxStamina}");
        return true;
    }

    public bool UpgradeArmor()
    {
        if (availablePoints <= 0) return false;
        availablePoints--;
        armor = Mathf.Min(armor + armorPerPoint, 75f);
        Debug.Log($"Armor naik jadi {armor}%");
        return true;
    }

    public bool UpgradeAttack()
    {
        if (availablePoints <= 0) return false;
        availablePoints--;
        attackDamage += attackPerPoint;
        Debug.Log($"Attack naik jadi {attackDamage}");
        return true;
    }
    public int CalculateDamage(int rawDamage)
    {
        float reduction = 1f - (armor / 100f);
        return Mathf.Max(1, Mathf.RoundToInt(rawDamage * reduction));
    }
}