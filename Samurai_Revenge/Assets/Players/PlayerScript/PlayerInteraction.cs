using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 2f;
    public KeyCode interactKey = KeyCode.E;
    public LayerMask interactableLayer;

    void Update()
    {
        Debug.Log("Update berjalan"); // Cek apakah Update dipanggil

        if (Input.GetKeyDown(interactKey))
        {
            Debug.Log("Tombol E ditekan!"); // Cek apakah input terbaca
            TryInteract();
        }
    }

    void TryInteract()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange);

        foreach (Collider2D hit in hits)
        {
            Debug.Log($"Nama: {hit.gameObject.name}");

            // Coba cari di GameObject itu sendiri DAN parent/childnya
            IInteractable interactable = hit.GetComponent<IInteractable>();
            IInteractable interactableParent = hit.GetComponentInParent<IInteractable>();
            IInteractable interactableChild = hit.GetComponentInChildren<IInteractable>();

            Debug.Log($"GetComponent: {interactable}");
            Debug.Log($"GetComponentInParent: {interactableParent}");
            Debug.Log($"GetComponentInChildren: {interactableChild}");

            // Coba ketiganya
            IInteractable found = interactable ?? interactableParent ?? interactableChild;

            if (found != null && found.CanInteract())
            {
                found.Interact();
                return;
            }
        }
    }

    // Visualisasi range di Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}