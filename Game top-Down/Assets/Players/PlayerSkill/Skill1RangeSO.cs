using UnityEngine;

[CreateAssetMenu(fileName = "Skill1RangeSO", menuName = "Skills/Skill1/Range")]
public class Skill1RangeSO : SkillActionSO
{
    public GameObject shurikenPrefab;
    public int shurikenAmount = 8;
    public float shurikenSpeed = 8f;
    public float shurikenLifetime = 2f;

    private GameObject _player;
    private AudioManage _audio;

    public override void Initialize(GameObject player)
    {
        _player = player;
        _audio = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManage>();
    }

    public override void Execute()
    {
        _audio.PlaySFX(_audio.S1Shu);
        float angleStep = 360f / shurikenAmount;
        for (int i = 0; i < shurikenAmount; i++)
        {
            float angle = i * angleStep;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            GameObject shuriken = Object.Instantiate(shurikenPrefab, _player.transform.position, Quaternion.identity);
            Rigidbody2D rb = shuriken.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = dir * shurikenSpeed;
            Object.Destroy(shuriken, shurikenLifetime);
        }
    }
}