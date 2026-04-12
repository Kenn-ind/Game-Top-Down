using UnityEngine;

/// <summary>
/// Taruh script ini di Prefab HealthPotion.
/// Lalu assign prefab tersebut ke field itemPrefab di ItemData HealthPotion.
/// </summary>
public class HealthPotionItem : MonoBehaviour, IUsable
{
    [Header("Pengaturan")]
    public int healAmount = 3;
    public GameObject useEffectPrefab;

    public bool CanUse(GameObject user)
    {
        PlayerHealth health = user.GetComponent<PlayerHealth>();
        if (health == null) return false;
        return !health.IsFullHealth; // tidak bisa dipakai jika HP sudah penuh
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
            Instantiate(useEffectPrefab, user.transform.position, Quaternion.identity);
    }
}