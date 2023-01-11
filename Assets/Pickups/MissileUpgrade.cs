using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileUpgrade : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerCombat>().UpgradeMissiles();
            FindObjectOfType<UIController>().UpgradeMessage("Missile capacity upgraded.");
            Destroy(gameObject);
        }
    }
}
