using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveStation : MonoBehaviour
{
    [SerializeField] float dropSpeed, idleSpeed, dropAmount, waveAmplitude, waveFrequency;
    private string stationState;
    private Vector3 startPos;
    private float sinTimer;
    private bool hasIdled;

    void Start()
    {
        startPos = transform.position;
        transform.position = new Vector3(startPos.x, startPos.y - dropAmount, 0f);
        sinTimer = 0f;
        hasIdled = false;
        stationState = "unpress";
    }

    void Update()
    {
        switch (stationState)
        {
            case "idle":
                {
                    float sinOffset = Mathf.Sin(sinTimer * waveFrequency) * waveAmplitude;
                    transform.position = new Vector3(startPos.x, startPos.y + sinOffset, 0f);
                    sinTimer += Time.deltaTime;
                    break;
                }
            case "depress":
                {
                    float newY = transform.position.y - dropSpeed * Time.deltaTime;
                    if (newY < startPos.y-dropAmount)
                    {
                        newY = startPos.y - dropAmount;
                    }
                    transform.position = new Vector3(startPos.x, newY, 0f);
                    if (newY == startPos.y - dropAmount)
                    {
                        if (hasIdled)
                        {
                            SaveGame();
                            hasIdled= false;
                        }
                        stationState = "wait";
                    }
                    break;
                }
            case "unpress":
                {
                    float newY = transform.position.y + dropSpeed * Time.deltaTime;
                    if (newY > startPos.y)
                    {
                        newY = startPos.y;
                    }
                    transform.position = new Vector3(startPos.x, newY, 0f);

                    if( transform.position.y == startPos.y )
                    {
                        sinTimer = 0f;
                        stationState = "idle";
                        hasIdled= true;
                    }
                    break;
                }
            case "wait":
                {
                    break;
                }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            stationState = "depress";
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            stationState = "unpress";
        }
    }

    private void SaveGame()
    {
        FindObjectOfType<UIController>().DisplayMessage("The game data has been saved.");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerPrefs.SetFloat("Xpos", player.transform.position.x);
        PlayerPrefs.SetFloat("Ypos", player.transform.position.y);
        PlayerPrefs.SetString("Room", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat("Health", player.GetComponent<PlayerHealth>().GetHealth());
        PlayerPrefs.SetFloat("MaxHealth", player.GetComponent<PlayerHealth>().GetMaxHealth());
        PlayerPrefs.SetInt("Missiles", player.GetComponent<PlayerCombat>().GetMissiles());
        PlayerPrefs.SetInt("MaxMissiles", player.GetComponent<PlayerCombat>().GetMaxMissiles());
    }
}