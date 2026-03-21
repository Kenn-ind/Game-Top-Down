using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            fadeImage.raycastTarget = false;
            fadeImage.gameObject.SetActive(false);
        }
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        Color c = fadeImage.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / duration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = to;
        fadeImage.color = c;
    }

    public IEnumerator FadeOut(float duration = -1f)
    {
        fadeImage.gameObject.SetActive(true);
        float d = duration > 0 ? duration : fadeDuration;
        yield return Fade(0f, 1f, d);
        fadeImage.raycastTarget = true;
    }

    public IEnumerator FadeIn(float duration = -1f)
    {
        float d = duration > 0 ? duration : fadeDuration;
        yield return Fade(1f, 0f, d);
        fadeImage.raycastTarget = false;
        fadeImage.gameObject.SetActive(false);
    }
}