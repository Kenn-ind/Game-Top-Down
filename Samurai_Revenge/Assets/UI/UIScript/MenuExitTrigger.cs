using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuExitTrigger : MonoBehaviour
{
    public static MenuExitTrigger Instance;

    [Header("=== Player & Exit ===")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private float walkSpeed = 2.5f;

    [Header("=== Fade Settings ===")]
    [SerializeField] private float fadeOutDuration = 0.4f;
    [SerializeField] private float blackDuration = 0.3f;

    [Header("=== Scene ===")]
    [SerializeField] private string targetScene = "GameScene";

    private movement _movement;
    private Animator _animator;
    private bool _isExiting = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        if (player != null)
        {
            _movement = player.GetComponent<movement>();
            _animator = player.GetComponent<Animator>();

            // Lock player dari awal di scene menu
            if (_movement != null)
                _movement.SetMovementLocked(true);
        }
    }

    public void TriggerExit(string sceneName = null)
    {
        if (_isExiting) return;
        targetScene = sceneName ?? targetScene;
        StartCoroutine(ExitRoutine());
    }

    IEnumerator ExitRoutine()
    {
        _isExiting = true;

        // 1. Lock player manual dulu sebelum fade
        if (_movement != null)
            _movement.SetMovementLocked(true);

        // 2. Hadapkan ke pintu
        if (_movement != null)
            _movement.FaceTowards(exitPoint.position);

        // 3. Animasi jalan ke pintu
        if (_animator != null)
        {
            _animator.SetBool("IsWalking", true);
            _animator.SetFloat("InputX", 0f);
            _animator.SetFloat("InputY", -1f);
        }

        // 4. Gerak ke exitPoint
        while (Vector2.Distance(player.position, exitPoint.position) > 0.08f)
        {
            player.position = Vector2.MoveTowards(
                player.position,
                exitPoint.position,
                walkSpeed * Time.deltaTime
            );
            yield return null;
        }

        // 5. Stop animasi
        if (_animator != null)
        {
            _animator.SetBool("IsWalking", false);
            _animator.SetFloat("InputX", 0f);
            _animator.SetFloat("InputY", 0f);
        }

        // 6. FadeOut (ScreenFader sudah auto lock player, tapi player sudah locked)
        if (ScreenFader.Instance != null)
            yield return StartCoroutine(ScreenFader.Instance.FadeOut(fadeOutDuration));

        yield return new WaitForSeconds(blackDuration);

        // 7. Pindah scene
        SceneManager.LoadScene(targetScene);
    }
}