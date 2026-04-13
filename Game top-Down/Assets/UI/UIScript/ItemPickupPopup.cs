using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemPickupPopup : MonoBehaviour
{
    public GameObject popupPanel;
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI stackCountText;

    public float displayDuration = 2f;
    public float fadeDuration = 0.3f;

    private CanvasGroup canvasGroup;
    private Coroutine currentCoroutine;

    public static ItemPickupPopup Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        canvasGroup = popupPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = popupPanel.AddComponent<CanvasGroup>();

        popupPanel.SetActive(false);
    }

    public void Show(ItemData data, int amount = 1)
    {
        if (data == null) return;

        if (itemIcon != null) itemIcon.sprite = data.icon;
        if (itemNameText != null) itemNameText.text = data.itemName;
        if (stackCountText != null)
            stackCountText.text = $"x{amount}";

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ShowRoutine());
    }

    IEnumerator ShowRoutine()
    {
        popupPanel.SetActive(true);

        yield return StartCoroutine(Fade(0f, 1f));

        yield return new WaitForSeconds(displayDuration);

        yield return StartCoroutine(Fade(1f, 0f));

        popupPanel.SetActive(false);
    }

    IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        canvasGroup.alpha = from;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}