using Cinemachine;
using UnityEngine;

public class MapTransisi : MonoBehaviour
{
    [SerializeField] PolygonCollider2D MapBoundry;
    private CinemachineConfiner Confiner;

    [SerializeField] Direction direction;

    enum Direction { Up, Down, Right, Left }

    private void Awake()
    {
        Confiner = FindObjectOfType<CinemachineConfiner>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Confiner.m_BoundingShape2D = MapBoundry;

            UpdatePlayerPosition(collision.gameObject);
        }
    }

    private void UpdatePlayerPosition(GameObject player)
    {
        Vector3 newPos = player.transform.position;

        switch (direction)
        {
            case Direction.Up:
                newPos.y += 2;
                break;

            case Direction.Down:
                newPos.y -= 2;
                break;

            case Direction.Right:
                newPos.x += 2;
                break;

            case Direction.Left:
                newPos.x -= 2;
                break;
        }

        player.transform.position = newPos;
    }
}