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
    private PlayerStamina _stamina;
    private PlayerUlt _ult;

    public float dashSpeed = 15f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;
    public int dashStaminaCost = 3;

    public float slowTimeScale = 0.3f;
    public float slowDuration = 0.2f;

    private bool _isDashing = false;
    private bool _canDash = true;
    private float _dashCooldownTimer = 0f;

    private bool _isLocked = false;
    public bool IsLocked => _isLocked;
    public Vector2 LastDirection => lastDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerAttack = GetComponent<PlayerAttack>();
        playerHealth = GetComponent<PlayerHealth>();
        _stamina = GetComponent<PlayerStamina>();
        _ult = GetComponent<PlayerUlt>();

        animator.SetFloat("LastInputX", lastDirection.x);
        animator.SetFloat("LastInputY", lastDirection.y);
    }

    void Update()
    {
        if (_isLocked) return;

        if (!_canDash)
        {
            _dashCooldownTimer -= Time.unscaledDeltaTime;
            if (_dashCooldownTimer <= 0f)
                _canDash = true;
        }

        if (_isDashing) return;

        if (playerHealth != null && playerHealth.isKnockback) return;

        if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash)
        {
            if (_stamina != null && !_stamina.UseStamina(dashStaminaCost))
            {
                Debug.Log("Stamina tidak cukup untuk dash!");
            }
            else
            {
                StartCoroutine(DoDash());
            }
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
            animator.SetBool("IsWalking", moveInput != Vector2.zero);
        }
    }

    IEnumerator DoDash()
    {
        _isDashing = true;
        _canDash = false;
        _dashCooldownTimer = dashCooldown;

        Vector2 dashDir = moveInput != Vector2.zero ? moveInput.normalized : lastDirection.normalized;

        if (playerHealth != null) playerHealth.isKnockback = true;

        Time.timeScale = slowTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        float timer = 0f;
        while (timer < dashDuration)
        {
            rb.velocity = dashDir * dashSpeed;
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        _isDashing = false;

        float slowTimer = 0f;
        while (slowTimer < slowDuration)
        {
            slowTimer += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        if (playerHealth != null) playerHealth.isKnockback = false;
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (_isLocked) return;

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
    public void SetMovementLocked(bool locked)
    {
        _isLocked = locked;
        if (locked)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("IsWalking", false);
            animator.SetFloat("InputX", 0f);
            animator.SetFloat("InputY", 0f);
        }
    }

    public void FaceTowards(Vector2 targetPosition)
    {
        Vector2 dir = (targetPosition - (Vector2)transform.position);
        float absX = Mathf.Abs(dir.x);
        float absY = Mathf.Abs(dir.y);

        if (absX > absY)
            lastDirection = dir.x > 0 ? Vector2.right : Vector2.left;
        else
            lastDirection = dir.y > 0 ? Vector2.up : Vector2.down;

        animator.SetFloat("LastInputX", lastDirection.x);
        animator.SetFloat("LastInputY", lastDirection.y);
    }
}