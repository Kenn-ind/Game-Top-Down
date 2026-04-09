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

    private movement GetPlayer()
    {
        return FindObjectOfType<movement>();
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
        GetPlayer()?.SetMovementLocked(true);

        if (questGiver != null)
        {
            bool completed = questGiver.IsCompleted();
            bool active = questGiver.IsActive();
            bool turnedIn = questGiver.IsTurnedIn();

            Debug.Log($"[NPC] Quest State → IsCompleted: {completed}, IsActive: {active}, IsTurnedIn: {turnedIn}");
            Debug.Log($"[NPC] turnInDialogueIndex: {questGiver.turnInDialogueIndex}, ongoingDialogueIndex: {questGiver.ongoingDialogueIndex}");

            if (completed && !turnedIn)
                dialogueIndex = questGiver.turnInDialogueIndex;
            else if (active)
                dialogueIndex = questGiver.ongoingDialogueIndex;
            else
                dialogueIndex = 0;
        }
        else
        {
            dialogueIndex = 0;
        }

        Debug.Log($"[NPC] StartDialogue → dialogueIndex: {dialogueIndex}");
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
            Debug.Log($"[NPC] NextLine → endDialogueLine TRUE di index {dialogueIndex}, EndDialogue dipanggil!");
            EndDialogue();
            return;
        }

        bool choiceFound = false;
        foreach (DialogueChoice dialogueChoice in dialogueData.choices)
        {
            Debug.Log($"[NPC] Checking choice → dialogueChoice.dialogueIndex: {dialogueChoice.dialogueIndex}, current: {dialogueIndex}");
            if (dialogueChoice.dialogueIndex == dialogueIndex)
            {
                choiceFound = true;
                DisplayChoices(dialogueChoice);
                return;
            }
        }

        if (!choiceFound)
            Debug.LogWarning($"[NPC] Tidak ada choice ditemukan untuk dialogueIndex: {dialogueIndex}");

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

            Debug.Log($"[NPC] Processing choice: '{choiceText}'");

            if (questGiver != null && choiceText.StartsWith("[QUEST]"))
            {
                if (questGiver.IsActive() || questGiver.IsTurnedIn())
                {
                    Debug.Log($"[NPC] [QUEST] skipped → IsActive: {questGiver.IsActive()}, IsTurnedIn: {questGiver.IsTurnedIn()}");
                    continue;
                }

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
                bool completed = questGiver.IsCompleted();
                bool turnedIn = questGiver.IsTurnedIn();
                Debug.Log($"[NPC] [TURNIN] check → IsCompleted: {completed}, IsTurnedIn: {turnedIn}");

                // Tampilkan jika quest selesai dan belum di-turn in
                if (!completed || turnedIn)
                {
                    Debug.Log($"[NPC] [TURNIN] skipped!");
                    continue;
                }

                string displayText = choiceText.Replace("[TURNIN]", "").Trim();
                dialogueUI.CreateChoiceButton(displayText, () =>
                {
                    questGiver.TryTurnIn();
                    ChooseOption(nextIndex);
                });
                anyButtonShown = true;
            }
            else if (choiceText.StartsWith("[SHOP]"))
            {
                string displayText = choiceText.Replace("[SHOP]", "").Trim();
                dialogueUI.CreateChoiceButton(displayText, () =>
                {
                    EndDialogue();
                    GetComponent<NPCShop>()?.OpenShop();
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
            Debug.LogWarning($"[NPC] Tidak ada choice yang ditampilkan di dialogueIndex: {dialogueIndex}, lanjut ke baris berikutnya.");
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
        GetPlayer()?.SetMovementLocked(false);
        PauseController.SetPause(false);
    }
}