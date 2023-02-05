using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioSource[] playerMovement, playerCombat, pickup, enemy, boss;
    private List<float> playerCombatPitches = new List<float>();
    private List<float> playerMovementPitches = new List<float>();
    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private AudioSource musicSource;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        for (int i = 0; i < playerMovement.Length; i++)
        {
            playerMovementPitches.Add(playerMovement[i].pitch);
        }
        for (int i = 0; i < playerCombat.Length; i++)
        {
            playerCombatPitches.Add(playerCombat[i].pitch);
        }
    }

    public void PlaySFX(string source, int index)
    {
        if (source == "PlayerMovement") playerMovement[index].Play();
        else if (source == "PlayerCombat") playerCombat[index].Play();
        else if (source == "Pickup") pickup[index].Play();
        else if (source == "Enemy") enemy[index].Play();
        else if (source == "Boss") boss[index].Play();
    }

    public void PlayAdjustedSFX(string source, int index, float offset)
    {
        float randOffset = Random.Range(offset * -1f, offset);
        if (source == "PlayerMovement")
        {
            playerMovement[index].pitch = playerMovementPitches[index] + randOffset;
            playerMovement[index].Play();
        }
        else if (source == "PlayerCombat")
        {
            playerCombat[index].pitch = playerCombatPitches[index] + randOffset;
            playerCombat[index].Play();
        }
    }

    public void PlayMusic(int index, float volume = 1f)
    {
        SetMusicVolume(volume);
        musicSource.clip = musicClips[index];
        musicSource.Play();
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void FadeOutMusic(float fadeTime)
    {
        StartCoroutine(FadeOutMusicCoroutine(fadeTime));
    }

    private IEnumerator FadeOutMusicCoroutine(float fadeTime)
    {
        while (musicSource.volume > 0f)
        {
            musicSource.volume -= (Time.unscaledDeltaTime/fadeTime);
            yield return null;
        }
    }

    public void FadeInMusic(float fadeTime)
    {
        StartCoroutine(FadeInMusicCoroutine(fadeTime));
    }

    private IEnumerator FadeInMusicCoroutine(float fadeTime)
    {
        while (musicSource.volume < 1f)
        {
            musicSource.volume += (Time.unscaledDeltaTime / fadeTime);
            yield return null;
        }
    }
}
