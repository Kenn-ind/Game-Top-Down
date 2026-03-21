using System.Collections;
using Cinemachine;
using UnityEngine;

public class MapTransisi : MonoBehaviour
{
    [SerializeField] PolygonCollider2D MapBoundry;
    [SerializeField] Direction direction;
    [SerializeField] Transform TeleportTarget;
    [SerializeField] private float fadeOutDuration = 0.2f;
    [SerializeField] private float blackScreenDuration = 0.5f;
    [SerializeField] private float fadeInDuration = 0.5f;   

    private CinemachineConfiner Confiner;
    private bool isTransitioning = false;

    enum Direction { Up, Down, Right, Left, Teleport }

    private void Awake()
    {
        Confiner = FindObjectOfType<CinemachineConfiner>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTransitioning)
        {
            if (direction == Direction.Teleport)
                StartCoroutine(DoTransitionWithFade(collision.gameObject));
            else
                DoTransitionDirect(collision.gameObject);
        }
    }
    private IEnumerator DoTransitionWithFade(GameObject player)
    {
        isTransitioning = true;

        if (ScreenFader.Instance != null)
            yield return StartCoroutine(ScreenFader.Instance.FadeOut(fadeOutDuration));

        UpdatePlayerPosition(player);
        Confiner.m_BoundingShape2D = MapBoundry;
        Confiner.InvalidatePathCache();

        yield return new WaitForSeconds(blackScreenDuration);

        if (ScreenFader.Instance != null)
            yield return StartCoroutine(ScreenFader.Instance.FadeIn(fadeInDuration));

        isTransitioning = false;
    }

    private void DoTransitionDirect(GameObject player)
    {
        Confiner.m_BoundingShape2D = MapBoundry;
        UpdatePlayerPosition(player);
    }

    private void UpdatePlayerPosition(GameObject player)
    {
        if (direction == Direction.Teleport)
        {
            player.transform.position = TeleportTarget.position;
            return;
        }

        Vector3 newPos = player.transform.position;
        switch (direction)
        {
            case Direction.Up: newPos.y += 2; break;
            case Direction.Down: newPos.y -= 2; break;
            case Direction.Right: newPos.x += 2; break;
            case Direction.Left: newPos.x -= 2; break;
        }
        player.transform.position = newPos;
    }
}