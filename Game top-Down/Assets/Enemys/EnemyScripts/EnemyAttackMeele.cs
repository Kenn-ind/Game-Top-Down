using UnityEngine;

public class EnemyAttackMelee : MonoBehaviour
{
    public Transform player;

    public float moveSpeed = 2f;
    public float chaseSpeed = 3.5f;

    public float detectRange = 6f;
    public float attackRange = 1.5f;

    public int damage = 10;
    public float attackCooldown = 1.5f;
    public float knockbackForce = 8f;

    public float patrolRadius = 4f;
    public float patrolWaitMin = 1f;
    public float patrolWaitMax = 3f;
    public float patrolPointReachDistance = 0.3f;
    public float returnHomeReachDistance = 0.3f;

    public LayerMask obstacleLayer;
    public float obstacleDetectDistance = 1.2f;
    public int steeringRays = 12;
    public float steeringProbeRadius = 0.25f;

    public float directionSmoothSpeed = 0.15f;

    public float attackAnimDuration = 0.5f;

    public Collider2D chaseZone;

    private enum State { Patrol, Chase, Attack, ReturnHome }
    private State currentState = State.Patrol;

    private Vector2 homePosition;
    private Vector2 currentPatrolTarget;
    private float patrolWaitTimer;
    private bool isWaiting = false;

    private float cooldownTimer;
    private Vector2 lastMoveDir = Vector2.down;

    private Vector2 smoothedDir = Vector2.down;

    private float attackAnimTimer;
    private bool isPlayingAttackAnim = false;

    private BaseEnemy baseEnemy; // tambah field ini

    private Animator animator;

    private static readonly int ParamMoveX = Animator.StringToHash("MoveX");
    private static readonly int ParamMoveY = Animator.StringToHash("MoveY");
    private static readonly int ParamIsMoving = Animator.StringToHash("IsMoving");
    private static readonly int ParamIsAttacking = Animator.StringToHash("IsAttacking");
    private static readonly int ParamAttackX = Animator.StringToHash("AttackX");
    private static readonly int ParamAttackY = Animator.StringToHash("AttackY");

    void Start()
    {
        baseEnemy = GetComponent<BaseEnemy>(); // tambah baris ini
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        homePosition = transform.position;
        PickNewPatrolTarget();
    }

    void Update()
    {
        if (baseEnemy != null && baseEnemy.IsDead()) return; // tambah ini
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
            case State.ReturnHome: HandleReturnHome(distToPlayer); break;
        }

        cooldownTimer -= Time.deltaTime;
        UpdateAnimator();
    }

    bool IsPlayerInChaseZone()
    {
        if (chaseZone == null) return true;
        return chaseZone.OverlapPoint(player.position);
    }

    bool IsEnemyInChaseZone()
    {
        if (chaseZone == null) return true;
        return chaseZone.OverlapPoint(transform.position);
    }

    void HandlePatrol(float distToPlayer)
    {
        if (distToPlayer <= detectRange && IsPlayerInChaseZone())
        {
            currentState = State.Chase;
            isWaiting = false;
            return;
        }

        if (isWaiting)
        {
            patrolWaitTimer -= Time.deltaTime;
            if (patrolWaitTimer <= 0f) { isWaiting = false; PickNewPatrolTarget(); }
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

    void HandleChase(float distToPlayer)
    {
        if (!IsEnemyInChaseZone())
        {
            currentState = State.ReturnHome;
            SetMoving(false);
            return;
        }

        if (!IsPlayerInChaseZone())
        {
            currentState = State.ReturnHome;
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
        if (!IsEnemyInChaseZone() || !IsPlayerInChaseZone())
        {
            currentState = State.ReturnHome;
            SetMoving(false);
            return;
        }

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

    void HandleReturnHome(float distToPlayer)
    {
        if (IsPlayerInChaseZone() && distToPlayer <= detectRange)
        {
            currentState = State.Chase;
            return;
        }

        float distToHome = Vector2.Distance(transform.position, homePosition);
        if (distToHome <= returnHomeReachDistance)
        {
            PickNewPatrolTarget();
            currentState = State.Patrol;
            SetMoving(false);
            return;
        }

        MoveToward(homePosition, moveSpeed);
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
        animator.SetBool(ParamIsMoving, false);
        animator.SetBool(ParamIsAttacking, true);
        isPlayingAttackAnim = true;
        attackAnimTimer = attackAnimDuration;
    }

    void SetMoving(bool moving)
    {
        animator.SetBool(ParamIsMoving, moving);
    }
    Vector2 GetSteeringDirection(Vector2 desiredTarget)
    {
        Vector2 desiredDir = ((Vector3)desiredTarget - transform.position).normalized;

        float bestScore = float.MinValue;
        Vector2 bestDir = desiredDir;
        bool foundFree = false;

        float angleStep = 360f / steeringRays;

        for (int i = 0; i < steeringRays; i++)
        {
            float angle = i * angleStep;
            Vector2 candidate = Rotate(desiredDir, angle);

            RaycastHit2D hit = Physics2D.CircleCast(
                transform.position, steeringProbeRadius,
                candidate, obstacleDetectDistance, obstacleLayer);

            if (hit.collider != null) continue;

            float score = Vector2.Dot(candidate, desiredDir);

            if (!foundFree || score > bestScore)
            {
                bestScore = score;
                bestDir = candidate;
                foundFree = true;
            }
        }

        if (!foundFree)
            bestDir = -desiredDir;

        return bestDir;
    }

    static Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y);
    }

    void MoveToward(Vector2 target, float speed)
    {
        Vector2 rawDir = GetSteeringDirection(target);

        smoothedDir = Vector2.Lerp(smoothedDir, rawDir, directionSmoothSpeed).normalized;

        if (Vector2.Dot(smoothedDir, lastMoveDir) < 0.95f || lastMoveDir == Vector2.zero)
        {
            lastMoveDir = smoothedDir;
        }

        Vector2 cardinal = DominantCardinal(smoothedDir);
        animator.SetBool(ParamIsMoving, true);
        animator.SetFloat(ParamMoveX, cardinal.x);
        animator.SetFloat(ParamMoveY, cardinal.y);

        transform.position += (Vector3)(smoothedDir * speed * Time.deltaTime);
    }

    void PickNewPatrolTarget()
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 candidate = homePosition + (Vector2)(Random.insideUnitCircle * patrolRadius);
            bool blocked = Physics2D.OverlapCircle(candidate, steeringProbeRadius, obstacleLayer);
            if (!blocked)
            {
                currentPatrolTarget = candidate;
                return;
            }
        }
        currentPatrolTarget = transform.position;
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Application.isPlaying ? (Vector3)homePosition : transform.position, returnHomeReachDistance);
    }
}