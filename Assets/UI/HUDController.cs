using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthNumber, ammoNumber;  // Reference to the current player health and missile count
    [SerializeField] GameObject[] energyTanks;                  // References the energy tank objects to set active or inactive
    [SerializeField] GameObject[] ammoObjects;                  // References the ammo objects to set active or inactive
    [SerializeField] SaveController saveController;             // Used to access save data

    public void UpdateHealth(float health, float maxHealth)
    {
        // Calculate the amount of tanks unlocked, and amount that are filled from the player data
        int unlockedTanks = (int)maxHealth / 100;
        int filledTanks = (int)health / 100;

        // Enable health bars according to what our max health is
        for (int i = 0; i < energyTanks.Length; i += 1)
        {
            if (i < unlockedTanks)
            {
                energyTanks[i].SetActive(true);

                if (i < filledTanks)
                {
                    energyTanks[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                }
                else
                {
                    energyTanks[i].GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
                }
            }
            else
            {
                energyTanks[i].SetActive(false);
            }
        }

        // Set the final digits from 0-99
        healthNumber.text = (health - (filledTanks*100)).ToString();
    }

    public void UpdateAmmo(int value, int max)
    {
        // Enable the ammo text and update the value accordingly
        if (max > 0)
        {
            foreach(GameObject ammoObject in ammoObjects) 
            {
                ammoObject.SetActive(true);
                ammoNumber.text = value.ToString();
            }
        }
        else
        {
            foreach (GameObject ammoObject in ammoObjects)
            {
                ammoObject.SetActive(false);
            }
        }
    }

    public void RefreshState()
    {
        // Used to refresh the HUD text depending on the player data after loading a save, quiting to menu, player death etc.
        UpdateHealth(saveController.playerData.currHealth, saveController.playerData.maxHealth);
        UpdateAmmo(saveController.playerData.currMissiles, saveController.playerData.maxMissiles);
    }
}