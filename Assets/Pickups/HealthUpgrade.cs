using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpgrade : MonoBehaviour
{
    [SerializeField] private int upgradeID;    // ID is used by the save controller to determine if the upgrade has already been collected

    private void Awake()
    {
        // Destroy upgrade if previously collected
        if (SaveController.instance.playerData.healthUpgrades[upgradeID] == 1) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Increase player total health if collected
        if (other.tag == "Player")
        {
            SaveController.instance.playerData.healthUpgrades[upgradeID] = 1;
            AudioManager.instance.PlaySFX("Pickup", 1);
            other.gameObject.GetComponent<PlayerHealth>().UpgradeHealth();
            FindObjectOfType<UIController>().DisplayMessage("Health capacity upgraded by 100 units.");
            Destroy(gameObject);
        }
    }
}
