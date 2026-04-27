using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill2MeleeSO", menuName = "Skills/Skill2/Melee")]
public class Skill2MeleeSO : SkillActionSO
{
    public float slashDuration = 0.5f;
    public float spinRadius = 1.5f;
    public SkillUpgradeData upgradeData; // ← assign di Inspector

    private GameObject _player;
    private PlayerStats _stats;
    private PlayerSkillState _skillState;
    private AudioManage _audio;
    private Animator _animator;
    private MonoBehaviour _runner;

    private static readonly int SpinSlashHash = Animator.StringToHash("SpinSlash");

    public override void Initialize(GameObject player)
    {
        _player = player;
        _stats = player.GetComponent<PlayerStats>();
        _skillState = player.GetComponent<PlayerSkillState>();
        _animator = player.GetComponent<Animator>();
        _runner = player.GetComponent<SkillController>();
        _audio = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManage>();
    }

    public override void Execute()
    {
        _runner.StartCoroutine(SlashAttack());
    }

    IEnumerator SlashAttack()
    {
        _skillState.isUsingSkill = true;
        _animator.SetTrigger(SpinSlashHash);
        _audio.PlaySFX(_audio.S2Sword);

        HashSet<BaseEnemy> hitEnemies = new HashSet<BaseEnemy>();
        float timer = 0f;

        while (timer < slashDuration)
        {
            timer += Time.deltaTime;
            foreach (Collider2D hit in Physics2D.OverlapCircleAll(_player.transform.position, spinRadius))
            {
                BaseEnemy enemy = hit.GetComponent<BaseEnemy>();
                if (enemy != null && !hitEnemies.Contains(enemy))
                {
                    int damage = _stats.attackDamage + (upgradeData != null ? upgradeData.skill2MeleeDamageBonus : 0);
                    enemy.TakeDamage(damage, Vector2.zero, false);
                    hitEnemies.Add(enemy);
                }
            }
            yield return null;
        }

        _animator.ResetTrigger(SpinSlashHash);
        _skillState.isUsingSkill = false;
    }
}