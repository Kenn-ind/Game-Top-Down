using System.Collections;
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
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowDialogueUI(bool show)
    {
        dialoguePanel.SetActive(show);
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
        Debug.Log($"CreateChoiceButton dipanggil: {choiceText}");
        Debug.Log($"choiceContainer: {choiceContainer}, choiceButton prefab: {this.choiceButton}");

        GameObject btn = Instantiate(this.choiceButton, choiceContainer);
        Debug.Log($"Button berhasil di-instantiate: {btn.name}");

        btn.GetComponentInChildren<TMP_Text>().text = choiceText;
        btn.GetComponent<Button>().onClick.AddListener(onclick);
        return btn;
    }
}
