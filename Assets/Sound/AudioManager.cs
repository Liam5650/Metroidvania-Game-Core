using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioSource[] playerMovement; 

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

    public void PlaySFX(string source, int index)
    {
        if (source == "PlayerMovement") playerMovement[index].Play();
    }

    public void PlayMusic(int index)
    {
        //
    }
}
