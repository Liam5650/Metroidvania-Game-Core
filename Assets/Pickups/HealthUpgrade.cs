using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpgrade : MonoBehaviour
{
    [SerializeField] private int upgradeID;
    private void Awake()
    {
        if (SaveController.instance.playerData.healthUpgrades[upgradeID] == 1) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
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
