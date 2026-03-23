using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    public int maxStamina = 10;
    public float regenPerSecond = 2f;
    public float regenDelay = 1f;

    private float currentStamina;
    private float regenTimer = 0f;

    public Sprite[] staminaFrames;
    public Image staminaImage;
    private int _currentFrame = 0;
    private Coroutine _staminaAnim;
    private PlayerStats _stats;

    void Awake()
    {
        _stats = GetComponent<PlayerStats>();

        if (_stats == null)
            Debug.LogError("PlayerStats tidak ditemukan di Player!");
    }

    void Start()
    {
        if (_stats != null)
            maxStamina = _stats.maxStamina;

        currentStamina = maxStamina;
        _currentFrame = 0;

        if (staminaImage != null && staminaFrames.Length > 0)
            staminaImage.sprite = staminaFrames[0];
    }

    void Update()
    {
        if (_stats != null)
            maxStamina = _stats.maxStamina;

        if (regenTimer > 0f)
        {
            regenTimer -= Time.deltaTime;
            return;
        }

        if (currentStamina < maxStamina)
        {
            currentStamina = Mathf.Min(
                currentStamina + regenPerSecond * Time.deltaTime,
                maxStamina
            );

            UpdateBar();
        }
    }

    public bool UseStamina(int amount)
    {
        if (currentStamina < amount) return false;

        currentStamina -= amount;
        currentStamina = Mathf.Max(currentStamina, 0f);

        regenTimer = regenDelay;
        UpdateBar();

        return true;
    }

    public bool HasEnough(int amount) => currentStamina >= amount;

    void UpdateBar()
    {
        if (staminaFrames == null || staminaFrames.Length == 0) return;
        if (maxStamina <= 0) return;

        float ratio = 1f - (currentStamina / maxStamina);
        int target = Mathf.RoundToInt(ratio * (staminaFrames.Length - 1));

        if (_staminaAnim != null)
            StopCoroutine(_staminaAnim);

        _staminaAnim = StartCoroutine(AnimateBar(target));
    }

    IEnumerator AnimateBar(int target)
    {
        if (staminaImage == null) yield break;

        int dir = target > _currentFrame ? 1 : -1;

        while (_currentFrame != target)
        {
            _currentFrame += dir;

            if (_currentFrame >= 0 && _currentFrame < staminaFrames.Length)
                staminaImage.sprite = staminaFrames[_currentFrame];

            yield return new WaitForSeconds(1f / 12f);
        }
    }
}