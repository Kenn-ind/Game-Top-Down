using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ChestController : MonoBehaviour
{
    public ChestData chestData;

    public float interactRange = 1.5f;
    public KeyCode interactKey = KeyCode.F;

    public ChestUI chestUI;

    public GameObject interactPrompt;

    private static readonly int ParamIsOpen = Animator.StringToHash("IsOpen");

    private Animator animator;
    private Transform player;
    private bool isOpen = false;
    private bool playerInRange = false;
    private InventoryController playerInventory;


    private movement playerMovement;
    private List<ChestData.ChestItem> runtimeItems;
    public List<ChestData.ChestItem> GetRuntimeItems() => runtimeItems;

    public static bool IsChestOpen => CurrentOpenChest != null;

    public static ChestController CurrentOpenChest { get; private set; }

    private bool _permanentlyOpened = false;
    public bool IsOpened => _permanentlyOpened;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerMovement = player.GetComponent<movement>();
        playerInventory = FindObjectOfType<InventoryController>();


        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        runtimeItems = new List<ChestData.ChestItem>(chestData.items);
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        playerInRange = dist <= interactRange;

        if (interactPrompt != null)
            interactPrompt.SetActive(playerInRange && !isOpen);

        if (playerInRange && !isOpen && Input.GetKeyDown(KeyCode.F))
            OpenChest();

        else if (isOpen && Input.GetKeyDown(KeyCode.F))
            CloseUI();
    }

    void OpenChest()
    {
        isOpen = true;
        _permanentlyOpened = true;
        CurrentOpenChest = this;
        animator.SetBool(ParamIsOpen, true);
        chestUI.Open(runtimeItems, this); 
        playerInventory.MoveToChestPanel();
        if (playerMovement != null) playerMovement.SetMovementLocked(true);
    }

    public void CloseUI()
    {
        chestUI.Close();
        playerInventory.MoveToInventoryPanel();
        CurrentOpenChest = null;
        isOpen = false;
        if (playerMovement != null) playerMovement.SetMovementLocked(false);
    }

    public void RemoveItemFromChest(Slot slot)
    {
        if (slot.currentItem == null) return;
        Destroy(slot.currentItem);
        slot.currentItem = null;
    }

    public void SetOpenedState(bool opened)
    {
        _permanentlyOpened = opened;
        if (opened)
        {
            isOpen = true;
            if (animator != null)
                animator.SetBool(ParamIsOpen, true);
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}