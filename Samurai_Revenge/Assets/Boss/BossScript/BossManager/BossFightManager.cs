using UnityEngine;

public class BossFightManager : MonoBehaviour
{
    public static BossFightManager Instance { get; private set; }

    [Header("References")]
    public GameObject bossObject;

    private BossAI bossAI;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        bossAI = bossObject?.GetComponent<BossAI>();
    }

    // Panggil ini di akhir dialog sistem kamu
    public void StartBossFight()
    {
        if (bossAI == null) return;
        bossAI.enabled = true;
        Debug.Log("[BossFight] Fight dimulai!");
    }

    public void OnBossDefeated()
    {
        Debug.Log("[BossFight] Boss dikalahkan!");
        // Tambahkan reward, cutscene, dll di sini
    }
}