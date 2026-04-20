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

    // Dipanggil ChestUI saat item di-shift+click dari chest → inventory
    public void RemoveItemFromChest(Slot slot)
    {
        if (slot.currentItem == null) return;
        Destroy(slot.currentItem);
        slot.currentItem = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}