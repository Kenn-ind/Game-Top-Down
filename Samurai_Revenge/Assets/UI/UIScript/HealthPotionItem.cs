using UnityEngine;

public class HealthPotionItem : MonoBehaviour, IUsable
{   
    public int healAmount = 3;
    public GameObject useEffectPrefab;

    public bool CanUse(GameObject user)
    {
        PlayerHealth health = user.GetComponent<PlayerHealth>();
        if (health == null) return false;
        return !health.IsFullHealth;
    }

    public void Use(GameObject user)
    {
        PlayerHealth health = user.GetComponent<PlayerHealth>();
        if (health == null)
        {
            Debug.LogWarning("[HealthPotion] PlayerHealth tidak ditemukan!");
            return;
        }

        health.Heal(healAmount);
        Debug.Log($"[HealthPotion] Memulihkan {healAmount} HP.");

        if (useEffectPrefab != null)
        {
            GameObject effect = Instantiate(
                useEffectPrefab,
                user.transform.position,
                Quaternion.identity
            );

            effect.transform.SetParent(user.transform);

            Destroy(effect, 2f);
        }
    }
}