using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    [SerializeField] bool doubleJump, ball, ballBomb, chargeBeam, missile;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Unlocks unlocked = FindObjectOfType<Unlocks>(); 

            if (doubleJump)
            {
                unlocked.UnlockDoubleJump();
            }
            if (ball)
            {
                unlocked.UnlockBall();
            }
            if (missile)
            {
                unlocked.UnlockMissile();
            }
            if (ballBomb)
            {
                unlocked.UnlockBallBomb();
            }
            if (chargeBeam)
            {
                unlocked.UnlockChargeBeam();
            }
            Destroy(gameObject);
        }
    }
}
