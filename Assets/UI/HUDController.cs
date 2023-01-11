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

    public void UpdateHealth(float health, float maxHealth = 99)
    {
        // Enable health bars according to what our max health is
        if (maxHealth > 99)
        {
            for (int i = 0; i < ((int)maxHealth/100); i += 1)
            {
                if (i < energyTanks.Length)
                {
                    energyTanks[i].SetActive(true);
                }
            }
        }

        // Fill or empty available tanks depending on what our health is vs max health
        int numTanks = (int)(health / 100);
        for (int i = 0; i < energyTanks.Length; i++)
        {
            if (i < numTanks)
            {
                energyTanks[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                energyTanks[i].GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
            }
        }

        // Set the final digits from 0-99
        healthNumber.text = (health - (numTanks*100)).ToString();
    }

    public void UpdateAmmo(int value, int max = 0)
    {
        if (max > 0)
        {
            foreach(GameObject ammoObject in ammoObjects) 
            {
                ammoObject.SetActive(true);
            }
        }
        ammoNumber.text = value.ToString();
    }
}
