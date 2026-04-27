using UnityEngine;

/// <summary>
/// Taruh script ini di Prefab StaminaPotion.
/// Lalu assign prefab tersebut ke field itemPrefab di ItemData StaminaPotion.
/// </summary>
public class StaminaPotionItem : MonoBehaviour, IUsable
{
    [Header("Pengaturan")]
    public int restoreAmount = 3;
    public GameObject useEffectPrefab;

    public bool CanUse(GameObject user)
    {
        PlayerStamina stamina = user.GetComponent<PlayerStamina>();
        if (stamina == null) return false;
        return !stamina.IsFullStamina; // tidak bisa dipakai jika stamina sudah penuh
    }

    public void Use(GameObject user)
    {
        PlayerStamina stamina = user.GetComponent<PlayerStamina>();
        if (stamina == null)
        {
            Debug.LogWarning("[StaminaPotion] PlayerStamina tidak ditemukan!");
            return;
        }
        stamina.RestoreStamina(restoreAmount);
        Debug.Log($"[StaminaPotion] Memulihkan {restoreAmount} Stamina.");

        GameObject effect = Instantiate(

            useEffectPrefab,
            user.transform.position,
            Quaternion.identity

        );

        effect.transform.SetParent(user.transform);
        Destroy(effect, 2f);

    }
}