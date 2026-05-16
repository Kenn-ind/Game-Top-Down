using System.Collections;
using UnityEngine;

public class BossDialogueTrigger : MonoBehaviour, IInteractable
{
    [Header("Dialogue")]
    public NPCDialogue bossDialogue;
    public int fightTriggerDialogueIndex;

    [Header("Settings")]
    public float interactRadius = 2f;

    private bool fightStarted = false;
    private bool isDialogueActive = false;
    private int currentLineIndex = 0;
    private NPCDialogue currentDialogue;
    private Coroutine typingCoroutine;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    // ─── IInteractable ───────────────────────────────────────────────────────────

    public bool CanInteract()
    {
        return !fightStarted;
    }

    public void Interact()
    {
        HandleInteract();
    }

    // ─── Mobile ──────────────────────────────────────────────────────────────────

    public void MobileInteract()
    {
        if (fightStarted) return;
        HandleInteract();
    }

    // ─── Dialogue Logic ──────────────────────────────────────────────────────────

    void HandleInteract()
    {
        if (!isDialogueActive)
            StartDialogue(bossDialogue);
        else
            AdvanceDialogue();
    }

    void StartDialogue(NPCDialogue dialogue)
    {
        if (dialogue == null) return;

        currentDialogue = dialogue;
        currentLineIndex = 0;
        isDialogueActive = true;

        DialogueController.Instance.ShowDialogueUI(true);
        DialogueController.Instance.SetNPCInfo(dialogue.npcName, dialogue.npcPortrait);
        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        if (currentDialogue == null) return;

        string line = currentDialogue.dialogueLines[currentLineIndex];
        DialogueController.Instance.SetDialogueText(line);
        DialogueController.Instance.ClearChoices();

        // Cek apakah ada choice di index ini
        foreach (DialogueChoice choice in currentDialogue.choices)
        {
            if (choice.dialogueIndex == currentLineIndex)
            {
                ShowChoices(choice);
                return;
            }
        }

        // Auto progress
        bool autoProgress = currentDialogue.autoProgressLines != null
                         && currentLineIndex < currentDialogue.autoProgressLines.Length
                         && currentDialogue.autoProgressLines[currentLineIndex];
        if (autoProgress)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(AutoAdvance());
        }
    }

    void ShowChoices(DialogueChoice choice)
    {
        for (int i = 0; i < choice.choices.Length; i++)
        {
            int nextIndex = choice.nextDialogueIndexes[i];
            string choiceText = choice.choices[i];

            DialogueController.Instance.CreateChoiceButton(choiceText, () =>
            {
                DialogueController.Instance.ClearChoices();
                currentLineIndex = nextIndex;
                ShowCurrentLine();
            });
        }
    }

    void AdvanceDialogue()
    {
        if (currentDialogue == null) return;

        // Cek apakah ini line fight trigger
        if (currentLineIndex == fightTriggerDialogueIndex)
        {
            EndDialogueAndFight();
            return;
        }

        // Cek end dialogue
        bool isEndLine = currentDialogue.endDialogueLines != null
                      && currentLineIndex < currentDialogue.endDialogueLines.Length
                      && currentDialogue.endDialogueLines[currentLineIndex];

        if (isEndLine)
        {
            EndDialogue();
            return;
        }

        // Lanjut ke line berikutnya
        currentLineIndex++;
        if (currentLineIndex < currentDialogue.dialogueLines.Length)
            ShowCurrentLine();
        else
            EndDialogueAndFight();
    }

    IEnumerator AutoAdvance()
    {
        yield return new WaitForSeconds(currentDialogue.autoProgressDelay);
        AdvanceDialogue();
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        currentDialogue = null;
        DialogueController.Instance.ShowDialogueUI(false);
        DialogueController.Instance.ClearChoices();

        QuestGiver qg = GetComponent<QuestGiver>();
        if (qg != null)
            QuestManager.Instance?.ReportTalk(qg.npcID);
    }

    void EndDialogueAndFight()
    {
        EndDialogue();
        BossFightManager.Instance?.StartBossFight();
        fightStarted = true;
        Debug.Log("[BossDialogueTrigger] Boss fight dimulai!");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}