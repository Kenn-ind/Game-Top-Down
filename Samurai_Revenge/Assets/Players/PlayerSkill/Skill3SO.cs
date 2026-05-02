using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill3SO", menuName = "Skills/Skill3")]
public class Skill3SO : SkillSO
{
    [Header("Ring")]
    public GameObject shurikenPrefab;
    public int shurikenAmount = 6;
    public float orbitRadius = 1.5f;
    public float rotateSpeed = 200f;
    public float ringDuration = 5f;

    [Header("Berserker")]
    public float holdThreshold = 0.4f;
    public float berserkerDuration = 5f;
    public float damageMultiplier = 2f;
    public float attackSpeedMultiplier = 2f;
    public float lifestealPercent = 0.2f;

    private GameObject _player;
    private PlayerStamina _stamina;
    private PlayerSkillState _skillState;
    private MonoBehaviour _runner;

    private float _nextSkillTime;
    private float _holdTimer;
    private bool _isHolding;
    private bool _berserkerActive;
    private List<GameObject> _shurikens = new List<GameObject>();

    public bool IsBerserkerActive => _berserkerActive;
    public float DamageMultiplier => _berserkerActive ? damageMultiplier : 1f;
    public float AttackSpeedMultiplier => _berserkerActive ? attackSpeedMultiplier : 1f;

    public override void Initialize(GameObject player)
    {
        _player = player;
        _stamina = player.GetComponent<PlayerStamina>();
        _skillState = player.GetComponent<PlayerSkillState>();
        _runner = player.GetComponent<SkillController>();
        _nextSkillTime = 0f;
        _berserkerActive = false;
        _holdTimer = 0f;
        _isHolding = false;
        _shurikens = new List<GameObject>();
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(keyBind)) { _holdTimer = 0f; _isHolding = true; }
        if (_isHolding && Input.GetKey(keyBind)) _holdTimer += Time.deltaTime;

        if (Input.GetKeyUp(keyBind) && _isHolding)
        {
            _isHolding = false;
            if (Time.time < _nextSkillTime) return;
            if (!_stamina.HasEnough(staminaCost)) { Debug.Log("Stamina tidak cukup!"); return; }
            if (_skillState.isUsingSkill) return;

            _stamina.UseStamina(staminaCost);

            if (_holdTimer < holdThreshold)
                _runner.StartCoroutine(ActivateRing());
            else
                TriggerBerserker();

            _nextSkillTime = Time.time + cooldown;
        }
    }

    IEnumerator ActivateRing()
    {
        _skillState.isUsingSkill = true;

        for (int i = 0; i < shurikenAmount; i++)
        {
            float angle = i * Mathf.PI * 2 / shurikenAmount;
            Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * orbitRadius;
            GameObject s = Object.Instantiate(shurikenPrefab, _player.transform.position + (Vector3)pos, Quaternion.identity);
            _shurikens.Add(s);
        }

        float timer = 0;
        while (timer < ringDuration)
        {
            timer += Time.deltaTime;
            for (int i = 0; i < _shurikens.Count; i++)
            {
                if (_shurikens[i] == null) continue;
                float angle = (Time.time * rotateSpeed + i * 360f / shurikenAmount) * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * orbitRadius;
                _shurikens[i].transform.position = _player.transform.position + (Vector3)offset;
            }
            yield return null;
        }

        foreach (GameObject s in _shurikens) if (s != null) Object.Destroy(s);
        _shurikens.Clear();

        _skillState.isUsingSkill = false;
    }

    void TriggerBerserker()
    {
        if (_berserkerActive) return;
        _runner.StartCoroutine(BerserkerMode());
    }

    IEnumerator BerserkerMode()
    {
        _berserkerActive = true;
        _player.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(berserkerDuration);
        _berserkerActive = false;
        _player.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void OnDamageDealt(int damageDealt, PlayerHealth playerHealth)
    {
        if (!_berserkerActive) return;
        int heal = Mathf.RoundToInt(damageDealt * lifestealPercent);
        if (heal > 0) playerHealth.Heal(heal);
    }

    public void MobileTriggerTap()
    {
        if (Time.time < _nextSkillTime) return;
        if (!_stamina.HasEnough(staminaCost)) { Debug.Log("Stamina tidak cukup!"); return; }
        if (_skillState.isUsingSkill) return;

        _stamina.UseStamina(staminaCost);
        _runner.StartCoroutine(ActivateRing());
        _nextSkillTime = Time.time + cooldown;
    }

    public void MobileHoldStart()
    {
        _holdTimer = 0f;
        _isHolding = true;
    }

    public void MobileHoldEnd()
    {
        if (!_isHolding) return;
        _isHolding = false;

        if (Time.time < _nextSkillTime) return;
        if (!_stamina.HasEnough(staminaCost)) { Debug.Log("Stamina tidak cukup!"); return; }
        if (_skillState.isUsingSkill) return;

        _stamina.UseStamina(staminaCost);

        if (_holdTimer < holdThreshold)
            _runner.StartCoroutine(ActivateRing());
        else
            TriggerBerserker();

        _nextSkillTime = Time.time + cooldown;
    }
}