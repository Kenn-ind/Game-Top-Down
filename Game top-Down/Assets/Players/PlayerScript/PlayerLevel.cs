using UnityEngine;
using System.Collections;

public class PlayerLevel : MonoBehaviour
{
    public int baseXPRequired = 100;
    public float xpMultiplier = 1.5f;
    public int debugAddXP = 50;
    public int maxLevel = 20; // ← LEVEL CAP

    private int _currentXP = 0;
    private int _xpRequired;
    private PlayerStats _stats;
    public StatUpgradeUI statUpgradeUI;

    void Start()
    {
        _stats = GetComponent<PlayerStats>();
        _xpRequired = CalculateXPRequired(_stats.level);
        if (statUpgradeUI != null)
            statUpgradeUI.Init();
        Debug.Log($"Level {_stats.level} | XP dibutuhkan: {_xpRequired}");
    }

    public void AddXP(int amount)
    {
        // Sudah max level, tidak terima XP
        if (_stats.level >= maxLevel)
        {
            Debug.Log("Level sudah maksimal!");
            return;
        }

        _currentXP += amount;
        Debug.Log($"XP: {_currentXP} / {_xpRequired}");

        bool leveledUp = false;

        while (_currentXP >= _xpRequired && _stats.level < maxLevel)
        {
            _currentXP -= _xpRequired;
            _stats.GainLevel();
            _xpRequired = CalculateXPRequired(_stats.level);
            leveledUp = true;

            Debug.Log($"Level up! Sekarang level {_stats.level}");

            // Jika sudah capai max level, reset sisa XP
            if (_stats.level >= maxLevel)
            {
                _currentXP = 0;
                Debug.Log($"Level maksimal ({maxLevel}) tercapai!");
                break;
            }
        }

        if (leveledUp && statUpgradeUI != null)
        {
            statUpgradeUI.ShowPopup();
        }
    }

    [ContextMenu("AddDebugXP")]
    public void AddDebugXP()
    {
        AddXP(debugAddXP);
    }

    int CalculateXPRequired(int level)
    {
        return Mathf.RoundToInt(baseXPRequired * Mathf.Pow(xpMultiplier, level - 1));
    }

    public bool IsMaxLevel => _stats.level >= maxLevel;
    public int CurrentXP => _currentXP;
    public int XPRequired => _xpRequired;
}