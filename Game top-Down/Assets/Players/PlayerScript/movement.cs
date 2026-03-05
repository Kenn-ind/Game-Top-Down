using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class movement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private PlayerAttack playerAttack;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    void Update()
    {
        if (playerAttack != null && playerAttack.IsAttacking())
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("IsWalking", false);
        }
        else
        {
            rb.velocity = moveInput * speed;

            if (moveInput != Vector2.zero)
                animator.SetBool("IsWalking", true);
            else
                animator.SetBool("IsWalking", false);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (context.canceled)
        {
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }

        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
    }
}