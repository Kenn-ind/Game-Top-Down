using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public string slotName;
    public string saveDateTime;
    public int playTimeSeconds;

    public int level;
    public int currentXP;
    public int availablePoints;
    public int maxHP;
    public int currentHP;
    public int maxStamina;
    public float armor;
    public int attackDamage;

    public int coins;
    public int scrollCount;

    public List<ItemSaveData> inventoryItems = new List<ItemSaveData>();

    public List<QuestSaveData> activeQuests = new List<QuestSaveData>();
    public List<string> completedQuests = new List<string>(); 
    public List<string> turnedInQuests = new List<string>();
    public string trackedQuestName;

    public SkillUpgradeSaveData skillUpgrades = new SkillUpgradeSaveData();

    public List<string> openedChestIDs = new List<string>();

    public float playerX;
    public float playerY;
    public string currentScene;
    public string activeBoundaryName;
}

[Serializable]
public class ItemSaveData
{
    public int slotIndex;
    public string itemID;
    public int stackCount;
}

[Serializable]
public class QuestSaveData
{
    public string questName;
    public int progress;
}

[Serializable]
public class SkillUpgradeSaveData
{
    public int meleeDamageBonus;
    public int meleeDamageCount;
    public int meleeDashCountBonus;
    public int meleeDashCount;
    public int meleeCooldownCount;
    public int meleeStaminaCount;

    public int rangeDamageBonus;
    public int rangeDamageCount;
    public int rangeShurikenBonus;
    public int rangeShurikenCount;
    public int rangeCooldownCount;
    public int rangeStaminaCount;

    public int skill2MeleeDamageBonus;
    public int skill2MeleeDamageCount;
    public int skill2MeleeCooldownCount;
    public int skill2MeleeStaminaCount;

    public int skill2RangeDamageBonus;
    public int skill2RangeDamageCount;
    public int skill2RangeShurikenBonus;
    public int skill2RangeShurikenCount;
    public int skill2RangeCooldownCount;
    public int skill2RangeStaminaCount;

    public int skill3ShurikenBonus;
    public int skill3ShurikenCount;
    public float skill3BerserkerDurationBonus;
    public int skill3BerserkerCount;
    public float skill3LifestealBonus;
    public int skill3LifestealCount;
    public int skill3CooldownCount;
    public int skill3StaminaCount;
}