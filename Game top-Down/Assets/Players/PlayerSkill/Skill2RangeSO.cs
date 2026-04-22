using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill2RangeSO", menuName = "Skills/Skill2/Range")]
public class Skill2RangeSO : SkillActionSO
{
    public GameObject shurikenPrefab;
    public int shurikenAmount = 5;
    public float shurikenSpeed = 10f;
    public float shurikenLifetime = 2f;
    public float fireDelay = 0.1f;
    public float detectRadius = 8f;
    public SkillUpgradeData upgradeData;

    private GameObject _player;
    private PlayerStats _stats;
    private PlayerSkillState _skillState;
    private AudioManage _audio;
    private MonoBehaviour _runner;

    public override void Initialize(GameObject player)
    {
        _player = player;
        _stats = player.GetComponent<PlayerStats>();
        _skillState = player.GetComponent<PlayerSkillState>();
        _runner = player.GetComponent<SkillController>();
        _audio = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManage>();
    }

    public override void Execute()
    {
        _runner.StartCoroutine(ShurikenBurst());
    }

    IEnumerator ShurikenBurst()
    {
        _skillState.isUsingSkill = true;

        for (int i = 0; i < shurikenAmount; i++)
        {
            GameObject target = FindNearestEnemy();
            if (target == null) break;

            _audio.PlaySFX(_audio.S2Shu);

            Vector2 direction = (target.transform.position - _player.transform.position).normalized;

            GameObject shuriken = Object.Instantiate(
                shurikenPrefab, _player.transform.position, Quaternion.identity);

            Shuriken shurikenScript = shuriken.GetComponent<Shuriken>();
            if (shurikenScript != null)
                shurikenScript.Init(_stats, upgradeData, isRangeSkill: true);

            Rigidbody2D rb = shuriken.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = direction * shurikenSpeed;

            Object.Destroy(shuriken, shurikenLifetime);

            yield return new WaitForSeconds(fireDelay);
        }

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
            if (dist < shortest && dist <= detectRadius)
            {
                shortest = dist;
                nearest = enemy.gameObject;
            }
        }
        return nearest;
    }
}