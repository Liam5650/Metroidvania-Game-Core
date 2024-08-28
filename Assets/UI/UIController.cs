using Cinemachine;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using Unity.VisualScripting;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject pauseScreen;                    // The pause screen UI
    [SerializeField] GameObject mainMenuScreen;                 // Main menu UI
    [SerializeField] GameObject hud;                            // HUD UI
    [SerializeField] GameObject player;                         // The player gameobject to enable/disable, position at start of game, etc.
    [SerializeField] Image fadeScreen;                          // The blackscreen to fade in between transitions
    [SerializeField] float fadeTime;                            // Time to fade in/out the blackscreen
    [SerializeField] float fadeHoldTime;                        // Time to wait when fully faded for transitions to occur
    [SerializeField] float roomTransitionDamping;               // Speed at which the camera transitions to the new camera bounds
    [SerializeField] float menuFadeTime;                        // Time for the menu to fade out when loading game
    [SerializeField] float menuFadeHoldTime;                    // Time the menu stays faded before the game is entered
    [SerializeField] Button continueButton;                     // Button to hanle continuing a game
    [SerializeField] string roomToDebug;                        // Room name to teleport player to to debug
    private bool isPaused;                                      // Reference if the player has paused the game
    private bool inTransition;                                  // Reference if we are in a scene transition
    [SerializeField] GameObject messageScreen;                  // Message screen UI
    [SerializeField] TextMeshProUGUI messageText;               // Text to display on message screen
    private SaveController saveController;                      // Reference to save controller for checking for or erasing data 
    [SerializeField] private GameObject mapScreen;              // Map screen UI
    private bool viewingMap;                                    // Reference if we are viewing the map
    [SerializeField] private MapController mapController;       // Used to tell the map script we would like to view it
    public static UIController instance;                        // Instance used for easy access by other scripts
    [SerializeField] private GameObject ConfirmMenuScreen;      // Confirm menu screen UI
    [SerializeField] private GameObject ConfirmNewGameScreen;   // Confirm new game screen UI

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
        SetContinueButton(saveController.HasSave());

        // Set target framerate
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
    void Start()
    {
        // Play menu music
        AudioManager.instance.PlayMusic(0);
    }

    void Update()
    {
        // Disable input under certain conditions such as a scene transition, or handle otherwise if we are on the pause screen or map screen
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
                    ConfirmMenuScreen.SetActive(false);
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

    public void LoadRoom (string sceneName)
    {
        // Start a transition to a new room
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
    public void OpenConfirmMenuScreen()
    {
        // Opens the confirm menu UI
        AudioManager.instance.PlaySFX("UI", 3);
        pauseScreen.SetActive(false);
        ConfirmMenuScreen.SetActive(true);
    }

    public void CloseConfirmMenuScreen()
    {
        // Closes the confirm menu UI
        AudioManager.instance.PlaySFX("UI", 3);
        pauseScreen.SetActive(true);
        ConfirmMenuScreen.SetActive(false);
    }

    public void LoadMenu(bool playSFX = true)
    {
        // Load the to the main menu scene
        if (playSFX) AudioManager.instance.PlaySFX("UI", 0);
        StartCoroutine(TransitionToMenu(menuFadeTime, menuFadeHoldTime));
        isPaused = false;
    }

    private IEnumerator TransitionToMenu(float fadeTime = 0f, float fadeHoldTime = 0f)
    {
        // Suspend play and fade in 
        MarkTransition(true);
        AudioManager.instance.FadeOutMusic(fadeTime);
        if (fadeScreen.color.a < 1f) yield return (StartCoroutine(FadeTransition("in", fadeTime)));

        // Reference the scene we are moving from to disable later
        string oldScene = SceneManager.GetActiveScene().name;

        // Set up menu
        player.SetActive(false);
        ConfirmMenuScreen.SetActive(false);
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
        player.transform.position = Vector3.zero;
        yield return new WaitForSecondsRealtime(fadeHoldTime);
        AudioManager.instance.PlayMusic(0);
        yield return (StartCoroutine(FadeTransition("out", fadeTime)));
        MarkTransition(false);
    }
    public void OpenNewGameScreen()
    {
        // Open the new game screen if we have save data to confirm if we want to delete, otherwise just start new game
        if (saveController.HasSave())
        {
            AudioManager.instance.PlaySFX("UI", 3);
            mainMenuScreen.SetActive(false);
            ConfirmNewGameScreen.SetActive(true);
        }
        else
        {
            NewGame();
        }
    }

    public void CloseNewGameScreen()
    {
        // Close the new game screen UI
        AudioManager.instance.PlaySFX("UI", 3);
        mainMenuScreen.SetActive(true);
        ConfirmNewGameScreen.SetActive(false);
    }

    public void NewGame()
    {
        // Initializes a new game
        AudioManager.instance.PlaySFX("UI", 0);
        saveController.ClearSave();
        StartCoroutine(TransitionFromMenu(saveController.playerData.roomName, menuFadeTime, menuFadeHoldTime));
    }

    public void Continue()
    {
        // Continues a previously saved game
        AudioManager.instance.PlaySFX("UI", 0);
        saveController.LoadSave();
        StartCoroutine(TransitionFromMenu(saveController.playerData.roomName, menuFadeTime, menuFadeHoldTime));
    }

    private IEnumerator TransitionFromMenu(string sceneName, float fadeTime = 0f, float fadeHoldTime = 0f)
    {
        // Suspend play and fade in 
        MarkTransition(true);
        AudioManager.instance.FadeOutMusic(fadeTime);
        yield return (StartCoroutine(FadeTransition("in", fadeTime)));

        // Set up player
        hud.SetActive(true);
        player.SetActive(true);
        player.GetComponent<PlayerHealth>().RefreshState();
        player.GetComponent<PlayerCombat>().RefreshState();
        player.GetComponent<Unlocks>().RefreshState();
        hud.GetComponent<HUDController>().RefreshState();
        mainMenuScreen.SetActive(false);
        ConfirmNewGameScreen.SetActive(false);

        // Complete the scene transition
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        yield return SceneManager.UnloadSceneAsync("MainMenu");

        // Update the camera with the new bounds, wait for delay, then fade out and resume play
        FindObjectOfType<TrackPlayer>().GetBounds();
        player.transform.position = saveController.playerData.playerPosition;
        mapController.RefreshMap(saveController.playerData.roomsVisited);
        yield return new WaitForSecondsRealtime(fadeHoldTime);
        AudioManager.instance.PlayMusic(1);
        yield return (StartCoroutine(FadeTransition("out", fadeTime)));
        MarkTransition(false);
    }

    public IEnumerator FadeTransition(string direction, float fadeTime)
    {
        // Use a black screen to make scene transitions more visually consistent
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

    public void MarkTransition(bool value)
    { 
        // Mark that we are in a scene transition so other input can't be given
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
        // Used to start simulating gameplay again after exiting a pause screen, map etc. 
        AudioManager.instance.PlaySFX("UI", 2);
        AudioManager.instance.SetMusicVolume(1f);
        Time.timeScale = 1f;
        isPaused = false;
        pauseScreen.SetActive(false);
    }

    public void QuitGame()
    {
        // Quit the application
        AudioManager.instance.PlaySFX("UI", 0);
        Debug.Log("Application Quit");
        Application.Quit();
    }

    public void DebugRoom()
    {
        // Load to the specified debug room
        AudioManager.instance.PlaySFX("UI", 0);
        if (roomToDebug!= null)
        {
            saveController.LoadSave();
            StartCoroutine(DebugTransition());
        }
    }

    private IEnumerator DebugTransition()
    {
        // Load the debug room and place the player in the middle of the room
        yield return StartCoroutine(TransitionFromMenu(roomToDebug, menuFadeTime, menuFadeHoldTime));
        player.transform.position = new Vector3(hud.transform.position.x, hud.transform.position.y, 0);
    }


    public void DisplayMessage(string message, float displayTime = 2)
    {
        // Start the message UI display coroutine
        StartCoroutine(DisplayMessageCoroutine(message, displayTime));
    }

    private IEnumerator DisplayMessageCoroutine(string message, float displayTime = 2)
    {
        // Display the message
        Time.timeScale = 0f;
        messageText.text = message;
        messageScreen.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(displayTime);
        messageScreen.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void SetContinueButton(bool value)
    {
        // Enable or disable the continue button in the main meny depending on whether or not we have save data
        if (value == true)
        {
            continueButton.interactable = true;
            continueButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().color = new Color(0f, 0f, 0f, 1f);
        }
        else
        {
            continueButton.interactable = false;
            continueButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().color = new Color(0f, 0f, 0f, 0.5f);
        }
    }
}