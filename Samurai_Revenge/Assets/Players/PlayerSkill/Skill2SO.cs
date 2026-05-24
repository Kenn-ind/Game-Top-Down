using UnityEngine;

[CreateAssetMenu(fileName = "Skill2SO", menuName = "Skills/Skill2/Skill2SO")]
public class Skill2SO : SkillSO
{
    public Skill2MeleeSO melee;
    public Skill2RangeSO range;

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
        // Blok jika tutorial aktif dan belum sampai step skill 2
        if (TutorialManager.Instance != null && TutorialManager.Instance.IsTutorialActive)
        {
            if (TutorialManager.Instance.CurrentRequiredAction < TutorialActionType.Skill2Range)
                return;
        }

        if (Input.GetKeyDown(keyBind) && Time.time >= _nextSkillTime)
        {
            if (!_stamina.HasEnough(staminaCost)) { Debug.Log("Stamina tidak cukup!"); return; }
            if (_skillState.isUsingSkill) return;

            GameObject target = FindNearestEnemy();
            if (target == null) return;

            _stamina.UseStamina(staminaCost);
            float distance = Vector2.Distance(_player.transform.position, target.transform.position);

            if (distance <= closeRange)
            {
                melee?.Execute();
                TutorialManager.Instance?.ReportAction(TutorialActionType.Skill2Melee);
            }
            else
            {
                range?.Execute();
                TutorialManager.Instance?.ReportAction(TutorialActionType.Skill2Range);
            }

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
        // Blok jika tutorial aktif dan belum sampai step skill 2
        if (TutorialManager.Instance != null && TutorialManager.Instance.IsTutorialActive)
        {
            if (TutorialManager.Instance.CurrentRequiredAction < TutorialActionType.Skill2Range)
                return;
        }

        if (Time.time < _nextSkillTime) return;
        if (!_stamina.HasEnough(staminaCost)) { Debug.Log("Stamina tidak cukup!"); return; }
        if (_skillState.isUsingSkill) return;

        GameObject target = FindNearestEnemy();
        if (target == null) return;

        _stamina.UseStamina(staminaCost);
        float distance = Vector2.Distance(_player.transform.position, target.transform.position);

        if (distance <= closeRange)
        {
            melee?.Execute();
            TutorialManager.Instance?.ReportAction(TutorialActionType.Skill2Melee);
        }
        else
        {
            range?.Execute();
            TutorialManager.Instance?.ReportAction(TutorialActionType.Skill2Range);
        }

        _nextSkillTime = Time.time + cooldown;
    }
}