using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class SkillUpgradeUI : MonoBehaviour
{
    [Header("Manager & Inventory")]
    public SkillUpgradeManager upgradeManager;
    public ScrollInventory scrollInventory;

    [Header("Skill1 Melee UI")]
    public TMP_Dropdown skill1MeleeDropdown;
    public Button skill1MeleeButton;
    public TextMeshProUGUI skill1MeleePreview;

    [Header("Skill1 Range UI")]
    public TMP_Dropdown skill1RangeDropdown;
    public Button skill1RangeButton;
    public TextMeshProUGUI skill1RangePreview;

    [Header("Skill2 Melee UI")]
    public TMP_Dropdown skill2MeleeDropdown;
    public Button skill2MeleeButton;
    public TextMeshProUGUI skill2MeleePreview;

    [Header("Skill2 Range UI")]
    public TMP_Dropdown skill2RangeDropdown;
    public Button skill2RangeButton;
    public TextMeshProUGUI skill2RangePreview;

    [Header("Skill3 UI")]
    public TMP_Dropdown skill3Dropdown;
    public Button skill3Button;
    public TextMeshProUGUI skill3Preview;

    [Header("Shared UI")]
    public TextMeshProUGUI scrollCountText;
    public TextMeshProUGUI feedbackText;

    private Coroutine _feedbackCoroutine;

    // Pasangan dropdown + button + preview + skillType
    private struct SkillUIGroup
    {
        public TMP_Dropdown dropdown;
        public Button button;
        public TextMeshProUGUI preview;
        public SkillUpgradeManager.SkillType skillType;
    }

    private List<SkillUIGroup> _groups;

    void Start()
    {
        _groups = new List<SkillUIGroup>
        {
            new SkillUIGroup { dropdown = skill1MeleeDropdown, button = skill1MeleeButton, preview = skill1MeleePreview, skillType = SkillUpgradeManager.SkillType.Skill1Melee },
            new SkillUIGroup { dropdown = skill1RangeDropdown, button = skill1RangeButton, preview = skill1RangePreview, skillType = SkillUpgradeManager.SkillType.Skill1Range },
            new SkillUIGroup { dropdown = skill2MeleeDropdown, button = skill2MeleeButton, preview = skill2MeleePreview, skillType = SkillUpgradeManager.SkillType.Skill2Melee },
            new SkillUIGroup { dropdown = skill2RangeDropdown, button = skill2RangeButton, preview = skill2RangePreview, skillType = SkillUpgradeManager.SkillType.Skill2Range },
            new SkillUIGroup { dropdown = skill3Dropdown,      button = skill3Button,      preview = skill3Preview,      skillType = SkillUpgradeManager.SkillType.Skill3 },
        };

        SetupDropdowns();
        SetupButtons();
        RefreshAll();
    }

    void SetupDropdowns()
    {
        // Skill1 Melee
        SetOptions(skill1MeleeDropdown, new List<string>
        {
            "⚔ Damage +1",
            "⏱ Cooldown -0.5s",
            "💨 Stamina Cost -1",
            "🔁 Dash Count +1"
        });

        // Skill1 Range
        SetOptions(skill1RangeDropdown, new List<string>
        {
            "⚔ Damage +1",
            "⏱ Cooldown -0.5s",
            "💨 Stamina Cost -1",
            "🌀 Shuriken +1"
        });

        // Skill2 Melee (tidak ada Unique)
        SetOptions(skill2MeleeDropdown, new List<string>
        {
            "⚔ Damage +1",
            "⏱ Cooldown -0.5s",
            "💨 Stamina Cost -1"
        });

        // Skill2 Range
        SetOptions(skill2RangeDropdown, new List<string>
        {
            "⚔ Damage +1",
            "⏱ Cooldown -0.5s",
            "💨 Stamina Cost -1",
            "🌀 Shuriken +1"
        });

        // Skill3 — index harus match UpgradeType enum
        // Damage=0(Lifesteal), Cooldown=1, Stamina=2, Unique=3(Shuriken), Unique2=4(Berserker)
        SetOptions(skill3Dropdown, new List<string>
        {
            "💉 Lifesteal +5%",
            "⏱ Cooldown -0.5s",
            "💨 Stamina Cost -1",
            "🌀 Shuriken +1",
            "🔥 Berserker Duration +1s"
        });

        // Listener update preview saat pilihan berubah
        foreach (var g in _groups)
        {
            var group = g; // capture untuk lambda
            if (group.dropdown != null)
                group.dropdown.onValueChanged.AddListener(_ => UpdatePreview(group));
        }
    }

    void SetOptions(TMP_Dropdown dropdown, List<string> options)
    {
        if (dropdown == null) return;
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
    }

    void SetupButtons()
    {
        foreach (var g in _groups)
        {
            var group = g;
            if (group.button != null)
                group.button.onClick.AddListener(() => OnUpgradeClicked(group));
        }
    }

    void OnUpgradeClicked(SkillUIGroup group)
    {
        if (group.dropdown == null) return;
        var upgradeType = (SkillUpgradeManager.UpgradeType)group.dropdown.value;
        bool success = upgradeManager.TryUpgrade(group.skillType, upgradeType);
        string optionName = group.dropdown.options[group.dropdown.value].text;
        ShowFeedback(success, group.skillType.ToString(), optionName);
        RefreshAll();
    }

    void RefreshAll()
    {
        UpdateScrollText();
        foreach (var g in _groups)
            UpdatePreview(g);
        UpdateButtonStates();
    }

    void UpdatePreview(SkillUIGroup group)
    {
        if (group.preview == null || group.dropdown == null) return;
        var upgradeType = (SkillUpgradeManager.UpgradeType)group.dropdown.value;
        group.preview.text = upgradeManager.GetUpgradePreview(group.skillType, upgradeType);
    }

    void UpdateScrollText()
    {
        if (scrollCountText != null)
            scrollCountText.text = $"🔷 Scroll: {scrollInventory.scrollCount}";
    }

    void UpdateButtonStates()
    {
        if (scrollInventory == null) return;
        bool hasScroll = scrollInventory.scrollCount > 0;

        foreach (var g in _groups)
        {
            if (g.button == null) continue;
            g.button.interactable = hasScroll;
        }
    }

    void ShowFeedback(bool success, string skillName, string upgradeName)
    {
        if (feedbackText == null) return;
        if (_feedbackCoroutine != null) StopCoroutine(_feedbackCoroutine);
        _feedbackCoroutine = StartCoroutine(FeedbackRoutine(success, skillName, upgradeName));
    }

    IEnumerator FeedbackRoutine(bool success, string skillName, string upgradeName)
    {
        feedbackText.text = success
            ? $"✓ {skillName} — {upgradeName} berhasil!"
            : "✗ Scroll tidak cukup!";
        feedbackText.color = success ? Color.green : Color.red;
        feedbackText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        feedbackText.gameObject.SetActive(false);
    }

    public void OnScrollChanged() => RefreshAll();
}