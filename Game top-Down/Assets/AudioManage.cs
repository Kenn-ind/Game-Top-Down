using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManage : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    public AudioClip backsoundMenu;
    public AudioClip backsoundGame;
    public AudioClip backsoundVillage;
    public AudioClip backsoundDungeon;
    public AudioClip S1Shu;
    public AudioClip S2Shu;
    public AudioClip S1Sword;
    public AudioClip S2Sword;
    public AudioClip S3;
    public AudioClip sword;
    public AudioClip shuriken;
    public AudioClip interact;
    public AudioClip enemyHurt;

    // Start is called before the first frame update
    private void Start()
    {
        musicSource.clip = backsoundGame;
        musicSource.Play();
    }

    // Update is called once per frame
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
