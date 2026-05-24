using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("Tutorial Steps")]
    public TutorialStep[] steps;

    [Header("References")]
    public GameObject tutorialPanel;
    public TMPro.TMP_Text titleText;
    public TMPro.TMP_Text descriptionText;
    public TMPro.TMP_Text progressText;
    public GameObject mobileNextButton;

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

        // Sembunyikan semua skill UI di awal
        SetSkillUIVisible(skillUI1, false);
        SetSkillUIVisible(skillUI2, false);
        SetSkillUIVisible(skillUI3, false);
        SetSkillUIVisible(ultUI, false);
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
        if (descriptionText != null) descriptionText.text = step.description;
        UpdateProgressText();

        // Update skill UI visibility sesuai step
        UpdateSkillVisibility();

        Debug.Log($"[Tutorial] Step {currentStepIndex + 1}/{steps.Length}: {step.title}");
    }

    void UpdateProgressText()
    {
        if (progressText == null || currentStepIndex >= steps.Length) return;
        TutorialStep step = steps[currentStepIndex];
        progressText.text = $"{currentActionCount}/{step.requiredCount}";
    }

    // ─── Visibility Skill UI ──────────────────────────────────────────────────

    void UpdateSkillVisibility()
    {
        TutorialActionType action = steps[currentStepIndex].requiredAction;

        // Skill 1 muncul mulai step Skill1Range
        bool showSkill1 = action >= TutorialActionType.Skill1Range;
        // Skill 2 muncul mulai step Skill2Range
        bool showSkill2 = action >= TutorialActionType.Skill2Range;
        // Skill 3 muncul mulai step Skill3Range
        bool showSkill3 = action >= TutorialActionType.Skill3Range;
        // Ult muncul mulai step Ult
        bool showUlt = action >= TutorialActionType.Ult;

        SetSkillUIVisible(skillUI1, showSkill1);
        SetSkillUIVisible(skillUI2, showSkill2);
        SetSkillUIVisible(skillUI3, showSkill3);
        SetSkillUIVisible(ultUI, showUlt);
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
        if (step.requiredAction != actionType) return;

        currentActionCount++;
        UpdateProgressText();

        Debug.Log($"[Tutorial] Action {actionType}: {currentActionCount}/{step.requiredCount}");

        if (currentActionCount >= step.requiredCount)
            StartCoroutine(AdvanceStep());
    }

    IEnumerator AdvanceStep()
    {
        // Delay kecil sebelum lanjut step
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

        // Tampilkan semua skill UI
        SetSkillUIVisible(skillUI1, true);
        SetSkillUIVisible(skillUI2, true);
        SetSkillUIVisible(skillUI3, true);
        SetSkillUIVisible(ultUI, true);

        Debug.Log("[Tutorial] Tutorial selesai!");

        // Report quest jika ada
        QuestManager.Instance?.ReportTalk("tutorial_complete");
    }
}