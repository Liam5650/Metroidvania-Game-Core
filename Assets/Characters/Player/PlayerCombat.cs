using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] GameObject beam;                   // Beam weapon projectile
    [SerializeField] GameObject chargedBeam;            // Charge beam weapon projectile   
    [SerializeField] GameObject chargedEffect;          // Charge beam particle effect to show when charged
    [SerializeField] Transform shootPoint;              // Point to spawn projectiles
    [SerializeField] float chargeTime;                  // Time needed to hold the shoot button to charge a shot
    private float timeCharged;                          // Amount of time the shoot button has currently been held
    private bool charging;                              // Initialize the charging sequence
    [SerializeField] GameObject bomb;                   // Bomb weapon gameobject
    [SerializeField] Transform bombDropPoint;           // Position bombs are dropped
    [SerializeField] GameObject missile;                // Missile weapon projectile
    [SerializeField] int maxMissiles, currMissiles;     // Reference for current missile inventory state
    [SerializeField] HUDController HUD;                 // Used to update HUD to reflect current inventory state
    private Unlocks unlocked;                           // Used to check what abilites are unlocked
    [SerializeField] SaveController saveController;     // Used to refresh player data to reflect saved data
    private bool chargeSFXPlayed;                       // Checks if we need to play the charge sfx or if it has already been played
    private float cooldown;                             // Amount of time after firing a charge shot before player can shoot again
    public static PlayerCombat instance;                // Used so other scripts can access such as upgrade scripts
    private int maxBombs = 3;                           // Max amount of bombs the player can have actively placed
    private int currBombs = 0;                          // Current number of active bombs

    void Awake()
    {
        // Set up instance
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        // Set up initial state
        timeCharged = 0f;
        charging = false;
        chargedEffect = Instantiate(chargedEffect, shootPoint.position, Quaternion.identity);
        chargedEffect.transform.parent = shootPoint.transform;
        unlocked = GetComponent<Unlocks>();
    }

    void Update()
    {
        // Handle standing state behaviour
        if (gameObject.GetComponent<PlayerMovement>().IsStanding())
        {
            if (Time.timeScale > 0f)
            {
                // Start incrementing cooldown to see if we can shoot again after charged shot
                cooldown += Time.deltaTime;

                // Handle reglar shooting and initiating charging
                if (Input.GetButtonDown("Fire1"))
                {
                    if (cooldown> 0.2f)
                    {
                        Instantiate(beam, shootPoint.position, Quaternion.identity).GetComponent<Beam>().Fire(gameObject.transform.localScale.x);
                        AudioManager.instance.PlaySFX("PlayerCombat", 0);
                    }
                    if (unlocked.ChargeBeam())
                    {
                        charging = true;
                        chargeSFXPlayed = false;
                    }
                }

                // Handle charge shot
                if (Input.GetButtonUp("Fire1"))
                {
                    if (timeCharged >= chargeTime)
                    {
                        Instantiate(chargedBeam, shootPoint.position, Quaternion.identity).GetComponent<Beam>().Fire(gameObject.transform.localScale.x);
                        AudioManager.instance.PlaySFX("PlayerCombat", 2);
                        cooldown = 0f;
                    }
                    timeCharged = 0f;
                    charging = false;
                    chargedEffect.gameObject.SetActive(false);
                }

                // Handle effects if we are currently charging
                if (charging)
                {
                    timeCharged += Time.deltaTime;
                    if (timeCharged >= 0.35f)
                    {
                        if (!chargeSFXPlayed) AudioManager.instance.PlaySFX("PlayerCombat", 4); chargeSFXPlayed=true;
                        if (timeCharged > chargeTime) timeCharged = chargeTime;
                        chargedEffect.gameObject.SetActive(true);
                        float chargePercent = (timeCharged-0.35f)/(chargeTime - 0.35f);
                        chargedEffect.gameObject.transform.localScale = new Vector3(1f * chargePercent, 1f * chargePercent, 1f);
                    }
                }

                // Handle firing a missile if we aren't charging
                else if (Input.GetButtonDown("Fire2") && currMissiles > 0 && unlocked.Missile())
                {
                    AudioManager.instance.PlaySFX("PlayerCombat", 5);
                    Instantiate(missile, shootPoint.position, Quaternion.identity).GetComponent<Missile>().Fire(gameObject.transform.localScale.x);
                    currMissiles -= 1;
                    HUD.UpdateAmmo(currMissiles, maxMissiles);
                }
            }

            // Handle if a charge shot is released during scene transitions
            else if (Time.timeScale == 0f && Input.GetButtonUp("Fire1"))
            {
                if (timeCharged > chargeTime)
                {
                    GameObject shot = Instantiate(chargedBeam, shootPoint.position, Quaternion.identity);
                    shot.GetComponent<Beam>().Fire(gameObject.transform.localScale.x);
                    DontDestroyOnLoad(shot);
                    cooldown = 0f;
                }
                timeCharged = 0f;
                charging = false;
                chargedEffect.gameObject.SetActive(false);
            }
        }
        // Handle ball state behaviour
        else
        {
            // Fire off a shot if it is charged and reset state
            if (timeCharged >= chargeTime)
            {
                Instantiate(chargedBeam, shootPoint.position, Quaternion.identity).GetComponent<Beam>().Fire(gameObject.transform.localScale.x);
                AudioManager.instance.PlaySFX("PlayerCombat", 2);
            }
            timeCharged = 0f;
            charging = false;
            chargedEffect.gameObject.SetActive(false);

            // Handle dropping bomb
            if (Time.timeScale > 0f && Input.GetButtonDown("Fire1") && unlocked.BallBomb() && (currBombs < maxBombs))
            {
                Instantiate(bomb, bombDropPoint.position, Quaternion.identity).gameObject.GetComponent<Bomb>().Drop();
                AudioManager.instance.PlaySFX("PlayerCombat", 8);
                currBombs += 1;
            }
        }
    }

    public void RechargeAmmo(int rechargeAmount)
    {
        // Refill missile amount and update HUD
        currMissiles += rechargeAmount;
        if (currMissiles > maxMissiles)
        {
            currMissiles = maxMissiles;
        }
        HUD.UpdateAmmo(currMissiles, maxMissiles);
    }

    public void UpgradeMissiles()
    {
        // Increase max missiles and update HUD
        maxMissiles += 5;
        currMissiles = maxMissiles;
        HUD.UpdateAmmo(currMissiles, maxMissiles);
    }

    public int GetMissiles()
    { 
        // Used by save controller for saving player data
        return currMissiles; 
    }

    public int GetMaxMissiles()
    {
        // Used by save controller for saving player data
        return maxMissiles;
    }


    public void RefreshState()
    {
        // Refresh values to be consistent with save data
        currMissiles = saveController.playerData.currMissiles;
        maxMissiles = saveController.playerData.maxMissiles;
    }

    public void DecrementBomb()
    {
        // Used when a bomb detonates, so we can place more
        currBombs--;
    }
}