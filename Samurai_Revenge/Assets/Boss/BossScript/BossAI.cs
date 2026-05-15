using UnityEngine;

public class BossAI : MonoBehaviour
{
    public enum BossState { Idle, Chase, Attack, UseSkill }

    [Header("Settings")]
    public float skillCooldown = 8f;
    public float chaseRadius = 8f;

    private BossState currentState = BossState.Idle;
    private BossMovement movement;
    private BossAttack attack;
    private BossSkillController skillController;
    private Transform player;
    private float nextSkillTime = 0f;

    void Start()
    {
        movement = GetComponent<BossMovement>();
        attack = GetComponent<BossAttack>();
        skillController = GetComponent<BossSkillController>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Boss tidak aktif sampai dialog selesai
        enabled = false;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (Time.time >= nextSkillTime
            && !attack.IsAttacking
            && !skillController.IsBusy)
        {
            currentState = BossState.UseSkill;
        }
        else if (distance <= attack.meleeRadius || distance <= attack.rangeRadius)
        {
            currentState = BossState.Attack;
        }
        else if (distance <= chaseRadius)
        {
            currentState = BossState.Chase;
        }
        else
        {
            currentState = BossState.Idle;
        }

        HandleState();
    }

    void HandleState()
    {
        switch (currentState)
        {
            case BossState.Idle:
                movement.StopMoving();
                break;

            case BossState.Chase:
                movement.MoveTowardsPlayer();
                break;

            case BossState.Attack:
                movement.StopMoving();
                attack.TryAttack(player);
                break;

            case BossState.UseSkill:
                movement.StopMoving();
                skillController.UseRandomSkill();
                nextSkillTime = Time.time + skillCooldown;
                break;
        }
    }

    public void ResetState()
    {
        currentState = BossState.Idle;
        movement?.StopMoving();
        attack?.ResetState();
        skillController?.ResetState();
        nextSkillTime = 0f;
    }
}