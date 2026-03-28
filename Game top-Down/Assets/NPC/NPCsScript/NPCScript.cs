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

    private Animator animator;
    private Vector2 lastMoveDir = Vector2.down;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;

            // Animator: berhenti gerak
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