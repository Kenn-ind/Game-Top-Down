using UnityEngine;
using TMPro;

public class StatDisplayUI : MonoBehaviour
{
    public PlayerStats stats;

    public TMP_Text levelText;
    public TMP_Text hpText;
    public TMP_Text staminaText;
    public TMP_Text armorText;
    public TMP_Text attackText;

    public void RefreshDisplay()
    {
        levelText.text = $"Level   : {stats.level}";
        hpText.text = $"Max HP  : {stats.maxHP}";
        staminaText.text = $"Stamina : {stats.maxStamina}";
        armorText.text = $"Armor   : {stats.armor}%";
        attackText.text = $"Attack  : {stats.attackDamage}";
    }
}