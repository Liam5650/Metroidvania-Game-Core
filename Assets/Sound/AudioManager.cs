using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;                            // Set up instance so that other scripts can easily play audio
    [SerializeField] private AudioSource[] playerMovement, playerCombat, playerHealth, pickup, enemy, boss, userInterface;    // Various audio sources
    private List<float> playerCombatPitches = new List<float>();    // Used so we can get the initial values so we can apply offsets later
    private List<float> playerMovementPitches = new List<float>();  // Used so we can get the initial values so we can apply offsets later
    [SerializeField] private AudioClip[] musicClips;                // Various clips for game music
    [SerializeField] private AudioSource musicSource;               // Source to play music from

    private void Awake()
    {
        // Set up instance
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);


        // Get reference of initial set pitch values for effects we want to modify the pitch of later for variety
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
        // Play audio through the correct source
        if      (source == "PlayerMovement") playerMovement[index].Play();
        else if (source == "PlayerCombat") playerCombat[index].Play();
        else if (source == "PlayerHealth") playerHealth[index].Play();
        else if (source == "Pickup") pickup[index].Play();
        else if (source == "Enemy") enemy[index].Play();
        else if (source == "Boss") boss[index].Play();
        else if (source == "UI") userInterface[index].Play();
    }

    public void PlayAdjustedSFX(string source, int index, float offset)
    {
        // Play the audio with a random offset through the correct source
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
        // Set the game music to be played
        SetMusicVolume(volume);
        musicSource.clip = musicClips[index];
        musicSource.Play();
    }

    public void SetMusicVolume(float volume)
    {
        // Adjust music volume
        musicSource.volume = volume;
    }

    public void FadeOutMusic(float fadeTime)
    {
        // Used to fade music between menu transitions, etc. 
        StartCoroutine(FadeOutMusicCoroutine(fadeTime));
    }

    private IEnumerator FadeOutMusicCoroutine(float fadeTime)
    {
        // Fades the music volume out
        while (musicSource.volume > 0f)
        {
            musicSource.volume -= (Time.unscaledDeltaTime/fadeTime);
            yield return null;
        }
    }

    public void FadeInMusic(float fadeTime)
    {
        // Used to fade music between menu transitions, etc. 
        StartCoroutine(FadeInMusicCoroutine(fadeTime));
    }

    private IEnumerator FadeInMusicCoroutine(float fadeTime)
    {
        // Fades the music volume in
        while (musicSource.volume < 1f)
        {
            musicSource.volume += (Time.unscaledDeltaTime / fadeTime);
            yield return null;
        }
    }
}