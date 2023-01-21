using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioSource musicSource, sfxSource;
    [SerializeField] private AudioClip[] music, weaponSFX;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void PlaySFX(int index)
    {
        sfxSource.PlayOneShot(weaponSFX[index]);
    }
    public void PlayMusic(int index)
    {
        musicSource.PlayOneShot(music[index]);
    }
}
