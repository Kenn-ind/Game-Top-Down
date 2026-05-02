using UnityEngine;

[CreateAssetMenu(fileName = "Skill1SO", menuName = "Skills/Skill1/Skill1SO")]
public class Skill1SO : SkillSO
{
    public Skill1MeleeSO melee;
    public Skill1RangeSO range;

    private GameObject _player;
    private PlayerStamina _stamina;
    private PlayerSkillState _skillState;
    private float _nextSkillTime;

    public override void Initialize(GameObject player)
    {
        _player = player;
        _stamina = player.GetComponent<PlayerStamina>();
        _skillState = player.GetComponent<PlayerSkillState>();

        _nextSkillTime = 0f;

        melee?.Initialize(player);
        range?.Initialize(player);
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(keyBind) && Time.time >= _nextSkillTime)
        {
            if (!_stamina.HasEnough(staminaCost)) { Debug.Log("Stamina tidak cukup!"); return; }
            if (_skillState.isUsingSkill) return;

            GameObject target = FindNearestEnemy();
            if (target == null) return;

            _stamina.UseStamina(staminaCost);

            float distance = Vector2.Distance(_player.transform.position, target.transform.position);
            if (distance <= closeRange)
                melee?.Execute();
            else
                range?.Execute();

            _nextSkillTime = Time.time + cooldown;
        }
    }

    GameObject FindNearestEnemy()
    {
        BaseEnemy[] enemies = Object.FindObjectsOfType<BaseEnemy>();
        float shortest = Mathf.Infinity;
        GameObject nearest = null;
        foreach (BaseEnemy enemy in enemies)
        {
            float dist = Vector2.Distance(_player.transform.position, enemy.transform.position);
            if (dist < shortest && dist <= detectRadius) { shortest = dist; nearest = enemy.gameObject; }
        }
        return nearest;
    }

    public void MobileTrigger()
    {
        if (Time.time < _nextSkillTime) return;
        if (!_stamina.HasEnough(staminaCost)) { Debug.Log("Stamina tidak cukup!"); return; }
        if (_skillState.isUsingSkill) return;
 
        GameObject target = FindNearestEnemy();
        if (target == null) return;
 
        _stamina.UseStamina(staminaCost);
        float distance = Vector2.Distance(_player.transform.position, target.transform.position);
        if (distance <= closeRange)
            melee?.Execute();
        else
            range?.Execute();
 
        _nextSkillTime = Time.time + cooldown;
    }
}