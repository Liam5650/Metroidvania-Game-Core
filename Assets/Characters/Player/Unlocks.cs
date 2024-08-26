using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlocks : MonoBehaviour
{
    /* 
        This class is used to manage the abilities that the player can unlock. Various scripts
        reference this class to check if the player has the ability to perform certain actions,
        such as the PlayerMovement and PlayerCombat scripts.
    */

    [SerializeField] bool doubleJump, ball, ballBomb, chargeBeam, missile;  // Bools that are used to check for abilities
    private SaveController saveController;                                  // Reference to the save controller to refresh ability unlocks

    // The following methods are used to access / set bools
    public bool DoubleJump()
    {
        return doubleJump;
    }

    public bool Ball()
    {
        return ball;
    }

    public bool BallBomb()
    {
        return ballBomb;
    }

    public bool ChargeBeam()
    {
        return chargeBeam;
    }

    public bool Missile()
    {
        return missile;
    }

    public void UnlockDoubleJump()
    {
        doubleJump = true;
    }

    public void UnlockBall()
    {
        ball = true;
    }

    public void UnlockBallBomb()
    {
        ballBomb = true;
    }

    public void UnlockChargeBeam()
    {
        chargeBeam = true;
    }

    public void UnlockMissile()
    {
        missile = true;
    }

    public void RefreshState()
    {
        // Refresh ability unlocks to the state that is stored in the save controller's playerData class
        saveController = SaveController.instance;
        doubleJump = saveController.playerData.doubleJump;
        ball = saveController.playerData.ball;
        ballBomb = saveController.playerData.ballBomb;
        chargeBeam = saveController.playerData.chargeBeam;
        missile = saveController.playerData.missile;
    }
}