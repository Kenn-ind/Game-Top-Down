using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuButtons : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject pauseMenuUI; // Drag panel ini di Inspector
    public GameObject panelPauseUI;

    public void OnPauseButton()
    {
        pauseMenuUI.SetActive(true);
        panelPauseUI.SetActive(true);
        Time.timeScale = 0f; // Pause game
    }

    // Tombol KIRI - Continue (sembunyikan UI)
    public void OnContinueButton()
    {
        pauseMenuUI.SetActive(false);
        panelPauseUI.SetActive(false);
        Time.timeScale = 1f; // Resume game jika sebelumnya di-pause
    }

    // Tombol TENGAH - Restart scene saat ini
    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Tombol KANAN - Pergi ke scene pertama (index 0)
    public void OnHomeButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

}