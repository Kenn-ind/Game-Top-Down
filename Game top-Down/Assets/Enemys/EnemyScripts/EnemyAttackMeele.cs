using UnityEngine;

public class EnemyAttackMelee : MonoBehaviour
{
    public Transform player;

    public float moveSpeed = 2f;
    public float chaseSpeed = 3.5f;

    public float detectRange = 6f;
    public float attackRange = 1.5f;
    public float losePlayerRange = 9f;

    public int damage = 10;
    public float attackCooldown = 1.5f;
    public float knockbackForce = 8f;

    public float patrolRadius = 4f;
    public float patrolWaitMin = 1f;
    public float patrolWaitMax = 3f;
    public float patrolPointReachDistance = 0.3f;

    public LayerMask obstacleLayer;
    public float obstacleDetectDistance = 0.8f;
    public float avoidStrength = 2f;

    // ─── Animator attack clip duration ───────────────────────────────────────
    // Set this to match the length (in seconds) of your attack animation clips.
    // The enemy will be "locked" in the attack state for this duration before
    // it can move or transition again.
    public float attackAnimDuration = 0.5f;

    // ─── Internal state ───────────────────────────────────────────────────────
    private enum State { Patrol, Chase, Attack }
    private State currentState = State.Patrol;

    private Vector2 homePosition;
    private Vector2 currentPatrolTarget;
    private float patrolWaitTimer;
    private bool isWaiting = false;

    private float cooldownTimer;

    // Direction the enemy is currently facing (used for idle & attack anim)
    private Vector2 lastMoveDir = Vector2.down; // default: face down like IdleDown

    // Timer that "locks" the enemy inside the attack animation
    private float attackAnimTimer;
    private bool isPlayingAttackAnim = false;

    // ─── Animator ─────────────────────────────────────────────────────────────
    private Animator animator;

    // Animator parameter names
    // Walk / Idle blend tree uses the same params from the screenshot
    private static readonly int ParamMoveX = Animator.StringToHash("MoveX");
    private static readonly int ParamMoveY = Animator.StringToHash("MoveY");
    private static readonly int ParamIsMoving = Animator.StringToHash("IsMoving");

    // Attack params — add these to your Animator Controller:
    //   bool  IsAttacking   (drive AnyState -> AttackXxx transitions)
    //   float AttackX       (same range as MoveX:  -1 / 0 / 1)
    //   float AttackY       (same range as MoveY:  -1 / 0 / 1)
    //
    // Recommended Animator setup for attack transitions (from AnyState):
    //   AnyState -> AttackUp    : IsAttacking = true  AND AttackY > 0.5
    //   AnyState -> AttackDown  : IsAttacking = true  AND AttackY < -0.5
    //   AnyState -> AttackRight : IsAttacking = true  AND AttackX > 0.5
    //   AnyState -> AttackLeft  : IsAttacking = true  AND AttackX < -0.5
    //   (all with Has Exit Time = false, Exit Time = 0)
    //
    // Each attack state should transition back to the blend tree (or Any State)
    // when IsAttacking = false (Has Exit Time can be true here so the clip finishes).
    private static readonly int ParamIsAttacking = Animator.StringToHash("IsAttacking");
    private static readonly int ParamAttackX = Animator.StringToHash("AttackX");
    private static readonly int ParamAttackY = Animator.StringToHash("AttackY");

