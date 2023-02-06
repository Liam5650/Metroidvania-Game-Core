using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpgrade : MonoBehaviour
{    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            AudioManager.instance.PlaySFX("Pickup", 1);
            other.gameObject.GetComponent<PlayerHealth>().UpgradeHealth();
            FindObjectOfType<UIController>().DisplayMessage("Health capacity upgraded by 100 units.");
            Destroy(gameObject);
        }
    }
}
