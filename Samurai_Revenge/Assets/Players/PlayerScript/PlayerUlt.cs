using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerUlt : MonoBehaviour
{
    public int killsRequired = 10;
    public int currentKills = 0;

    public Sprite[] ultFrames;
    public Image ultBarImage;
    private int _currentFrame = 0;

    public KeyCode ultKey = KeyCode.R;
    public float chargeTime = 3f;
    public float aoeRadius = 5f;
    public int slashDamage = 3;
    public int slashCount = 3;
    public float slashDelay = 0.2f;

    public float refundPercent = 1f;

    private bool _isReady = false;
    private bool _isCharging = false;
    private bool _isCasting = false;
    private float _chargeTimer = 0f;
    public float ultDashSpeed = 20f;
    public float ultDashDuration = 0.15f;
    public float fadeDuration = 0.2f;

    private Rigidbody2D _rb;
    private PlayerHealth _health;
    private movement _movement;
    private SpriteRenderer _spriteRenderer;
    private Animator _anim;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _health = GetComponent<PlayerHealth>();
        _movement = GetComponent<movement>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();

        UpdateBar();
    }

    void Update()
    {
        if (_isCasting) return;

        if (Input.GetKeyDown(ultKey) && _isReady && !_isCharging)
        {
            StartCoroutine(ChargeUlt());
        }
    }

    public void OnEnemyKilled()
    {
        if (_isReady) return;

        currentKills++;
        currentKills = Mathf.Min(currentKills, killsRequired);
        UpdateBar();

        if (currentKills >= killsRequired)
        {
            _isReady = true;
            Debug.Log("ULT SIAP!");
        }
    }

    IEnumerator ChargeUlt()
    {
        _isCharging = true;
        _chargeTimer = 0f;
        _rb.velocity = Vector2.zero;
        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        _movement?.SetMovementLocked(true);

        if (_anim != null)
            _anim.SetBool("IsChargingUlt", true);

        while (_chargeTimer < chargeTime)
        {
            _chargeTimer += Time.deltaTime;
            yield return null;
        }

        if (_anim != null)
        {
            _anim.SetBool("IsChargingUlt", false);
            _anim.SetTrigger("CastUlt");
        }

        _isCharging = false;
        StartCoroutine(ExecuteUlt());
    }

    public void CancelCharge()
    {
        if (!_isCharging) return;
        StopCoroutine(nameof(ChargeUlt));
        _isCharging = false;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _movement?.SetMovementLocked(false);

        if (_anim != null)
            _anim.SetBool("IsChargingUlt", false);

        if (_spriteRenderer != null)
        {
            Color c = _spriteRenderer.color;
            _spriteRenderer.color = new Color(c.r, c.g, c.b, 1f);
        }

        int refundKills = Mathf.RoundToInt(killsRequired * refundPercent);
        currentKills = Mathf.Min(refundKills, killsRequired);
        _isReady = currentKills >= killsRequired;
        UpdateBar();
    }

    IEnumerator ExecuteUlt()
    {
        _isCasting = true;
        _isReady = false;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;


        yield return StartCoroutine(UltDash());

        Vector2 castPosition = transform.position;
        _rb.constraints = RigidbodyConstraints2D.FreezeAll;

        Debug.Log("JUDGEMENT CUT!");

        yield return new WaitForSeconds(fadeDuration);

        for (int i = 0; i < slashCount; i++)
        {
            PerformSlash(castPosition, i + 1);
            yield return new WaitForSeconds(slashDelay);
        }

        yield return new WaitForSeconds(fadeDuration);

        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        currentKills = 0;
        UpdateBar();
        _isCasting = false;
        _movement?.SetMovementLocked(false);

        Debug.Log("Ult selesai.");
    }

    IEnumerator UltDash()
    {
        Vector2 dashDir = _movement != null ? GetDashDirection() : Vector2.up;

        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        float timer = 0f;
        while (timer < ultDashDuration)
        {
            _rb.velocity = dashDir * ultDashSpeed;
            timer += Time.deltaTime;
            yield return null;
        }

        _rb.velocity = Vector2.zero;
    }

    Vector2 GetDashDirection()
    {
        return _movement.LastDirection;
    }

    void PerformSlash(Vector2 position, int slashNumber)
    {
        Debug.Log($"Tebasan {slashNumber}!");
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, aoeRadius);
        foreach (Collider2D hit in hits)
        {
            BaseEnemy enemy = hit.GetComponent<BaseEnemy>();
            if (enemy != null)
                enemy.TakeDamage(slashDamage, Vector2.zero, false);
        }
    }

    void UpdateBar()
    {
        if (ultFrames.Length == 0 || ultBarImage == null) return;

        float ratio = (float)currentKills / killsRequired;
        int target = Mathf.RoundToInt(ratio * (ultFrames.Length - 1));
        target = Mathf.Clamp(target, 0, ultFrames.Length - 1);

        _currentFrame = target;
        ultBarImage.sprite = ultFrames[_currentFrame];

        Button ultButton = ultBarImage.GetComponent<Button>();
        if (ultButton != null)
            ultButton.interactable = _isReady;
    }

    public void MobileUlt()
    {
        if (_isReady && !_isCharging && !_isCasting)
            StartCoroutine(ChargeUlt());
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}