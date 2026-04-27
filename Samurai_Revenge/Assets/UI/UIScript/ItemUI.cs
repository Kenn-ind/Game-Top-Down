using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemUI : MonoBehaviour
{
    public ItemData itemData;
    public int stackCount = 1;

    public Image iconImage;
    public TextMeshProUGUI stackText;

    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (iconImage != null && itemData != null)
            iconImage.sprite = itemData.icon;

        if (stackText != null)
            stackText.text = (stackCount > 1) ? stackCount.ToString() : "";
    }
}