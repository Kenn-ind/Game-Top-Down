using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public ItemData itemData;
    public int stackCount = 1;

    public float bobSpeed = 2f;
    public float bobHeight = 0.1f;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}