using UnityEngine;

public class TutorialInteractable : MonoBehaviour, IInteractable
{
    [Header("Dialogue sebelum tutorial")]
    public NPCDialogue introDialogue;

    private bool tutorialStarted = false;
    private bool isDialogueActive = false;
    private int currentLineIndex = 0;
    private Coroutine typingCoroutine;

    public bool CanInteract() => !tutorialStarted;

    public void Interact()
    {
        if (tutorialStarted) return;

        if (!isDialogueActive)
            StartIntroDialogue();
        else
            AdvanceDialogue();
    }

    void StartIntroDialogue()
    {
        if (introDialogue == null)
        {
            // Tidak ada dialogue, langsung mulai tutorial
            BeginTutorial();
            return;
        }

        isDialogueActive = true;
        currentLineIndex = 0;
        DialogueController.Instance.ShowDialogueUI(true);
        DialogueController.Instance.SetNPCInfo(introDialogue.npcName, introDialogue.npcPortrait);
        DialogueController.Instance.SetDialogueText(introDialogue.dialogueLines[0]);
        DialogueController.Instance.SetCurrentInteractable(this);
    }

    void AdvanceDialogue()
    {
        if (introDialogue == null) return;

        currentLineIndex++;
        if (currentLineIndex < introDialogue.dialogueLines.Length)
        {
            DialogueController.Instance.SetDialogueText(
                introDialogue.dialogueLines[currentLineIndex]);
        }
        else
        {
            // Dialog selesai, mulai tutorial
            DialogueController.Instance.ShowDialogueUI(false);
            DialogueController.Instance.SetCurrentInteractable(null);
            isDialogueActive = false;
            BeginTutorial();
        }
    }

    void BeginTutorial()
    {
        tutorialStarted = true;
        TutorialManager.Instance?.StartTutorial();
    }
}