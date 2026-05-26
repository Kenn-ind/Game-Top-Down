using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScene : MonoBehaviour
{
    [Header("=== Mode Scenes ===")]
    [Tooltip("Nama scene untuk Story Mode")]
    public string storyModeSceneName = "GameScene";
    [Tooltip("Nama scene untuk Normal Mode")]
    public string normalModeSceneName = "GameScene";

    [Header("=== Panels ===")]
    //public GameObject settingsPanel;
    public GameObject modeSelectorPanel;
    public GameObject mainMenuPanel;
    public GameObject rightBar;
    public GameObject comingSoonPopup;
    public GameObject settingsPanel;

    private bool _isNewGame = false;

    // ============================================================
    //  PLAY
    // ============================================================
    public void OnClickPlay()
    {
        _isNewGame = false;
        ShowModeSelector();
    }

    // ============================================================
    //  NEW GAME
    // ============================================================
    public void OnClickNewGame()
    {
        _isNewGame = true;
        ShowModeSelector();
    }

    // ============================================================
    //  MODE SELECTOR
    // ============================================================
    void ShowModeSelector()
    {
        if (modeSelectorPanel != null)
        {
            mainMenuPanel?.SetActive(false);
            modeSelectorPanel.SetActive(true);
            SetRightBarVisible(false);
        }
        else
        {
            Debug.LogWarning("[Menu] modeSelectorPanel belum di-assign!");
            StartGame(GameModeType.Normal);
        }
    }

    public void OnClickNormalMode()
    {
        StartGame(GameModeType.Normal);
    }

    public void OnClickCloseCredit()
    {
        settingsPanel.SetActive(false);
    }

    public void OnClickStoryMode()
    {
        if (comingSoonPopup != null)
        {
            comingSoonPopup.SetActive(true);
            modeSelectorPanel?.SetActive(false);
        }
        else
            StartGame(GameModeType.Story);
    }

    public void OnClickBackFromModeSelector()
    {
        modeSelectorPanel?.SetActive(false);
        mainMenuPanel?.SetActive(true);
        SetRightBarVisible(true);
    }

    public void OnClickCloseComingSoon()
    {
        if (comingSoonPopup != null)
            comingSoonPopup.SetActive(false);

        modeSelectorPanel?.SetActive(true);
    }

    // ============================================================
    //  START GAME
    // ============================================================
    void StartGame(GameModeType mode)
    {
        GameMode.Current = mode;
        PlayerPrefs.SetInt("GameMode", (int)mode);

        if (_isNewGame)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("GameMode", (int)mode);
        }

        PlayerPrefs.Save();

        modeSelectorPanel?.SetActive(false);
        mainMenuPanel?.SetActive(false);
        SetRightBarVisible(false);

        string sceneName = mode == GameModeType.Story ? storyModeSceneName : normalModeSceneName;

        if (MenuExitTrigger.Instance != null)
            MenuExitTrigger.Instance.TriggerExit(sceneName);
        else
            SceneManager.LoadScene(sceneName);
    }

    // ============================================================
    //  SETTINGS
    // ============================================================
    public void OnClickSettings()
    {
        if (settingsPanel == null) return;
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    // ============================================================
    //  EXIT
    // ============================================================
    public void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ============================================================
    //  HELPER
    // ============================================================
    public void SetRightBarVisible(bool visible)
    {
        if (rightBar != null)
            rightBar.SetActive(visible);
    }
}