    // ─────────────────────────────────────────────────────────────────────────

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        homePosition = transform.position;
        PickNewPatrolTarget();
    }

    void Update()
    {
        if (player == null) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        // While attack animation is playing, block all state transitions
        if (isPlayingAttackAnim)
        {
            attackAnimTimer -= Time.deltaTime;
            if (attackAnimTimer <= 0f)
            {
                isPlayingAttackAnim = false;
                animator.SetBool(ParamIsAttacking, false);
            }
            cooldownTimer -= Time.deltaTime;
            return;
        }

        switch (currentState)
        {
            case State.Patrol: HandlePatrol(distToPlayer); break;
            case State.Chase: HandleChase(distToPlayer); break;
            case State.Attack: HandleAttack(distToPlayer); break;
        }

        cooldownTimer -= Time.deltaTime;
        UpdateAnimator();
    }

    // ─── State handlers ───────────────────────────────────────────────────────

    void HandlePatrol(float distToPlayer)
    {
        if (distToPlayer <= detectRange)
        {
            currentState = State.Chase;
            isWaiting = false;
            return;
        }

        if (isWaiting)
        {
            patrolWaitTimer -= Time.deltaTime;
            if (patrolWaitTimer <= 0f)
            {
                isWaiting = false;
                PickNewPatrolTarget();
            }
            SetMoving(false);
            return;
        }

        float distToTarget = Vector2.Distance(transform.position, currentPatrolTarget);
        if (distToTarget <= patrolPointReachDistance)
        {
            isWaiting = true;
            patrolWaitTimer = Random.Range(patrolWaitMin, patrolWaitMax);
            SetMoving(false);
        }
        else
        {
            MoveToward(currentPatrolTarget, moveSpeed);
        }
    }

    void PickNewPatrolTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * patrolRadius;
        currentPatrolTarget = homePosition + randomOffset;
    }

    void HandleChase(float distToPlayer)
    {
        if (distToPlayer > losePlayerRange)
        {
            currentState = State.Patrol;
            homePosition = transform.position;
            PickNewPatrolTarget();
            SetMoving(false);
            return;
        }

        if (distToPlayer <= attackRange)
        {
            currentState = State.Attack;
            SetMoving(false);
            return;
        }

        MoveToward(player.position, chaseSpeed);
    }

    void HandleAttack(float distToPlayer)
    {
        if (distToPlayer > attackRange)
        {
            currentState = State.Chase;
            return;
        }

        if (cooldownTimer <= 0f)
        {
            // Snap facing direction toward player before triggering attack anim
            lastMoveDir = ((Vector2)(player.position - transform.position)).normalized;

            TriggerAttackAnim();
            Attack();
            cooldownTimer = attackCooldown;
        }
    }

    void Attack()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            playerHealth.TakeDamage(damage, direction * knockbackForce);
        }
    }

    // ─── Animator helpers ─────────────────────────────────────────────────────

    /// <summary>
    /// Drives the walk / idle blend tree every frame (except during attack lock).
    /// </summary>
    void UpdateAnimator()
    {
        // These are already set inside MoveToward / SetMoving, but we refresh
        // them here to make sure idle also reflects the last facing direction.
        animator.SetFloat(ParamMoveX, lastMoveDir.x);
        animator.SetFloat(ParamMoveY, lastMoveDir.y);
    }

    /// <summary>
    /// Triggers the 4-directional attack animation and locks movement for its duration.
    /// Sets AttackX / AttackY so the Animator can pick the correct directional clip.
    /// </summary>
    void TriggerAttackAnim()
    {
        // Convert continuous direction to cardinal: pick dominant axis
        Vector2 cardinal = DominantCardinal(lastMoveDir);

        animator.SetFloat(ParamAttackX, cardinal.x);
        animator.SetFloat(ParamAttackY, cardinal.y);
        animator.SetBool(ParamIsAttacking, true);

        // Lock state for the duration of the attack clip
        isPlayingAttackAnim = true;
        attackAnimTimer = attackAnimDuration;
    }

    /// <summary>
    /// Flips IsMoving and records the direction for blend-tree / attack snapping.
    /// </summary>
    void SetMoving(bool moving)
    {
        animator.SetBool(ParamIsMoving, moving);
    }

    // ─── Movement ─────────────────────────────────────────────────────────────

    void MoveToward(Vector2 target, float speed)
    {
        Vector2 direction = ((Vector3)target - transform.position).normalized;

        RaycastHit2D hit = Physics2D.CircleCast(
            transform.position, 0.3f, direction, obstacleDetectDistance, obstacleLayer);

        if (hit.collider != null)
        {
            if (currentState == State.Patrol)
            {
                PickNewPatrolTarget();
                SetMoving(false);
                return;
            }
            else
            {
                Vector2 avoidDir = Vector2.Perpendicular(direction).normalized;

                RaycastHit2D hitLeft = Physics2D.CircleCast(transform.position, 0.3f, avoidDir, obstacleDetectDistance, obstacleLayer);
                RaycastHit2D hitRight = Physics2D.CircleCast(transform.position, 0.3f, -avoidDir, obstacleDetectDistance, obstacleLayer);

                if (hitLeft.collider == null) direction = (direction + avoidDir * avoidStrength).normalized;
                else if (hitRight.collider == null) direction = (direction + -avoidDir * avoidStrength).normalized;
            }
        }

        // Record facing direction for blend tree & attack snap
        lastMoveDir = direction;

        // Drive animator
        animator.SetBool(ParamIsMoving, true);
        animator.SetFloat(ParamMoveX, direction.x);
        animator.SetFloat(ParamMoveY, direction.y);

        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    // ─── Utility ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Maps a normalized direction to the nearest cardinal (Up/Down/Left/Right).
    /// Returns a vector with one axis = ±1 and the other = 0.
    /// </summary>
    Vector2 DominantCardinal(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
            return new Vector2(Mathf.Sign(dir.x), 0f);   // Left or Right
        else
            return new Vector2(0f, Mathf.Sign(dir.y));   // Down or Up
    }

    // ─── Gizmos ───────────────────────────────────────────────────────────────

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Application.isPlaying ? (Vector3)homePosition : transform.position, patrolRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, losePlayerRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}