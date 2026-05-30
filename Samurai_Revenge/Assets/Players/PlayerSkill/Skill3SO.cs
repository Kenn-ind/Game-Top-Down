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
    public float holdThreshold = 0.2f;
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

    private float _mobileHoldTimer = 0f;
    private bool _mobileIsHolding = false;

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
        _mobileHoldTimer = 0f;
        _mobileIsHolding = false;
        _shurikens = new List<GameObject>();
    }

    // ─── Tutorial Block Check ─────────────────────────────────────────────────

    bool IsTutorialBlocked()
    {
        if (TutorialManager.Instance == null) return false;
        if (!TutorialManager.Instance.IsTutorialActive) return false;
        return TutorialManager.Instance.CurrentRequiredAction < TutorialActionType.Skill3Melee; // ← ubah dari Skill3Range
    }

    // ─── Keyboard Input ───────────────────────────────────────────────────────

    public override void OnUpdate()
    {
        if (IsTutorialBlocked()) return;

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

    // ─── Mobile Input ─────────────────────────────────────────────────────────

    public void MobileHoldStart()
    {
        if (IsTutorialBlocked()) return;
        _mobileHoldTimer = 0f;
        _mobileIsHolding = true;
    }

    public void MobileHoldUpdate()
    {
        if (_mobileIsHolding)
            _mobileHoldTimer += Time.deltaTime;
    }

    public void MobileHoldEnd()
    {
        if (!_mobileIsHolding) return;
        _mobileIsHolding = false;

        Debug.Log($"[Skill3] MobileHoldEnd, holdTimer={_mobileHoldTimer}, threshold={holdThreshold}");

        if (IsTutorialBlocked()) return;
        if (Time.time < _nextSkillTime) return;
        if (!_stamina.HasEnough(staminaCost)) return;
        if (_skillState.isUsingSkill) return;

        _stamina.UseStamina(staminaCost);

        if (_mobileHoldTimer < holdThreshold)
            _runner.StartCoroutine(ActivateRing());
        else
            TriggerBerserker();

        _nextSkillTime = Time.time + cooldown;
    }

    public void MobileTriggerTap()
    {
        if (IsTutorialBlocked()) return;
        if (Time.time < _nextSkillTime) return;
        if (!_stamina.HasEnough(staminaCost)) { Debug.Log("Stamina tidak cukup!"); return; }
        if (_skillState.isUsingSkill) return;

        _stamina.UseStamina(staminaCost);
        _runner.StartCoroutine(ActivateRing());
        _nextSkillTime = Time.time + cooldown;
    }

    // ─── Skill Logic ──────────────────────────────────────────────────────────

    IEnumerator ActivateRing()
    {
        _skillState.isUsingSkill = true;

        for (int i = 0; i < shurikenAmount; i++)
        {
            float angle = i * Mathf.PI * 2 / shurikenAmount;
            Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * orbitRadius;
            GameObject s = Object.Instantiate(
                shurikenPrefab,
                _player.transform.position + (Vector3)pos,
                Quaternion.identity);
            _shurikens.Add(s);
        }

        // Debug sebelum report
        Debug.Log($"[Skill3] ActivateRing, TutorialManager={TutorialManager.Instance != null}, IsTutorialActive={TutorialManager.Instance?.IsTutorialActive}, CurrentAction={TutorialManager.Instance?.CurrentRequiredAction}");

        TutorialManager.Instance?.ReportAction(TutorialActionType.Skill3Range);

        float timer = 0;
        while (timer < ringDuration)
        {
            timer += Time.deltaTime;
            for (int i = 0; i < _shurikens.Count; i++)
            {
                if (_shurikens[i] == null) continue;
                float angle = (Time.time * rotateSpeed + i * 360f / shurikenAmount)
                              * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle))
                                 * orbitRadius;
                _shurikens[i].transform.position =
                    _player.transform.position + (Vector3)offset;
            }
            yield return null;
        }

        foreach (GameObject s in _shurikens)
            if (s != null) Object.Destroy(s);
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

        Debug.Log($"[Skill3] BerserkerMode aktif, TutorialManager={TutorialManager.Instance != null}, IsTutorialActive={TutorialManager.Instance?.IsTutorialActive}, CurrentAction={TutorialManager.Instance?.CurrentRequiredAction}");

        TutorialManager.Instance?.ReportAction(TutorialActionType.Skill3Melee);

        yield return new WaitForSeconds(berserkerDuration);
        _berserkerActive = false;
        _player.GetComponent<SpriteRenderer>().color = Color.white;
    }

    // ─── Reset ────────────────────────────────────────────────────────────────

    public void ResetSkillState()
    {
        _runner.StopAllCoroutines();

        _berserkerActive = false;
        if (_player != null)
            _player.GetComponent<SpriteRenderer>().color = Color.white;

        foreach (GameObject s in _shurikens)
            if (s != null) Object.Destroy(s);
        _shurikens.Clear();

        if (_skillState != null)
            _skillState.isUsingSkill = false;

        _isHolding = false;
        _mobileIsHolding = false;
        _holdTimer = 0f;
        _mobileHoldTimer = 0f;
    }

    public void OnDamageDealt(int damageDealt, PlayerHealth playerHealth)
    {
        if (!_berserkerActive) return;
        int heal = Mathf.RoundToInt(damageDealt * lifestealPercent);
        if (heal > 0) playerHealth.Heal(heal);
    }
}