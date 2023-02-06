using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    [SerializeField] private int upgradeID;
    [SerializeField] bool doubleJump, ball, ballBomb, chargeBeam, missile;

    private void Awake()
    {
        if (SaveController.instance.playerData.abilityUpgrades[upgradeID] == 1) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            SaveController.instance.playerData.abilityUpgrades[upgradeID] = 1;
            Unlocks unlocked = other.gameObject.GetComponent<Unlocks>();
            AudioManager.instance.PlaySFX("Pickup", 2);

            if (doubleJump)
            {
                UIController.instance.DisplayMessage("Double Jump unlocked. Press jump in the air to perform an extra jump.", 5f);
                unlocked.UnlockDoubleJump();
            }
            if (ball)
            {
                UIController.instance.DisplayMessage("Ball unlocked. Press down to transform to ball, and up to transform back.", 5f);
                unlocked.UnlockBall();
            }
            if (missile)
            {
                UIController.instance.DisplayMessage("Missiles unlocked. Press Fire2 to fire.", 5f);
                unlocked.UnlockMissile();
                other.gameObject.GetComponent<PlayerCombat>().UpgradeMissiles();
            }
            if (ballBomb)
            {
                UIController.instance.DisplayMessage("Bombs unlocked. Press Fire1 as a ball to drop a bomb.", 5f);
                unlocked.UnlockBallBomb();
            }
            if (chargeBeam)
            {
                UIController.instance.DisplayMessage("Charge Beam unlocked. Hold Fire1 shortly and then release to fire a more powerful shot.", 5f);
                unlocked.UnlockChargeBeam();
            }
            Destroy(gameObject);
        }
    }
}
