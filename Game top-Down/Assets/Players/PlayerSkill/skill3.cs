using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skill3 : MonoBehaviour
{
    public GameObject shurikenPrefab;
    public KeyCode KeyBindSkill3;

    public int shurikenCount = 6;
    public float orbitRadius = 1.5f;
    public float rotateSpeed = 200f;
    public float ringDuration = 5f;

    public float holdThreshold = 0.4f; 
    public float berserkerDuration = 5f;
    public float damageMultiplier = 2f;
    public float attackSpeedMultiplier = 2f;
    public float lifestealPercent = 0.2f;

    public float cooldown = 8f;
    public int staminaCost = 5;

    private float nextSkillTime;
    private float holdTimer = 0f;
    private bool isHolding = false;
    private bool berserkerActive = false;
    private List<GameObject> shurikens = new List<GameObject>();
    private PlayerStamina _stamina;


    public bool IsBerserkerActive => berserkerActive;
    public float DamageMultiplier => berserkerActive ? damageMultiplier : 1f;
    public float AttackSpeedMultiplier => berserkerActive ? attackSpeedMultiplier : 1f;

    void Start()
    {
        _stamina = GetComponent<PlayerStamina>();

    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyBindSkill3))
        {
            holdTimer = 0f;
            isHolding = true;
        }

        if (isHolding && Input.GetKey(KeyBindSkill3))
        {
            holdTimer += Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyBindSkill3) && isHolding)
        {
            isHolding = false;

            if (Time.time < nextSkillTime) return;
            if (!_stamina.HasEnough(staminaCost))
            {
                Debug.Log("Stamina tidak cukup!");
                return;
            }

            if (holdTimer < holdThreshold)
                TriggerRing();
            else
                TriggerBerserker();

            nextSkillTime = Time.time + cooldown;
        }
    }


    void TriggerRing()
    {
        _stamina.UseStamina(staminaCost);
        StartCoroutine(ActivateRing());
    }

    IEnumerator ActivateRing()
    {
        for (int i = 0; i < shurikenCount; i++)
        {
            float angle = i * Mathf.PI * 2 / shurikenCount;
            Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * orbitRadius;
            GameObject s = Instantiate(shurikenPrefab, transform.position + (Vector3)pos, Quaternion.identity);
            shurikens.Add(s);
        }

        float timer = 0;
        while (timer < ringDuration)
        {
            timer += Time.deltaTime;
            for (int i = 0; i < shurikens.Count; i++)
            {
                if (shurikens[i] == null) continue;
                float angle = (Time.time * rotateSpeed + i * 360f / shurikenCount) * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * orbitRadius;
                shurikens[i].transform.position = transform.position + (Vector3)offset;
            }
            yield return null;
        }

        foreach (GameObject s in shurikens)
            if (s != null) Destroy(s);
        shurikens.Clear();
    }


    void TriggerBerserker()
    {
        if (berserkerActive) return;
        _stamina.UseStamina(staminaCost);
        StartCoroutine(BerserkerMode());
    }

    IEnumerator BerserkerMode()
    {
        berserkerActive = true;
        Debug.Log("Berserker aktif!");

         GetComponent<SpriteRenderer>().color = Color.red;

        yield return new WaitForSeconds(berserkerDuration);

        berserkerActive = false;
        Debug.Log("Berserker selesai.");

         GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void OnDamageDealt(int damageDealt, PlayerHealth playerHealth)
    {
        if (!berserkerActive) return;
        int heal = Mathf.RoundToInt(damageDealt * lifestealPercent);
        if (heal > 0) playerHealth.Heal(heal);
    }
}