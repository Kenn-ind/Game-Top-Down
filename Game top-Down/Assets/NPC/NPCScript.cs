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

    void Update()
    {
        if (waypoints.Length == 0) return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0)
            {
                isWaiting = false;
                ChangeWaypoint();
            }

            return;
        }

        Transform target = waypoints[currentWaypoint];

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

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