using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    private DialogueController dialogueUI;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    private void Start()
    {
        dialogueUI = DialogueController.Instance;
        if (dialogueUI == null)
            Debug.LogError("DialogueController.Instance tidak ditemukan! Pastikan ada di scene.");
    }

    public bool CanInteract()
    {
        return true;
    }

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
        if (dialogueUI.dialoguePanel == null) { Debug.LogError("dialoguePanel NULL!"); return; }
        if (dialogueUI.nameText == null) { Debug.LogError("nameText NULL!"); return; }
        if (dialogueUI.portraitImage == null) { Debug.LogError("portraitImage NULL!"); return; }

        isDialogueActive = true;
        dialogueIndex = 0;
        dialogueUI.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);
        dialogueUI.ShowDialogueUI(true);
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

        if (dialogueData.endDialogueLines.Length > dialogueIndex && dialogueData.endDialogueLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }

        foreach (DialogueChoice dialogueChoice in dialogueData.choices)
        {
            Debug.Log($"Cek choice: dialogueChoice.dialogueIndex={dialogueChoice.dialogueIndex}, dialogueIndex={dialogueIndex}");
            if (dialogueChoice.dialogueIndex == dialogueIndex)
            {
                Debug.Log("Choice ditemukan! Memanggil DisplayChoices...");
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
        Debug.Log($"DisplayChoices dipanggil, jumlah pilihan: {choice.choices.Length}");
        for (int i = 0; i < choice.choices.Length; i++)
        {
            Debug.Log($"Membuat button: {choice.choices[i]}");
            int nextIndex = choice.nextDialogueIndexes[i];
            dialogueUI.CreateChoiceButton(choice.choices[i], () => ChooseOption(nextIndex));
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