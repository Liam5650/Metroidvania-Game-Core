using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject pauseScreen;
    [SerializeField] Image fadeScreen;
    [SerializeField] float fadeTime;
    [SerializeField] float fadeHoldTime;
    private bool isPaused;
    private bool inTransition;

    void Update()
    {
        if (!inTransition)
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

    public void SceneTransition(string sceneName)
    {
        StartCoroutine(LoadLevel(sceneName, fadeTime, fadeHoldTime));
    }

    private IEnumerator LoadLevel(string sceneName, float fadeTime = 0f, float fadeHoldTime = 0f)
    {
        // Make sure player can't move during transition
        Time.timeScale = 0f;

        // Reference the scene we are moving from to disable later
        string oldScene = SceneManager.GetActiveScene().name;

        // Fix the warning where multiple cameras are loaded while both scenes are active
        GameObject camera = GameObject.FindGameObjectWithTag("Camera");

        // Fade in black screen
        float timeWaited = 0f;
        while (timeWaited < fadeTime)
        {
            Debug.Log(timeWaited);
            timeWaited += Time.unscaledDeltaTime;
            fadeScreen.color = new Color(0f, 0f, 0f, timeWaited/fadeTime);

            // Wait for new frame
            yield return null;
        }

        // Wait for new scene to load
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // Disable the camera of the scene we are moving from
        camera.SetActive(false);

        // Set our new active scene
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        // Unload old scene
        yield return SceneManager.UnloadSceneAsync(oldScene);

        // Wait for the fadeHoldTime duration
        yield return new WaitForSecondsRealtime(fadeHoldTime);

        // Fade out the black screen
        timeWaited = 0f;
        while (timeWaited < fadeTime)
        {
            Debug.Log(timeWaited);
            timeWaited += Time.unscaledDeltaTime;
            fadeScreen.color = new Color(0f, 0f, 0f, 1f - (timeWaited / fadeTime));

            // Wait for new frame
            yield return null;
        }

        // Resume play
        Time.timeScale = 1f;
    }
}
