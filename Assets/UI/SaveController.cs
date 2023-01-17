using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveController : MonoBehaviour
{
    public PlayerData playerData;
    private string json;

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
        public Vector3 playerPosition = Vector3.zero;
        public string roomName = "Room1";
        public float currHealth = 99f, maxHealth = 99f;
        public int currMissiles = 0, maxMissiles = 0;
        public bool doubleJump = false, ball = false, ballBomb = false, chargeBeam = false, missile = false;
        //public int[] roomsVisited;
    }

    public void SaveData()
    { 
        json = JsonUtility.ToJson(playerData);
        File.WriteAllText(Application.dataPath + "/saveFile.json", json);
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
    }
}