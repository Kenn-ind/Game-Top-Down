using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : Slot
{
    [Header("Highlight")]
    public Image highlightImage;

    public void SetHighlight(bool active)
    {
        if (highlightImage != null)
            highlightImage.gameObject.SetActive(active);
    }
}