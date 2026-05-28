using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialDescPanel : MonoBehaviour
{
    public static TutorialDescPanel Instance { get; private set; }

    [Header("Panel")]
    public GameObject descPanel;

    [Header("UI References")]
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text pageText; // "1 / 3"
    public Button nextButton;
    public Button prevButton;
    public Button closeButton;

    private List<TutorialDescPage> pages = new List<TutorialDescPage>();
    private int currentPageIndex = 0;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        descPanel?.SetActive(false);

        nextButton?.onClick.AddListener(NextPage);
        prevButton?.onClick.AddListener(PrevPage);
        closeButton?.onClick.AddListener(ClosePanel);
    }

    // ─── Buka Panel ───────────────────────────────────────────────────────────

    public void ShowPages(List<TutorialDescPage> newPages)
    {
        if (newPages == null || newPages.Count == 0) return;

        pages = newPages;
        currentPageIndex = 0;
        descPanel?.SetActive(true);
        DisplayCurrentPage();
    }

    public void ReopenPanel()
    {
        Debug.Log($"[DescPanel] ReopenPanel dipanggil, pages count={pages?.Count ?? 0}");
        if (pages == null || pages.Count == 0)
        {
            Debug.Log("[DescPanel] Pages kosong!");
            return;
        }
        currentPageIndex = 0;
        descPanel?.SetActive(true);
        Debug.Log($"[DescPanel] Panel aktif: {descPanel?.activeSelf}");
        DisplayCurrentPage();
    }

    // ─── Display ──────────────────────────────────────────────────────────────

    void DisplayCurrentPage()
    {
        if (pages == null || currentPageIndex >= pages.Count) return;

        TutorialDescPage page = pages[currentPageIndex];

        if (titleText != null) titleText.text = page.title;
        if (descriptionText != null) descriptionText.text = page.description;
        if (pageText != null) pageText.text = $"{currentPageIndex + 1} / {pages.Count}";

        // Prev button hanya muncul jika bukan halaman pertama
        if (prevButton != null)
            prevButton.gameObject.SetActive(currentPageIndex > 0);

        // Next button hanya muncul jika bukan halaman terakhir
        if (nextButton != null)
            nextButton.gameObject.SetActive(currentPageIndex < pages.Count - 1);

        // Close button selalu muncul
        if (closeButton != null)
            closeButton.gameObject.SetActive(true);
    }

    // ─── Navigation ──────────────────────────────────────────────────────────

    void NextPage()
    {
        if (currentPageIndex < pages.Count - 1)
        {
            currentPageIndex++;
            DisplayCurrentPage();
        }
    }

    void PrevPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            DisplayCurrentPage();
        }
    }

    public void ClosePanel()
    {
        descPanel?.SetActive(false);
    }
}

[System.Serializable]
public class TutorialDescPage
{
    public string title;
    [TextArea] public string description;
}