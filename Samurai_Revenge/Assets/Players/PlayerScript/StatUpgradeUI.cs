using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatUpgradeUI : MonoBehaviour
{
    public PlayerStats stats;
    public PlayerLevel playerLevel;

    public TMP_Text titleText;
    public TMP_Text hpText;
    public TMP_Text staminaText;
    public TMP_Text armorText;
    public TMP_Text attackText;

    public Button hpButton;
    public Button staminaButton;
    public Button armorButton;
    public Button attackButton;
    public Button closeButton;

    public StatDisplayUI statDisplay;

    public void Init()
    {
        if (hpButton != null) hpButton.onClick.AddListener(OnUpgradeHP);
        if (staminaButton != null) staminaButton.onClick.AddListener(OnUpgradeStamina);
        if (armorButton != null) armorButton.onClick.AddListener(OnUpgradeArmor);
        if (attackButton != null) attackButton.onClick.AddListener(OnUpgradeAttack);
        if (closeButton != null) closeButton.onClick.AddListener(OnClose);
    }

    public void ShowPopup()
    {
        Debug.Log("POPUP MUNCUL");

        gameObject.SetActive(true);
        RefreshUI();
    }

    void OnUpgradeHP()
    {
        if (stats != null && stats.UpgradeHP())RefreshUI();
    }

    void OnUpgradeStamina()
    {
        if (stats != null && stats.UpgradeStamina()) RefreshUI();
    }

    void OnUpgradeArmor()
    {
        if (stats != null && stats.UpgradeArmor()) RefreshUI();
    }

    void OnUpgradeAttack()
    {
        if (stats != null && stats.UpgradeAttack()) RefreshUI();
    }

    void OnClose()
    {
        gameObject.SetActive(false);
    }
    void OnEnable()
    {
        Debug.Log("POPUP AKTIF");
    }

    void OnDisable()
    {
        Debug.Log("POPUP DIMATIKAN");
    }

    void RefreshUI()
    {
        if (stats == null)
        {
            Debug.LogError("StatUpgradeUI: stats belum di assign!");
            return;
        }

        int poin = stats.availablePoints;

        if (titleText != null)
            titleText.text = $"LEVEL UP!  Poin tersisa: {poin}";

        if (hpText != null)
            hpText.text = $"Max HP     : {stats.maxHP}";

        if (staminaText != null)
            staminaText.text = $"Stamina    : {stats.maxStamina}";

        if (armorText != null)
            armorText.text = $"Armor      : {stats.armor}%";

        if (attackText != null)
            attackText.text = $"Attack     : {stats.attackDamage}";

        bool hasPoin = poin > 0;

        if (hpButton != null) hpButton.interactable = hasPoin;
        if (staminaButton != null) staminaButton.interactable = hasPoin;
        if (armorButton != null) armorButton.interactable = hasPoin;
        if (attackButton != null) attackButton.interactable = hasPoin;

        if (closeButton != null)
            closeButton.interactable = !hasPoin;

        if (statDisplay != null)
        {
            statDisplay.RefreshDisplay();
        }
        else
        {
            Debug.LogWarning("StatUpgradeUI: statDisplay belum di assign!");
        }
    }
}