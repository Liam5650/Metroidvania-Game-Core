using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlocks : MonoBehaviour
{
    [SerializeField] bool doubleJump, ball, ballBomb, chargeBeam, missile;
    private SaveController saveController;

    private void Start()
    {
        saveController = SaveController.instance;
        if (saveController.HasSave())
        {
            doubleJump = saveController.playerData.doubleJump;
            ball = saveController.playerData.ball;
            ballBomb = saveController.playerData.ballBomb;
            chargeBeam= saveController.playerData.chargeBeam;
            missile = saveController.playerData.missile;
        }
    }

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

    private void OnDisable()
    {
        // Reset upgrades since last save
        if (saveController.HasSave())
        {
            doubleJump = saveController.playerData.doubleJump;
            ball = saveController.playerData.ball;
            ballBomb = saveController.playerData.ballBomb;
            chargeBeam = saveController.playerData.chargeBeam;
            missile = saveController.playerData.missile;
        }
    }
}