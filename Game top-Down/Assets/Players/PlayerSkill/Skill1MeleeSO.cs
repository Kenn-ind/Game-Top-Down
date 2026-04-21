using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill1MeleeSO", menuName = "Skills/Skill1/Melee")]
public class Skill1MeleeSO : SkillActionSO
{
    public float dashDistance = 2f;
    public float dashSpeed = 15f;
    public float dashHitRadius = 0.5f;
    public int dashCount = 1;

    private GameObject _player;
    private PlayerStamina _stamina;
    private PlayerStats _stats;
    private PlayerSkillState _skillState;
    private AudioManage _audio;
    private MonoBehaviour _runner;
    public SkillUpgradeData upgradeData;


    public override void Initialize(GameObject player)
    {
        _player = player;
        _stamina = player.GetComponent<PlayerStamina>();
        _stats = player.GetComponent<PlayerStats>();
        _skillState = player.GetComponent<PlayerSkillState>();
        _runner = player.GetComponent<SkillController>();
        _audio = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManage>();
    }

    public override void Execute()
    {
        GameObject target = FindNearestEnemy();
        if (target != null)
            _runner.StartCoroutine(DashAttack(target));
    }

    IEnumerator DashAttack(GameObject target)
    {
        _skillState.isUsingSkill = true;

        Rigidbody2D rb = _player.GetComponent<Rigidbody2D>();
        Collider2D col = _player.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        if (rb != null) rb.isKinematic = true;

        for (int i = 0; i < dashCount; i++)
        {
            if (target == null) { target = FindNearestEnemy(); if (target == null) break; }

            HashSet<BaseEnemy> hitEnemies = new HashSet<BaseEnemy>();
            Vector2 direction = (target.transform.position - _player.transform.position).normalized;
            Vector2 dashTarget = (Vector2)target.transform.position + direction * dashDistance;

            while (Vector2.Distance(_player.transform.position, dashTarget) > 0.05f)
            {
                _player.transform.position = Vector2.MoveTowards(
                    _player.transform.position, dashTarget, dashSpeed * Time.deltaTime);

                foreach (Collider2D hit in Physics2D.OverlapCircleAll(_player.transform.position, dashHitRadius))
                {
                    BaseEnemy enemy = hit.GetComponent<BaseEnemy>();
                    if (enemy != null && !hitEnemies.Contains(enemy))
                    {
                        _audio.PlaySFX(_audio.S1Sword);
                        enemy.TakeDamage(_stats.attackDamage + (upgradeData != null ? upgradeData.meleeDamageBonus : 0), Vector2.zero, false);
                        hitEnemies.Add(enemy);
                    }
                }
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
            if (target == null) target = FindNearestEnemy();
        }

        if (col != null) col.enabled = true;
        if (rb != null) rb.isKinematic = false;
        _skillState.isUsingSkill = false;
    }

    GameObject FindNearestEnemy()
    {
        BaseEnemy[] enemies = Object.FindObjectsOfType<BaseEnemy>();
        float shortest = Mathf.Infinity;
        GameObject nearest = null;
        foreach (BaseEnemy enemy in enemies)
        {
            float dist = Vector2.Distance(_player.transform.position, enemy.transform.position);
            if (dist < shortest) { shortest = dist; nearest = enemy.gameObject; }
        }
        return nearest;
    }
}