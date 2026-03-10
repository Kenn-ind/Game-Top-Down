using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class movement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastDirection = Vector2.down;

    private Animator animator;
    private PlayerAttack playerAttack;
    private PlayerHealth playerHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerAttack = GetComponent<PlayerAttack>();
        playerHealth = GetComponent<PlayerHealth>();

        animator.SetFloat("LastInputX", lastDirection.x);
        animator.SetFloat("LastInputY", lastDirection.y);
    }

    void Update()
    {
        if (playerHealth != null && playerHealth.isKnockback)
        {
            return;
        }

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

        if (context.performed)
        {
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
                lastDirection = new Vector2(Mathf.Sign(moveInput.x), 0);
            else
                lastDirection = new Vector2(0, Mathf.Sign(moveInput.y));

            animator.SetFloat("LastInputX", lastDirection.x);
            animator.SetFloat("LastInputY", lastDirection.y);
        }

        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
    }
}