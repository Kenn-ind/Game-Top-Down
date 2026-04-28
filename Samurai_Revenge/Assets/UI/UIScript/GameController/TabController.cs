using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    public Image[] tabImages;
    public GameObject[] pages;
    public StatDisplayUI statDisplay;

    public void ActivateTab(int tabNo)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            tabImages[i].color = Color.grey;
        }
        pages[tabNo].SetActive(true);
        tabImages[tabNo].color = Color.white;

        if (statDisplay != null)
            statDisplay.RefreshDisplay();

        if (tabNo == 3)
            QuestLogUI.Instance?.RefreshLog();

        if (tabNo == 4)
            SaveSlotUI.Instance?.RefreshSlotList();
    }
}
