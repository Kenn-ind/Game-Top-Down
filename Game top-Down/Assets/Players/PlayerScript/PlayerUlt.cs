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
    private Rigidbody2D _rb;
    private PlayerHealth _health;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _health = GetComponent<PlayerHealth>();
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

        Debug.Log("Charging ult...");

        while (_chargeTimer < chargeTime)
        {
            _chargeTimer += Time.deltaTime;
            yield return null;
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

        int refundKills = Mathf.RoundToInt(killsRequired * refundPercent);
        currentKills = Mathf.Min(refundKills, killsRequired);
        _isReady = currentKills >= killsRequired;

        UpdateBar();
        Debug.Log($"Charge dibatalkan! Kill dikembalikan: {currentKills}");
    }

    IEnumerator ExecuteUlt()
    {
        _isCasting = true;
        _isReady = false;

        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        Debug.Log("JUDGEMENT CUT!");

        for (int i = 0; i < slashCount; i++)
        {
            PerformSlash(i + 1);
            yield return new WaitForSeconds(slashDelay);
        }

        currentKills = 0;
        UpdateBar();
        _isCasting = false;

        Debug.Log("Ult selesai.");
    }

    void PerformSlash(int slashNumber)
    {
        Debug.Log($"Tebasan {slashNumber}!");

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, aoeRadius);
        foreach (Collider2D hit in hits)
        {
            BaseEnemy enemy = hit.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(slashDamage, Vector2.zero, false);
            }
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
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}