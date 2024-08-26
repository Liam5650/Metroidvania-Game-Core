using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;

public class SaveController : MonoBehaviour
{
    public PlayerData playerData;           // Class that stores all information needed about the player and game state
    private string json;                    // The JSON string containing the save data
    public static SaveController instance;  // Create instance so other scripts can easily access player data

    private void Awake()
    {
        // Set up instance
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        // Initialize player data
        if (System.IO.File.Exists(Application.dataPath + "/saveFile.json"))
        {
            json = File.ReadAllText(Application.dataPath + "/saveFile.json");
            playerData = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            playerData = new PlayerData();
        }
    }

    public class PlayerData
    {
        // Basic player information
        public Vector3 playerPosition = new Vector3(27f, 9f, 0f);
        public string roomName = "Room1";
        public float currHealth = 99f, maxHealth = 99f;
        public int currMissiles = 0, maxMissiles = 0;
        public bool doubleJump = false, ball = false, ballBomb = false, chargeBeam = false, missile = false;

        // Map coordinate information
        public Vector3Int[] roomsVisited;

        // Event / upgrade reference array where each index is mapped to a single gameobject. A value of 0 means it hasn't been collected, and 1 means has been collected
        public int[] missileUpgrades = new int[10];
        public int[] healthUpgrades = new int[5];
        public int[] abilityUpgrades= new int[10];
        public int[] events = new int[10];
    }

    public void SaveData()
    { 
        // Save the player data to a JSON
        json = JsonUtility.ToJson(playerData);
        File.WriteAllText(Application.dataPath + "/saveFile.json", json);
        UIController.instance.SetContinueButton(true);
    }

    public bool HasSave()
    {
        // Check for save data
        return System.IO.File.Exists(Application.dataPath + "/saveFile.json");
    }

    public void ClearSave()
    {
        // Delete save data and reinitialize
        if (System.IO.File.Exists(Application.dataPath + "/saveFile.json"))
        {
            System.IO.File.Delete(Application.dataPath + "/saveFile.json");
        }
        playerData = new PlayerData();
        UIController.instance.SetContinueButton(false);
    }

    public void LoadSave()
    {
        // Load the player save data
        if (HasSave())
        {
            json = File.ReadAllText(Application.dataPath + "/saveFile.json");
            playerData = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            playerData= new PlayerData();
        }
    }
}