using UnityEngine;

public class NPCScript : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;
    public float reachDistance = 0.2f;
    public float waitTime = 2f;

    private int currentWaypoint = 0;
    private int direction = 1;
    private float waitTimer;
    private bool isWaiting = false;
    private bool isInteracting = false;

    private Animator animator;
    private Vector2 lastMoveDir = Vector2.down;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isInteracting) return;
        if (waypoints.Length == 0) return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;

            animator.SetBool("IsMoving", false);
            animator.SetFloat("MoveX", lastMoveDir.x);
            animator.SetFloat("MoveY", lastMoveDir.y);

            if (waitTimer <= 0)
            {
                isWaiting = false;
                ChangeWaypoint();
            }
            return;
        }

        Transform target = waypoints[currentWaypoint];

        Vector2 moveDir = ((Vector2)target.position - (Vector2)transform.position).normalized;

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        lastMoveDir = moveDir;
        animator.SetBool("IsMoving", true);
        animator.SetFloat("MoveX", moveDir.x);
        animator.SetFloat("MoveY", moveDir.y);

        if (Vector2.Distance(transform.position, target.position) < reachDistance)
        {
            isWaiting = true;
            waitTimer = waitTime;
        }
    }

    public void StartInteraction(Vector2 playerPosition)
    {
        isInteracting = true;

        Vector2 dirToPlayer = (playerPosition - (Vector2)transform.position);

        float absX = Mathf.Abs(dirToPlayer.x);
        float absY = Mathf.Abs(dirToPlayer.y);

        Vector2 facingDir;
        if (absX > absY)
            facingDir = dirToPlayer.x > 0 ? Vector2.right : Vector2.left;
        else
            facingDir = dirToPlayer.y > 0 ? Vector2.up : Vector2.down;

        lastMoveDir = facingDir;

        animator.SetBool("IsMoving", false);
        animator.SetFloat("MoveX", facingDir.x);
        animator.SetFloat("MoveY", facingDir.y);

        Debug.Log($"[NPC] FacingDir → X:{facingDir.x} Y:{facingDir.y}");
    }

    public void StopInteraction()
    {
        isInteracting = false;
    }

    void ChangeWaypoint()
    {
        currentWaypoint += direction;

        if (currentWaypoint >= waypoints.Length)
        {
            direction = -1;
            currentWaypoint = waypoints.Length - 2;
        }
        else if (currentWaypoint < 0)
        {
            direction = 1;
            currentWaypoint = 1;
        }
    }
}