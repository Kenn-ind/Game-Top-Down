using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DungeonLightEffect : MonoBehaviour
{
    public Light2D playerLight;
    public Light2D globalLight;

    void Start()
    {
        foreach (Light2D light in FindObjectsOfType<Light2D>())
        {
            if (light.lightType == Light2D.LightType.Global)
            {
                globalLight = light;
                break;
            }
        }

        if (globalLight == null)
            Debug.LogError("Global Light 2D tidak ditemukan!");

        playerLight.enabled = false;
    }

    public void EnterDungeon()
    {
        Debug.Log("EnterDungeon dipanggil!");
        globalLight.gameObject.SetActive(false);
        playerLight.enabled = true;
        Debug.Log("playerLight enabled: " + playerLight.enabled);
        Debug.Log("globalLight active: " + globalLight.gameObject.activeSelf);
    }

    public void ExitDungeon()
    {
        Debug.Log("ExitDungeon dipanggil!");
        globalLight.gameObject.SetActive(true);
        playerLight.enabled = false;
    }
}