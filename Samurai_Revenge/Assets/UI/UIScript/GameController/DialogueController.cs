using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set; }
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;
    public Transform choiceContainer;
    public GameObject choiceButton;
    public GameObject mobileUIPanel;

    // ─── Mobile Next Line ─────────────────────────────────────────────────────
    private IInteractable currentInteractable;

    public void SetCurrentInteractable(IInteractable interactable)
    {
        currentInteractable = interactable;
    }

    public void OnMobileNextLine()
    {
        currentInteractable?.Interact();
    }
    // ─────────────────────────────────────────────────────────────────────────

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowDialogueUI(bool show)
    {
        dialoguePanel.SetActive(show);
        HotbarController.Instance.SetHotbarVisible(!show);
        if (mobileUIPanel != null)
            mobileUIPanel.SetActive(!show);
        if (QuestUI.Instance?.trackerButton != null)
            QuestUI.Instance.trackerButton.gameObject.SetActive(!show);
    }

    public void SetNPCInfo(string npcName, Sprite portrait)
    {
        nameText.text = npcName;
        portraitImage.sprite = portrait;
    }

    public void SetDialogueText(string text)
    {
        dialogueText.text = text;
    }

    public void ClearChoices()
    {
        foreach (Transform child in choiceContainer) Destroy(child.gameObject);
    }

    public GameObject CreateChoiceButton(string choiceText, UnityEngine.Events.UnityAction onclick)
    {
        GameObject btn = Instantiate(this.choiceButton, choiceContainer);
        btn.GetComponentInChildren<TMP_Text>().text = choiceText;
        btn.GetComponent<Button>().onClick.AddListener(onclick);
        return btn;
    }
}