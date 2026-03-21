using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    public enum ZoneType { Overworld, Village, Dungeon, Menu }

    [SerializeField] ZoneType zone;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        switch (zone)
        {
            case ZoneType.Dungeon: AudioManage.Instance.EnterDungeon(); break;
            case ZoneType.Village: AudioManage.Instance.EnterVillage(); break;
            case ZoneType.Overworld: AudioManage.Instance.EnterOverworld(); break;
            case ZoneType.Menu: AudioManage.Instance.EnterMenu(); break;
        }
    }
}