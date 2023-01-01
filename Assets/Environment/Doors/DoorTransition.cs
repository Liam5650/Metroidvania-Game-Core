using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTransition : MonoBehaviour
{
    [SerializeField] string roomToLoad;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {

            // Disable the trigger so it cant be hit multiple times, and start the scene switch coroutine
            gameObject.GetComponent<BoxCollider2D>().enabled = false;  
            StartCoroutine(LoadLevel(roomToLoad));
        }
    }

    private static IEnumerator LoadLevel(string sceneName)
    {
        // Make sure player can't move during transition
        Time.timeScale = 0f;
        
        // Reference the scene we are moving from to disable later
        string oldScene = SceneManager.GetActiveScene().name;

        // Fix the warning where multiple cameras are loaded while both scenes are active
        GameObject camera =  GameObject.FindGameObjectWithTag("Camera");

        // Wait for new scene to load
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // Disable the camera of the scene we are moving from
        camera.gameObject.SetActive(false);

        // Set our new active scene
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        // Resume player movement and unload old scene
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync(oldScene);
    }
}
