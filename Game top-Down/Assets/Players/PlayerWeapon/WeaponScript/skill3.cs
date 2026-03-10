using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skill3 : MonoBehaviour
{
    public GameObject shurikenPrefab;

    public int shurikenCount = 6;
    public float orbitRadius = 1.5f;
    public float rotateSpeed = 200f;

    public float duration = 5f;
    public float cooldown = 8f;

    private float nextSkillTime;
    private List<GameObject> shurikens = new List<GameObject>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3) && Time.time >= nextSkillTime)
        {
            StartCoroutine(ActivateRing());
            nextSkillTime = Time.time + cooldown;
        }
    }

    IEnumerator ActivateRing()
    {
        for (int i = 0; i < shurikenCount; i++)
        {
            float angle = i * Mathf.PI * 2 / shurikenCount;

            Vector2 pos = new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            ) * orbitRadius;

            GameObject s = Instantiate(
                shurikenPrefab,
                transform.position + (Vector3)pos,
                Quaternion.identity
            );

            shurikens.Add(s);
        }

        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            for (int i = 0; i < shurikens.Count; i++)
            {
                if (shurikens[i] == null) continue;

                float angle = (Time.time * rotateSpeed + i * 360f / shurikenCount) * Mathf.Deg2Rad;

                Vector2 offset = new Vector2(
                    Mathf.Cos(angle),
                    Mathf.Sin(angle)
                ) * orbitRadius;

                shurikens[i].transform.position = transform.position + (Vector3)offset;
            }

            yield return null;
        }

        foreach (GameObject s in shurikens)
        {
            if (s != null)
                Destroy(s);
        }

        shurikens.Clear();
    }
}