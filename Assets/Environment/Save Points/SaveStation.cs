using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveStation : MonoBehaviour
{
    [SerializeField] float dropSpeed, dropAmount, waveAmplitude, waveFrequency;    // Controls the motion of the top platform
    private string stationState;    // Used for the switch case to change behavior
    private Vector3 startPos;       // Start pos of the platform
    private float sinTimer;         // Timer used to keep track of the point on the sin wave we are on
    private bool hasIdled;          // Only allows the platform to trigger a save again if it has reached the idle state
    private GameObject player;      // Used to make sure the platform only depresses if the player is on it

    void Start()
    {
        // Set up initial state
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
                    // Move the platform using a sin wave
                    float sinOffset = Mathf.Sin(sinTimer * waveFrequency) * waveAmplitude;
                    transform.position = new Vector3(startPos.x, startPos.y + sinOffset, 0f);
                    sinTimer += Time.deltaTime;
                    break;
                }
            case "depress":
                {
                    // Depress the platform
                    float newY = transform.position.y - dropSpeed * Time.deltaTime;
                    if (newY < startPos.y-dropAmount)
                    {
                        newY = startPos.y - dropAmount;
                    }
                    transform.position = new Vector3(startPos.x, newY, 0f);

                    // Save the game state if the platform fully depresses
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
                    // Return the platform to the idle position
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
                    // Player is standing on the fully depressed platform, so do nothing
                    break;
                }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Depress the platform if the player is standing on it 
        if (other.gameObject.tag == "Player")
        {
            stationState = "depress";
            if (player == null)
            {
                player = other.gameObject;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Unpress the platform when the player leaves it
        if (other.gameObject.tag == "Player")
        {
            stationState = "unpress";
        }
    }

    private void SaveGame()
    {
        // Get all components that need to be saved and update them in the player save data
        AudioManager.instance.PlaySFX("UI", 4);
        SaveController.instance.playerData.playerPosition = player.transform.position;
        SaveController.instance.playerData.roomName = SceneManager.GetActiveScene().name;
        SaveController.instance.playerData.currHealth = player.GetComponent<PlayerHealth>().GetHealth();
        SaveController.instance.playerData.maxHealth = player.GetComponent<PlayerHealth>().GetMaxHealth();
        SaveController.instance.playerData.currMissiles = player.GetComponent<PlayerCombat>().GetMissiles();
        SaveController.instance.playerData.maxMissiles = player.GetComponent<PlayerCombat>().GetMaxMissiles();
        SaveController.instance.playerData.doubleJump = player.GetComponent<Unlocks>().DoubleJump();
        SaveController.instance.playerData.ball = player.GetComponent<Unlocks>().Ball();
        SaveController.instance.playerData.ballBomb = player.GetComponent<Unlocks>().BallBomb();
        SaveController.instance.playerData.chargeBeam = player.GetComponent<Unlocks>().ChargeBeam();
        SaveController.instance.playerData.missile = player.GetComponent<Unlocks>().Missile();
        SaveController.instance.playerData.roomsVisited = MapController.instance.SaveMap();

        // Save the data
        SaveController.instance.SaveData();
        UIController.instance.DisplayMessage("The game data has been saved.");
    }
}