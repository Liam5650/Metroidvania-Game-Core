using Cinemachine;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject mainMenuScreen;
    [SerializeField] GameObject hud;
    [SerializeField] GameObject player;
    [SerializeField] Image fadeScreen;
    [SerializeField] float fadeTime;
    [SerializeField] float fadeHoldTime;
    [SerializeField] float roomTransitionDamping;
    [SerializeField] float menuFadeTime;
    [SerializeField] float menuFadeHoldTime;
    [SerializeField] Button continueButton;
    [SerializeField] string roomToDebug;
    private bool isPaused;
    private bool inTransition;
    [SerializeField] GameObject messageScreen;
    [SerializeField] TextMeshProUGUI messageText;
    private SaveController saveController;
    [SerializeField] private GameObject mapScreen;
    private bool viewingMap;
    [SerializeField] private MapController mapController;
    public static UIController instance;

    private void Awake()
    {
        // Set up instance
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        saveController = gameObject.GetComponent<SaveController>();

        // Enable continue button if we have save data
        if (saveController.HasSave()) continueButton.interactable = true;
        else continueButton.interactable = false;

        AudioManager.instance.PlayMusic(0);
    }

    void Update()
    {
        // Disable input under certain conditions
        if (!inTransition && SceneManager.GetActiveScene().name != "MainMenu" && !messageScreen.activeSelf)
        {
            // Pause Menu input handling
            if (!viewingMap && Input.GetKeyDown(KeyCode.Escape))
            {
                if (!isPaused)
                {
                    Time.timeScale = 0f;
                    isPaused = true;
                    pauseScreen.SetActive(true);
                    AudioManager.instance.SetMusicVolume(0.5f);
                    AudioManager.instance.PlaySFX("UI", 1);
                }
                else
                {
                    Time.timeScale = 1f;
                    isPaused = false;
                    pauseScreen.SetActive(false);
                    AudioManager.instance.SetMusicVolume(1f);
                    AudioManager.instance.PlaySFX("UI", 2);
                }
            }
            // Map Menu input handling
            if (!isPaused && Input.GetKeyDown(KeyCode.M))
            {
                if (!viewingMap)
                {
                    Time.timeScale = 0f;
                    viewingMap = true;
                    mapScreen.SetActive(true);
                    mapController.ViewingMap(true);
                    AudioManager.instance.SetMusicVolume(0.5f);
                    AudioManager.instance.PlaySFX("UI", 1);
                }
                else
                {
                    Time.timeScale = 1f;
                    viewingMap = false;
                    mapScreen.SetActive(false);
                    mapController.ViewingMap(false);
                    AudioManager.instance.SetMusicVolume(1f);
                    AudioManager.instance.PlaySFX("UI", 2);
                }
            }
        }
    }

    // Public getter as UI controller persists through scene loads
    public void LoadRoom (string sceneName)
    {
        StartCoroutine(RoomTransition(sceneName, fadeTime, fadeHoldTime));
    }
  
    private IEnumerator RoomTransition(string sceneName, float fadeTime = 0f, float fadeHoldTime = 0f)
    {
        // Suspend play
        MarkTransition(true);

        // Reference the scene we are moving from to disable later
        string oldScene = SceneManager.GetActiveScene().name;

        // Make the player appear above the ui blackscreen for room transitions
        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
        int sortOrder = playerSprite.sortingOrder;
        playerSprite.sortingOrder = 10000;

        // Fade in and complete the scene transition
        yield return (StartCoroutine(FadeTransition("in", fadeTime)));
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        yield return SceneManager.UnloadSceneAsync(oldScene);

        // Update the camera with the new bounds and move it to the new position using the damping
        FindObjectOfType<TrackPlayer>().GetBounds();
        GameObject.FindObjectOfType<CinemachineConfiner2D>().m_Damping = roomTransitionDamping;
        yield return new WaitForSecondsRealtime(fadeHoldTime);
        GameObject.FindObjectOfType<CinemachineConfiner2D>().m_Damping = 0f;

        // Fade out and reset the player sorting order, then resume play
        yield return (StartCoroutine(FadeTransition("out", fadeTime)));
        playerSprite.sortingOrder = sortOrder;
        MarkTransition(false);

    }

    public void LoadMenu()
    {
        StartCoroutine(TransitionToMenu(menuFadeTime, menuFadeHoldTime));
        isPaused = false;
    }

    private IEnumerator TransitionToMenu(float fadeTime = 0f, float fadeHoldTime = 0f)
    {
        // Suspend play and fade in 
        MarkTransition(true);
        AudioManager.instance.FadeOutMusic(fadeTime);
        yield return (StartCoroutine(FadeTransition("in", fadeTime)));

        // Reference the scene we are moving from to disable later
        string oldScene = SceneManager.GetActiveScene().name;

        // Do required cleanup based on loading into menu
        player.SetActive(false);
        pauseScreen.SetActive(false);
        hud.SetActive(false);
        mainMenuScreen.SetActive(true);
        GameObject[] shots = GameObject.FindGameObjectsWithTag("Shot");
        foreach (GameObject shot in shots)
            GameObject.Destroy(shot);

        // Complete the scene transition
        yield return SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainMenu"));
        yield return SceneManager.UnloadSceneAsync(oldScene);

        // Update the camera with the new bounds, wait for delay, then fade out and resume play
        FindObjectOfType<TrackPlayer>().GetBounds();
        player.transform.position = saveController.playerData.playerPosition;
        mapController.LoadMap(saveController.playerData.roomsVisited);
        yield return new WaitForSecondsRealtime(fadeHoldTime);
        AudioManager.instance.PlayMusic(0);
        yield return (StartCoroutine(FadeTransition("out", fadeTime)));
        MarkTransition(false);
    }

    public void NewGame()
    {
        AudioManager.instance.PlaySFX("UI", 0);
        saveController.ClearSave();
        saveController.SaveData();
        StartCoroutine(TransitionFromMenu(saveController.playerData.roomName, menuFadeTime, menuFadeHoldTime));
    }

    public void Continue()
    {
        AudioManager.instance.PlaySFX("UI", 0);
        StartCoroutine(TransitionFromMenu(saveController.playerData.roomName, menuFadeTime, menuFadeHoldTime));
    }

    private IEnumerator TransitionFromMenu(string sceneName, float fadeTime = 0f, float fadeHoldTime = 0f)
    {
        // Suspend play and fade in 
        MarkTransition(true);
        AudioManager.instance.FadeOutMusic(fadeTime);
        yield return (StartCoroutine(FadeTransition("in", fadeTime)));
        if (saveController.HasSave()) continueButton.interactable = true;

        // Set up player
        hud.SetActive(true);
        player.SetActive(true);
        mainMenuScreen.SetActive(false);

        // Complete the scene transition
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        yield return SceneManager.UnloadSceneAsync("MainMenu");

        // Update the camera with the new bounds, wait for delay, then fade out and resume play
        FindObjectOfType<TrackPlayer>().GetBounds();
        player.transform.position = saveController.playerData.playerPosition;
        mapController.LoadMap(saveController.playerData.roomsVisited);
        yield return new WaitForSecondsRealtime(fadeHoldTime);
        AudioManager.instance.PlayMusic(1);
        yield return (StartCoroutine(FadeTransition("out", fadeTime)));
        MarkTransition(false);
    }

    private IEnumerator FadeTransition(string direction, float fadeTime)
    {
        float timeWaited = 0f;
        if (direction == "in")
        {
            // Fade in black screen
            while (timeWaited < fadeTime)
            {
                timeWaited += Time.unscaledDeltaTime;
                fadeScreen.color = new Color(0f, 0f, 0f, timeWaited / fadeTime);
                yield return null; // Wait for new frame
            }
        }
        else if (direction == "out")
        {
            // Fade out the black screen
            while (timeWaited < fadeTime)
            {
                timeWaited += Time.unscaledDeltaTime;
                fadeScreen.color = new Color(0f, 0f, 0f, 1f - (timeWaited / fadeTime));
                yield return null; // Wait for new frame
            }
        }
    }

    private void MarkTransition(bool value)
    { 
        if (value == true)
        {
            inTransition = true;
            fadeScreen.gameObject.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            inTransition = false;
            fadeScreen.gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void ResumePlay()
    {
        AudioManager.instance.PlaySFX("UI", 0);
        AudioManager.instance.SetMusicVolume(1f);
        Time.timeScale = 1f;
        isPaused = false;
        pauseScreen.SetActive(false);
    }

    public void QuitGame()
    {
        AudioManager.instance.PlaySFX("UI", 0);
        Debug.Log("Application Quit");
        Application.Quit();
    }

    public void DebugRoom()
    {
        AudioManager.instance.PlaySFX("UI", 0);
        if (roomToDebug!= null)
        {
            StartCoroutine(DebugTransition());
        }
    }

    private IEnumerator DebugTransition()
    {
        yield return StartCoroutine(TransitionFromMenu(roomToDebug, menuFadeTime, menuFadeHoldTime));
        player.transform.position = new Vector3(hud.transform.position.x, hud.transform.position.y, 0);
    }


    public void DisplayMessage(string message, float displayTime = 2)
    {
        StartCoroutine(DisplayMessageCoroutine(message, displayTime));
    }

    private IEnumerator DisplayMessageCoroutine(string message, float displayTime = 2)
    {
        Time.timeScale = 0f;
        messageText.text = message;
        messageScreen.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(displayTime);
        messageScreen.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}