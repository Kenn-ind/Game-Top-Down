using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManage : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    public AudioClip backsoundMenu;
    public AudioClip backsoundGame;
    public AudioClip backsoundVillage;
    public AudioClip backsoundDungeon;

    public AudioClip S1Shu;
    public AudioClip S2Shu;
    public AudioClip S1Sword;
    public AudioClip S2Sword;
    public AudioClip S3;
    public AudioClip sword;
    public AudioClip shuriken;
    public AudioClip interact;
    public AudioClip enemyHurt;

    [Header("Fade Settings")]
    [SerializeField] float fadeDuration = 1.5f;

    private Coroutine fadeCoroutine;

    // ── Singleton biar bisa dipanggil dari mana saja ──────────────
    public static AudioManage Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayMusic(backsoundGame);
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource.clip == clip) return;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeToMusic(clip));
    }

    private IEnumerator FadeToMusic(AudioClip newClip)
    {
        float startVol = musicSource.volume;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVol, 0f, elapsed / fadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();


        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, startVol, elapsed / fadeDuration);
            yield return null;
        }

        musicSource.volume = startVol;
    }

    // ─── Shortcut per zona ────────────────────────────────────────
    public void EnterDungeon() => PlayMusic(backsoundDungeon);
    public void EnterVillage() => PlayMusic(backsoundVillage);
    public void EnterOverworld() => PlayMusic(backsoundGame);
    public void EnterMenu() => PlayMusic(backsoundMenu);
}