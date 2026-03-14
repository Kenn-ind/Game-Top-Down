using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    public GameObject popupPrefab;
    public Vector3 popupOffset = new Vector3(0, 1f, 0);
    public float detectRadius = 2f;

    public float bobSpeed = 2f;
    public float bobHeight = 0.08f;

    private GameObject popupInstance;
    private bool isShowing = false;
    private Vector3 popupBasePos;

    public LayerMask playerLayer;

    void Update()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            transform.position, detectRadius, playerLayer
        );

        if (hit != null)
        {
            if (!isShowing) ShowPopup();
            BobPopup();
        }
        else
        {
            if (isShowing) HidePopup();
        }
    }

    void ShowPopup()
    {
        if (popupPrefab == null) return;

        popupBasePos = transform.position + popupOffset;
        popupInstance = Instantiate(popupPrefab, popupBasePos, Quaternion.identity);
        popupInstance.transform.SetParent(transform);
        popupInstance.transform.localPosition = popupOffset;
        isShowing = true;
    }

    void HidePopup()
    {
        if (popupInstance != null)
            Destroy(popupInstance);
        isShowing = false;
    }

    void BobPopup()
    {
        if (popupInstance == null) return;

        float newY = popupOffset.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        popupInstance.transform.localPosition = new Vector3(
            popupOffset.x, newY, popupOffset.z
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}