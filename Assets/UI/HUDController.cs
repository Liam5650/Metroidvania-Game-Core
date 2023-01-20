using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthNumber, ammoNumber;
    [SerializeField] GameObject[] energyTanks;
    [SerializeField] GameObject[] ammoObjects;
    [SerializeField] SaveController saveController;

    public void UpdateHealth(float health, float maxHealth)
    {
        int unlockedTanks = (int)maxHealth / 100;
        int filledTanks = (int)health / 100;

        // Enable health bars according to what our max health is
        if (unlockedTanks > 0)
        {
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
        }

        // Set the final digits from 0-99
        healthNumber.text = (health - (filledTanks*100)).ToString();
    }

    public void UpdateAmmo(int value, int max)
    {
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

    private void OnDisable()
    {
        if (saveController.HasSave())
        {
            UpdateHealth(saveController.playerData.currHealth, saveController.playerData.maxHealth);
            UpdateAmmo(saveController.playerData.currMissiles, saveController.playerData.maxMissiles);
        }
    }
}