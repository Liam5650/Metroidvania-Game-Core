using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject mainMenuScreen;
    [SerializeField] GameObject hud;
    [SerializeField] GameObject player;
    [SerializeField] Image fadeScreen;
    [SerializeField] float fadeTime;
    [SerializeField] float fadeHoldTime;
    [SerializeField] Button continueButton;
    private bool isPaused;
    private bool inTransition;

    private void Awake()
    {
        // Create the ability to put the UI in any scene to start for testing
        string currScene = SceneManager.GetActiveScene().name;
        if (currScene == "MainMenu")
        {
            mainMenuScreen.SetActive(true);
            hud.SetActive(false);
        }
        else
        {
            mainMenuScreen.SetActive(false);
            hud.SetActive(true);
        }

        // Set up continue button if we have save data
        if (PlayerPrefs.HasKey("Room"))
        {
            continueButton.interactable = true;
        }
        else
        {
            continueButton.interactable = false;
        }

    }

    void Update()
    {
        if (!inTransition && SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!isPaused)
                {
                    Time.timeScale = 0f;
                    isPaused = true;
                    pauseScreen.SetActive(true);
                }
                else
                {
                    Time.timeScale = 1f;
                    isPaused = false;
                    pauseScreen.SetActive(false);
                }
            }
        }
    }

    public void SceneTransition (string sceneName)
    {
        StartCoroutine(LoadLevel(sceneName, fadeTime, fadeHoldTime));
    }
  
    private IEnumerator LoadLevel(string sceneName, float fadeTime = 0f, float fadeHoldTime = 0f)
    {
        inTransition = true;
        fadeScreen.gameObject.SetActive(true);
        Time.timeScale = 0f;

        // Reference the scene we are moving from to disable later
        string oldScene = SceneManager.GetActiveScene().name;

        // Reference active camera to disable when both levels are briefly active simultaneously
        GameObject camera = GameObject.FindGameObjectWithTag("Camera");

        // Fade in black screen
        float timeWaited = 0f;
        while (timeWaited < fadeTime)
        {
            timeWaited += Time.unscaledDeltaTime;
            fadeScreen.color = new Color(0f, 0f, 0f, timeWaited/fadeTime); 
            yield return null; // Wait for new frame
        }

        // Do required actions based on if we are loading into a level or the menu
        if (sceneName == "MainMenu")
        {
            player.SetActive(false);
            pauseScreen.SetActive(false);
            mainMenuScreen.SetActive(true);
            hud.SetActive(false);
            GameObject[] shots = GameObject.FindGameObjectsWithTag("Shot");
            foreach (GameObject shot in shots)
                GameObject.Destroy(shot);
        }
        else if (oldScene == "MainMenu")
        {
            hud.SetActive(true);
            player.SetActive(true);
            mainMenuScreen.SetActive(false);
        }

        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // Disable the camera of the scene we are moving from
        camera.SetActive(false);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        yield return SceneManager.UnloadSceneAsync(oldScene);
        yield return new WaitForSecondsRealtime(fadeHoldTime);

        // Fade out the black screen
        timeWaited = 0f;
        while (timeWaited < fadeTime)
        {
            timeWaited += Time.unscaledDeltaTime;
            fadeScreen.color = new Color(0f, 0f, 0f, 1f - (timeWaited / fadeTime));
            yield return null; // Wait for new frame
        }

        inTransition = false;
        fadeScreen.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ResumePlay()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pauseScreen.SetActive(false);
    }

    public void LoadMenu()
    {
        StartCoroutine(LoadLevel("MainMenu", 1f, 0.5f));
        PlayerPrefs.SetFloat("Xpos", player.transform.position.x);
        PlayerPrefs.SetFloat("Ypos", player.transform.position.y);
        PlayerPrefs.SetString("Room", SceneManager.GetActiveScene().name);
        isPaused = false;
        continueButton.interactable = true;
    }

    public void QuitGame()
    {
        Debug.Log("Application Quit");
        Application.Quit();
    }

    public void NewGame(string levelName)
    {
        PlayerPrefs.DeleteAll();
        player.transform.position = Vector3.zero;
        StartCoroutine(LoadLevel("Room1", 1f, 0.5f));
    }

    public void Continue()
    {
        player.transform.position = new Vector3(PlayerPrefs.GetFloat("Xpos"), PlayerPrefs.GetFloat("Ypos"), 0);
        StartCoroutine(LoadLevel(PlayerPrefs.GetString("Room"), 1f, 0.5f));
    }
}
