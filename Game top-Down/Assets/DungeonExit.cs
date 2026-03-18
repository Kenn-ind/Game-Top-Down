using UnityEngine;

public class DungeonExit : MonoBehaviour
{
    public DungeonLightEffect lightEffect;
    public Transform teleportDestination;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.transform.position = teleportDestination.position;
            lightEffect.ExitDungeon();
        }
    }
}