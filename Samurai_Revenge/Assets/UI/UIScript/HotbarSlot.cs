using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : Slot
{
    [Header("Highlight")]
    public Image highlightImage;

    public void SetHighlight(bool active)
    {
        if (highlightImage == null) return;

        highlightImage.gameObject.SetActive(active);

        // Paksa render di atas semua child slot
        Canvas highlightCanvas = highlightImage.GetComponent<Canvas>();
        if (highlightCanvas == null)
            highlightCanvas = highlightImage.gameObject.AddComponent<Canvas>();

        highlightCanvas.overrideSorting = true;
        highlightCanvas.sortingOrder = 10;
    }
}