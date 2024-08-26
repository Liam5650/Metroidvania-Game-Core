using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileUpgrade : MonoBehaviour
{
    [SerializeField] private int upgradeID;    // ID is used by the save controller to determine if the upgrade has already been collected

    private void Awake()
    {
        // Destroy upgrade if previously collected
        if (SaveController.instance.playerData.missileUpgrades[upgradeID] == 1) Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Increase player total missile count if collected
        if (other.tag == "Player")
        {
            SaveController.instance.playerData.missileUpgrades[upgradeID] = 1;
            AudioManager.instance.PlaySFX("Pickup", 1);
            other.gameObject.GetComponent<PlayerCombat>().UpgradeMissiles();
            FindObjectOfType<UIController>().DisplayMessage("Missile capacity upgraded by 5.");
            Destroy(gameObject);
        }
    }
}
