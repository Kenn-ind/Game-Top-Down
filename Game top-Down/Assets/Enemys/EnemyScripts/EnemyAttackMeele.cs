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

    public float attackAnimDuration = 0.5f;

    private enum State { Patrol, Chase, Attack }
    private State currentState = State.Patrol;

    private Vector2 homePosition;
    private Vector2 currentPatrolTarget;
    private float patrolWaitTimer;
    private bool isWaiting = false;

    private float cooldownTimer;

    private Vector2 lastMoveDir = Vector2.down;

    private float attackAnimTimer;
    private bool isPlayingAttackAnim = false;

    private Animator animator;

    private static readonly int ParamMoveX = Animator.StringToHash("MoveX");
    private static readonly int ParamMoveY = Animator.StringToHash("MoveY");
    private static readonly int ParamIsMoving = Animator.StringToHash("IsMoving");
    private static readonly int ParamIsAttacking = Animator.StringToHash("IsAttacking");
    private static readonly int ParamAttackX = Animator.StringToHash("AttackX");
    private static readonly int ParamAttackY = Animator.StringToHash("AttackY");

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


    void UpdateAnimator()
    {
        Vector2 cardinal = DominantCardinal(lastMoveDir);
        animator.SetFloat(ParamMoveX, cardinal.x);
        animator.SetFloat(ParamMoveY, cardinal.y);
    }

    void TriggerAttackAnim()
    {
        Vector2 cardinal = DominantCardinal(lastMoveDir);

        animator.SetFloat(ParamAttackX, cardinal.x);
        animator.SetFloat(ParamAttackY, cardinal.y);
        animator.SetBool(ParamIsMoving, false);      // ← matikan walk dulu
        animator.SetBool(ParamIsAttacking, true);       // ← baru nyalakan attack

        isPlayingAttackAnim = true;
        attackAnimTimer = attackAnimDuration;
    }

    void SetMoving(bool moving)
    {
        animator.SetBool(ParamIsMoving, moving);
    }


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

        lastMoveDir = direction;

        Vector2 cardinal = DominantCardinal(direction);

        animator.SetBool(ParamIsMoving, true);
        animator.SetFloat(ParamMoveX, cardinal.x);
        animator.SetFloat(ParamMoveY, cardinal.y);

        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }


    Vector2 DominantCardinal(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
            return new Vector2(Mathf.Sign(dir.x), 0f);
        else
            return new Vector2(0f, Mathf.Sign(dir.y));
    }


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