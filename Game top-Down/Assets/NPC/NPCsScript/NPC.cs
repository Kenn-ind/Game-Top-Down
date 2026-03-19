using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;

    public QuestGiver questGiver;

    private DialogueController dialogueUI;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    private void Start()
    {
        dialogueUI = DialogueController.Instance;
        if (dialogueUI == null)
            Debug.LogError("DialogueController.Instance tidak ditemukan! Pastikan ada di scene.");
    }

    public bool CanInteract() => true;

    public void Interact()
    {
        if (dialogueData == null) return;
        if (dialogueUI == null) return;


        if (isDialogueActive)
            NextLine();
        else
            StartDialogue();
    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueUI.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);
        dialogueUI.ShowDialogueUI(true);

        if (questGiver != null && questGiver.IsCompleted())
            dialogueIndex = questGiver.turnInDialogueIndex;
        else if (questGiver != null && questGiver.IsActive())
            dialogueIndex = questGiver.ongoingDialogueIndex;
        else
            dialogueIndex = 0;

        DisplayCurrentLine();
    }

    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueUI.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }

        dialogueUI.ClearChoices();

        if (dialogueData.endDialogueLines.Length > dialogueIndex &&
            dialogueData.endDialogueLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }

        foreach (DialogueChoice dialogueChoice in dialogueData.choices)
        {
            if (dialogueChoice.dialogueIndex == dialogueIndex)
            {
                DisplayChoices(dialogueChoice);
                return;
            }
        }

        if (++dialogueIndex < dialogueData.dialogueLines.Length)
            DisplayCurrentLine();
        else
            EndDialogue();
    }


    IEnumerator TypeLine()
    {
        isTyping = true;
        string fullLine = dialogueData.dialogueLines[dialogueIndex];
        string current = "";

        foreach (char letter in fullLine)
        {
            current += letter;
            dialogueUI.SetDialogueText(current);
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        if (dialogueData.autoProgressLines != null &&
            dialogueIndex < dialogueData.autoProgressLines.Length &&
            dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    void DisplayChoices(DialogueChoice choice)
    {
        bool anyButtonShown = false;

        for (int i = 0; i < choice.choices.Length; i++)
        {
            int nextIndex = choice.nextDialogueIndexes[i];
            string choiceText = choice.choices[i];

            if (questGiver != null && choiceText.StartsWith("[QUEST]"))
            {
                if (questGiver.IsActive() || questGiver.IsTurnedIn()) continue;

                string displayText = choiceText.Replace("[QUEST]", "").Trim();
                dialogueUI.CreateChoiceButton(displayText, () =>
                {
                    questGiver.TryAccept();
                    ChooseOption(nextIndex);
                });
                anyButtonShown = true;
            }
            else if (questGiver != null && choiceText.StartsWith("[TURNIN]"))
            {
                if (!questGiver.IsCompleted()) continue;

                string displayText = choiceText.Replace("[TURNIN]", "").Trim();
                dialogueUI.CreateChoiceButton(displayText, () =>
                {
                    questGiver.TryTurnIn();
                    ChooseOption(nextIndex);
                });
                anyButtonShown = true;
            }
            else
            {
                dialogueUI.CreateChoiceButton(choiceText, () => ChooseOption(nextIndex));
                anyButtonShown = true;
            }
        }

        if (!anyButtonShown)
        {
            if (++dialogueIndex < dialogueData.dialogueLines.Length)
                DisplayCurrentLine();
            else
                EndDialogue();
        }
    }

    void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    void ChooseOption(int nextIndex)
    {
        dialogueIndex = nextIndex;
        dialogueUI.ClearChoices();
        DisplayCurrentLine();
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);
        PauseController.SetPause(false);
    }
}