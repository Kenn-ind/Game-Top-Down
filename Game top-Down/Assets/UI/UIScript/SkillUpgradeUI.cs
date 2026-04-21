using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SkillUpgradeUI : MonoBehaviour
{
    [Header("Manager & Inventory")]
    public SkillUpgradeManager upgradeManager;
    public ScrollInventory scrollInventory;

    [Header("Melee UI")]
    public TMP_Dropdown meleeDropdown;
    public Button meleeUpgradeButton;
    public TextMeshProUGUI meleePreviewText;   // teks preview "Damage: 1 → 2"

    [Header("Range UI")]
    public TMP_Dropdown rangeDropdown;
    public Button rangeUpgradeButton;
    public TextMeshProUGUI rangePreviewText;

    [Header("Shared UI")]
    public TextMeshProUGUI scrollCountText;    // "Scroll: 3"
    public TextMeshProUGUI feedbackText;       // "✓ Berhasil!" / "✗ Gagal!"

    private Coroutine _feedbackCoroutine;

    void Start()
    {
        SetupDropdowns();
        SetupButtons();
        RefreshAll();
    }

    void SetupDropdowns()
    {
        // Melee options
        meleeDropdown.ClearOptions();
        meleeDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "⚔ Damage +1",
            "⏱ Cooldown -0.5s",
            "💨 Stamina Cost -1",
            "🔁 Dash Count +1"
        });

        // Range options
        rangeDropdown.ClearOptions();
        rangeDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "⚔ Damage +1",
            "⏱ Cooldown -0.5s",
            "💨 Stamina Cost -1",
            "🌀 Shuriken +1"
        });

        // Update preview saat dropdown berubah
        meleeDropdown.onValueChanged.AddListener(_ => RefreshAll());
        rangeDropdown.onValueChanged.AddListener(_ => RefreshAll());
    }

    void SetupButtons()
    {
        meleeUpgradeButton.onClick.AddListener(OnMeleeUpgrade);
        rangeUpgradeButton.onClick.AddListener(OnRangeUpgrade);
    }

    void OnMeleeUpgrade()
    {
        var upgradeType = (SkillUpgradeManager.UpgradeType)meleeDropdown.value;
        bool success = upgradeManager.TryUpgrade(SkillUpgradeManager.SkillType.Melee, upgradeType);
        ShowFeedback(success, "Melee", meleeDropdown.options[meleeDropdown.value].text);
        RefreshAll();
    }

    void OnRangeUpgrade()
    {
        var upgradeType = (SkillUpgradeManager.UpgradeType)rangeDropdown.value;
        bool success = upgradeManager.TryUpgrade(SkillUpgradeManager.SkillType.Range, upgradeType);
        ShowFeedback(success, "Range", rangeDropdown.options[rangeDropdown.value].text);
        RefreshAll();
    }

    void RefreshAll()
    {
        UpdateScrollText();
        UpdatePreviews();
        UpdateButtonStates();
    }

    void UpdateScrollText()
    {
        if (scrollCountText != null)
            scrollCountText.text = $"🔷 Scroll: {scrollInventory.scrollCount}";
    }

    void UpdatePreviews()
    {
        if (meleePreviewText != null)
        {
            var meleeUpgrade = (SkillUpgradeManager.UpgradeType)meleeDropdown.value;
            meleePreviewText.text = upgradeManager.GetUpgradePreview(
                SkillUpgradeManager.SkillType.Melee, meleeUpgrade);
        }

        if (rangePreviewText != null)
        {
            var rangeUpgrade = (SkillUpgradeManager.UpgradeType)rangeDropdown.value;
            rangePreviewText.text = upgradeManager.GetUpgradePreview(
                SkillUpgradeManager.SkillType.Range, rangeUpgrade);
        }
    }

    void UpdateButtonStates()
    {
        bool hasScroll = scrollInventory.scrollCount > 0;
        meleeUpgradeButton.interactable = hasScroll;
        rangeUpgradeButton.interactable = hasScroll;

        // Ganti warna teks button jika tidak bisa upgrade
        Color btnColor = hasScroll ? Color.white : Color.gray;
        meleeUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().color = btnColor;
        rangeUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().color = btnColor;
    }

    void ShowFeedback(bool success, string skillName, string upgradeName)
    {
        if (feedbackText == null) return;

        if (_feedbackCoroutine != null)
            StopCoroutine(_feedbackCoroutine);

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