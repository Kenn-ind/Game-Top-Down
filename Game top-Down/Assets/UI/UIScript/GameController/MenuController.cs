using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;
    public StatUpgradeUI statUI;
    void Start()
    {
        menuCanvas.SetActive(false);
    }

    void Update()
    {
        if (statUI != null && statUI.gameObject.activeInHierarchy)
            return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            menuCanvas.SetActive(!menuCanvas.activeSelf);
        }
    }
}
