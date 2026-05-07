using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScene : MonoBehaviour
{
    [Header("=== Scene Names ===")]
    [Tooltip("Nama scene untuk lanjut game (Load Save)")]
    public string continueSceneName = "GameScene";

    [Tooltip("Nama scene untuk new game")]
    public string newGameSceneName = "GameScene";

    [Header("=== Panel Settings ===")]
    [Tooltip("Drag Panel Settings kamu ke sini")]
    public GameObject settingsPanel;

    // ──────────────────────────────────────────────────────────
    // PLAY — Lanjutkan game (bisa dimodifikasi untuk load save)
    // ──────────────────────────────────────────────────────────
    public void OnClickPlay()
    {
        Debug.Log("[Menu] PLAY diklik");

        if (MenuExitTrigger.Instance != null)
            MenuExitTrigger.Instance.TriggerExit(continueSceneName);
        else
            SceneManager.LoadScene(continueSceneName);
    }

    // ──────────────────────────────────────────────────────────
    // NEW GAME — Reset progress lalu mulai dari awal
    // ──────────────────────────────────────────────────────────
    public void OnClickNewGame()
    {
        Debug.Log("[Menu] NEW GAME diklik");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        if (MenuExitTrigger.Instance != null)
            MenuExitTrigger.Instance.TriggerExit(newGameSceneName);
        else
            SceneManager.LoadScene(newGameSceneName); // Fallback
    }

    // ──────────────────────────────────────────────────────────
    // SETTINGS — Toggle panel Settings (SetActive true/false)
    // ──────────────────────────────────────────────────────────
    public void OnClickSettings()
    {
        if (settingsPanel == null)
        {
            Debug.LogWarning("[Menu] settingsPanel belum di-assign di Inspector!");
            return;
        }

        bool isActive = settingsPanel.activeSelf;
        settingsPanel.SetActive(!isActive);

        Debug.Log("[Menu] SETTINGS panel → " + (!isActive ? "DIBUKA" : "DITUTUP"));
    }

    // ──────────────────────────────────────────────────────────
    // EXIT — Keluar dari game
    // ──────────────────────────────────────────────────────────
    public void OnClickExit()
    {
        Debug.Log("[Menu] EXIT diklik → menutup game.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}