using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] GameObject beam;
    [SerializeField] GameObject chargedBeam;
    [SerializeField] GameObject chargedEffect;
    [SerializeField] Transform shootPoint;
    [SerializeField] float chargeTime;
    private float timeCharged;
    private bool charging;

    [SerializeField] GameObject bomb;
    [SerializeField] Transform bombDropPoint;

    [SerializeField] GameObject missile;
    [SerializeField] int maxMissiles, currMissiles;
    [SerializeField] HUDController HUD;
    private Unlocks unlocked;

    [SerializeField] SaveController saveController;

    private bool chargeSFXPlayed;
    private float cooldown;

    void Start()
    {
        timeCharged = 0f;
        charging = false;
        chargedEffect = Instantiate(chargedEffect, shootPoint.position, Quaternion.identity);
        chargedEffect.transform.parent = shootPoint.transform;

        if (saveController.HasSave())
        {
            currMissiles = saveController.playerData.currMissiles;
            maxMissiles = saveController.playerData.maxMissiles;
        }
        HUD.UpdateAmmo(currMissiles, maxMissiles);
        unlocked = GetComponent<Unlocks>();
    }

    void Update()
    {
        // Handle standing state behaviour
        if (gameObject.GetComponent<PlayerMovement>().IsStanding())
        {
            if (Time.timeScale > 0f)
            {
                cooldown += Time.deltaTime;

                if (Input.GetButtonDown("Fire1"))
                {
                    if (cooldown> 0.2f)
                    {
                        Instantiate(beam, shootPoint.position, Quaternion.identity).GetComponent<Beam>().Fire(gameObject.transform.localScale.x);
                        AudioManager.instance.PlaySFX(0);
                    }
                    if (unlocked.ChargeBeam())
                    {
                        charging = true;
                        chargeSFXPlayed = false;
                    }
                }

                if (Input.GetButtonUp("Fire1"))
                {
                    if (timeCharged >= chargeTime)
                    {
                        Instantiate(chargedBeam, shootPoint.position, Quaternion.identity).GetComponent<Beam>().Fire(gameObject.transform.localScale.x);
                        AudioManager.instance.PlaySFX(1);
                        cooldown = 0f;
                    }
                    timeCharged = 0f;
                    charging = false;
                    chargedEffect.gameObject.SetActive(false);
                }

                if (charging)
                {
                    timeCharged += Time.deltaTime;
                    if (timeCharged >= 0.35f)
                    {
                        if (!chargeSFXPlayed) AudioManager.instance.PlaySFX(2); chargeSFXPlayed=true;
                        if (timeCharged > chargeTime) timeCharged = chargeTime;
                        chargedEffect.gameObject.SetActive(true);
                        float chargePercent = (timeCharged-0.35f)/(chargeTime - 0.35f);
                        chargedEffect.gameObject.transform.localScale = new Vector3(1f * chargePercent, 1f * chargePercent, 1f);
                    }
                }
                else if (Input.GetButtonDown("Fire2") && currMissiles > 0 && unlocked.Missile())
                {
                    Instantiate(missile, shootPoint.position, Quaternion.identity).GetComponent<Missile>().Fire(gameObject.transform.localScale.x);
                    currMissiles -= 1;
                    HUD.UpdateAmmo(currMissiles, maxMissiles);
                }
            }
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
            // Fire off a shot if it is charged
            if (timeCharged >= chargeTime)
            {
                Instantiate(chargedBeam, shootPoint.position, Quaternion.identity).GetComponent<Beam>().Fire(gameObject.transform.localScale.x);
                AudioManager.instance.PlaySFX(1);
            }
            timeCharged = 0f;
            charging = false;
            chargedEffect.gameObject.SetActive(false);

            if (Time.timeScale > 0f && Input.GetButtonDown("Fire1") && unlocked.BallBomb())
            {
                Instantiate(bomb, bombDropPoint.position, Quaternion.identity).gameObject.GetComponent<Bomb>().Drop();
            }
        }
    }
    public void RechargeAmmo(int rechargeAmount)
    {
        currMissiles += rechargeAmount;
        if (currMissiles > maxMissiles)
        {
            currMissiles = maxMissiles;
        }
        HUD.UpdateAmmo(currMissiles, maxMissiles);
    }

    public void UpgradeMissiles()
    {
        maxMissiles += 5;
        currMissiles = maxMissiles;
        HUD.UpdateAmmo(currMissiles, maxMissiles);
    }

    public int GetMissiles()
    { 
        return currMissiles; 
    }

    public int GetMaxMissiles()
    {
        return maxMissiles;
    }

    private void OnDisable()
    {
        // Revert changes since last save
        if (saveController.HasSave())
        {
            currMissiles = saveController.playerData.currMissiles;
            maxMissiles = saveController.playerData.maxMissiles;
        }
    }
}