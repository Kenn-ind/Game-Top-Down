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
        if (statUI != null && statUI.gameObject.activeInHierarchy) return;
        if (ChestController.IsChestOpen) return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            menuCanvas.SetActive(!menuCanvas.activeSelf);
            MobileInput.Instance?.SetMobileUIVisible(!menuCanvas.activeSelf);
        }

        if (menuCanvas.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverMenu())
            {
                menuCanvas.SetActive(false);
                MobileInput.Instance?.SetMobileUIVisible(true);
            }
        }
    }

    bool IsPointerOverMenu()
    {
        UnityEngine.EventSystems.PointerEventData ped =
            new UnityEngine.EventSystems.PointerEventData(
                UnityEngine.EventSystems.EventSystem.current);
        ped.position = Input.mousePosition;

        List<UnityEngine.EventSystems.RaycastResult> results =
            new List<UnityEngine.EventSystems.RaycastResult>();

        UnityEngine.EventSystems.EventSystem.current.RaycastAll(ped, results);

        foreach (var r in results)
        {
            if (r.gameObject.transform.IsChildOf(menuCanvas.transform))
                return true;
        }
        return false;
    }

    public void ToggleMenu()
    {
        if (statUI != null && statUI.gameObject.activeInHierarchy) return;
        if (ChestController.IsChestOpen) return;

        menuCanvas.SetActive(!menuCanvas.activeSelf);
        MobileInput.Instance?.SetMobileUIVisible(!menuCanvas.activeSelf);
    }
}