using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public AudioClip enemyAtk;
    public AudioClip enemyDie;
    public AudioClip DashSfx;
    public AudioClip DialogSfx;
    public AudioClip ButtonSfx;
    public AudioClip DoorSfx;
    public AudioClip RespawnSfx;

    [Header("Fade Settings")]
    [SerializeField] float fadeDuration = 1.5f;
    [SerializeField] float targetVolume = 1f;

    private Coroutine fadeCoroutine;

    public static AudioManage Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        musicSource.volume = targetVolume;
        PlayMusicForCurrentScene();
    }

    private void PlayMusicForCurrentScene()
    {
        // Ambil nama scene yang sedang aktif
        string currentScene = SceneManager.GetActiveScene().name;

        switch (currentScene)
        {
            case "Menu":
                PlayMusic(backsoundMenu);
                break;

            case "SampleScene":
                PlayMusic(backsoundGame);
                break;

            //case "BossStage":
            //    PlayMusic(backsoundBoss);
            //    break;

            //case "Ending":
            //    PlayMusic(backsoundEnding);
            //    break;

            default:
                Debug.LogWarning($"[MusicManager] Scene '{currentScene}' tidak punya backsound.");
                musicSource.Stop();
                break;
        }
    }

    // ─── SFX ──────────────────────────────────────────────────────
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    // ─── Ganti musik dengan crossfade ─────────────────────────────
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource.clip == clip) return;

        if (musicSource.clip == clip && musicSource.isPlaying) return;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeToMusic(clip));
    }

    private IEnumerator FadeToMusic(AudioClip newClip)
    {
        // Fade out — turun dari targetVolume ke 0
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(targetVolume, 0f, elapsed / fadeDuration);
            yield return null;
        }
        musicSource.volume = 0f;

        // Swap clip
        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in — naik dari 0 ke targetVolume
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / fadeDuration);
            yield return null;
        }
        musicSource.volume = targetVolume;
    }

    // ─── Shortcut per zona ────────────────────────────────────────
    public void EnterDungeon() => PlayMusic(backsoundDungeon);
    public void EnterVillage() => PlayMusic(backsoundVillage);
    public void EnterOverworld() => PlayMusic(backsoundGame);
    public void EnterMenu() => PlayMusic(backsoundMenu);
}