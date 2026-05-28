using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("Tutorial Steps")]
    public TutorialStep[] steps;

    [Header("References")]
    public GameObject tutorialPanel;
    public TMP_Text titleText;
    public TMP_Text progressText;
    public Button descButton;

    [Header("UI Skill References")]
    public GameObject skillUI1;
    public GameObject skillUI2;
    public GameObject skillUI3;
    public GameObject ultUI;

    private int currentStepIndex = -1;
    private int currentActionCount = 0;
    private bool isTutorialActive = false;

    public bool IsTutorialActive => isTutorialActive;
    public TutorialActionType CurrentRequiredAction =>
        currentStepIndex >= 0 && currentStepIndex < steps.Length
        ? steps[currentStepIndex].requiredAction
        : TutorialActionType.MeleeAttack;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        tutorialPanel?.SetActive(false);

        SetSkillUIVisible(skillUI1, false);
        SetSkillUIVisible(skillUI2, false);
        SetSkillUIVisible(skillUI3, false);
        SetSkillUIVisible(ultUI, false);

        // Tombol desc untuk buka ulang panel deskripsi
        if (descButton != null)
            descButton.onClick.AddListener(() =>
            {
                Debug.Log("[Tutorial] DescButton diklik!");
                TutorialDescPanel.Instance?.ReopenPanel();
            });
    }

    // ─── Start Tutorial ───────────────────────────────────────────────────────

    public void StartTutorial()
    {
        if (isTutorialActive) return;
        isTutorialActive = true;
        currentStepIndex = 0;
        currentActionCount = 0;
        ShowCurrentStep();
    }

    // ─── Show Step ────────────────────────────────────────────────────────────

    void ShowCurrentStep()
    {
        if (currentStepIndex >= steps.Length)
        {
            EndTutorial();
            return;
        }

        TutorialStep step = steps[currentStepIndex];

        tutorialPanel?.SetActive(true);
        if (titleText != null) titleText.text = step.title;
        UpdateProgressText();
        UpdateSkillVisibility();

        // Jika step Ult, langsung force ready
        if (step.requiredAction == TutorialActionType.Ult)
        {
            PlayerUlt playerUlt = FindObjectOfType<PlayerUlt>();
            playerUlt?.ForceUltReady();
        }

        // Tampilkan desc panel otomatis saat step baru
        if (step.descPages != null && step.descPages.Count > 0)
            TutorialDescPanel.Instance?.ShowPages(step.descPages);

        Debug.Log($"[Tutorial] Step {currentStepIndex + 1}/{steps.Length}: {step.title}");
    }

    void UpdateProgressText()
    {
        if (progressText == null || currentStepIndex >= steps.Length) return;
        TutorialStep step = steps[currentStepIndex];
        progressText.text = $"{currentActionCount}/{step.requiredCount}";
    }

    // ─── Skill Visibility ─────────────────────────────────────────────────────

    void UpdateSkillVisibility()
    {
        TutorialActionType action = steps[currentStepIndex].requiredAction;

        SetSkillUIVisible(skillUI1, action >= TutorialActionType.Skill1Range);
        SetSkillUIVisible(skillUI2, action >= TutorialActionType.Skill2Range);
        SetSkillUIVisible(skillUI3, action >= TutorialActionType.Skill3Melee); // ← ubah dari Skill3Range
        SetSkillUIVisible(ultUI, action >= TutorialActionType.Ult);
    }

    void SetSkillUIVisible(GameObject ui, bool visible)
    {
        if (ui != null) ui.SetActive(visible);
    }

    // ─── Report Action ────────────────────────────────────────────────────────

    public void ReportAction(TutorialActionType actionType)
    {
        if (!isTutorialActive) return;
        if (currentStepIndex >= steps.Length) return;

        TutorialStep step = steps[currentStepIndex];

        Debug.Log($"[Tutorial] ReportAction: {actionType}, required: {step.requiredAction}, count: {currentActionCount}/{step.requiredCount}");

        if (step.requiredAction != actionType) return;

        currentActionCount++;
        UpdateProgressText();

        if (currentActionCount >= step.requiredCount)
            StartCoroutine(AdvanceStep());
    }

    IEnumerator AdvanceStep()
    {
        yield return new WaitForSeconds(0.5f);

        currentStepIndex++;
        currentActionCount = 0;

        if (currentStepIndex >= steps.Length)
            EndTutorial();
        else
            ShowCurrentStep();
    }

    // ─── End Tutorial ─────────────────────────────────────────────────────────

    void EndTutorial()
    {
        isTutorialActive = false;
        tutorialPanel?.SetActive(false);
        TutorialDescPanel.Instance?.ClosePanel();

        SetSkillUIVisible(skillUI1, true);
        SetSkillUIVisible(skillUI2, true);
        SetSkillUIVisible(skillUI3, true);
        SetSkillUIVisible(ultUI, true);

        QuestManager.Instance?.ReportTalk("tutorial_complete");
        Debug.Log("[Tutorial] Tutorial selesai!");
    }
}