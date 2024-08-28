using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;

public class SaveController : MonoBehaviour
{
    public PlayerData playerData;                           // Class that stores all information needed about the player and game state
    private string json;                                    // The JSON string containing the save data
    public static SaveController instance;                  // Create instance so other scripts can easily access player data
    [SerializeField] private bool usePlayerPrefs = true;    // Used to toggle between saving player data as a JSON string in PlayerPrefs, vs as a file

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
        if (!usePlayerPrefs)
        {
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
        else
        {
            if (PlayerPrefs.HasKey("Data"))
            {
                json = PlayerPrefs.GetString("Data");
            }
            else
            {
                playerData = new PlayerData();
            }
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
        if (!usePlayerPrefs) File.WriteAllText(Application.dataPath + "/saveFile.json", json);
        else PlayerPrefs.SetString("Data", json);
        UIController.instance.SetContinueButton(true);
    }

    public bool HasSave()
    {
        // Check for save data
        if (!usePlayerPrefs) return System.IO.File.Exists(Application.dataPath + "/saveFile.json");
        else return PlayerPrefs.HasKey("Data");
    }

    public void ClearSave()
    {
        // Delete save data and reinitialize
        if (!usePlayerPrefs && System.IO.File.Exists(Application.dataPath + "/saveFile.json")) System.IO.File.Delete(Application.dataPath + "/saveFile.json");
        else if (PlayerPrefs.HasKey("Data")) PlayerPrefs.DeleteKey("Data");

        playerData = new PlayerData();
        UIController.instance.SetContinueButton(false);
    }

    public void LoadSave()
    {
        // Load the player save data
        if (HasSave())
        {
            if (!usePlayerPrefs) json = File.ReadAllText(Application.dataPath + "/saveFile.json");
            else json = PlayerPrefs.GetString("Data");
            playerData = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            playerData= new PlayerData();
        }
    }
}