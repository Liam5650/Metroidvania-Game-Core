using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject pauseScreen;
    private bool isPaused;

    void Update()
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
