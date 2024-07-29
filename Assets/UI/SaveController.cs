using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;

public class SaveController : MonoBehaviour
{
    public PlayerData playerData;
    private string json;
    public static SaveController instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    void Start()
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

    public class PlayerData
    {
        public Vector3 playerPosition = new Vector3(27f, 9f, 0f);
        public string roomName = "Room1";
        public float currHealth = 99f, maxHealth = 99f;
        public int currMissiles = 0, maxMissiles = 0;
        public bool doubleJump = false, ball = false, ballBomb = false, chargeBeam = false, missile = false;
        public Vector3Int[] roomsVisited;

        // Event/upgrade reference array. Each index is mapped to a single gameobject, and changed to the value 1 if the gameobject hitbox has been triggered
        public int[] missileUpgrades = new int[10];
        public int[] healthUpgrades = new int[5];
        public int[] abilityUpgrades= new int[10];
        public int[] events = new int[10];
    }

    public void SaveData()
    { 
        json = JsonUtility.ToJson(playerData);
        File.WriteAllText(Application.dataPath + "/saveFile.json", json);
        UIController.instance.SetContinueButton(true);
    }

    public bool HasSave()
    {
        return System.IO.File.Exists(Application.dataPath + "/saveFile.json");
    }

    public void ClearSave()
    {
        if (System.IO.File.Exists(Application.dataPath + "/saveFile.json"))
        {
            System.IO.File.Delete(Application.dataPath + "/saveFile.json");
        }
        playerData = new PlayerData();
        UIController.instance.SetContinueButton(false);
    }

    public void LoadSave()
    {
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