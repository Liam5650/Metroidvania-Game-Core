using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileUpgrade : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            AudioManager.instance.PlaySFX("Pickup", 1);
            other.gameObject.GetComponent<PlayerCombat>().UpgradeMissiles();
            FindObjectOfType<UIController>().DisplayMessage("Missile capacity upgraded by 5.");
            Destroy(gameObject);
        }
    }
}
