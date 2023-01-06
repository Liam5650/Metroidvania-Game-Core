using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    void Start()
    {
        timeCharged = 0f;
        charging = false;
        chargedEffect = Instantiate(chargedEffect, shootPoint.position, Quaternion.identity);
        chargedEffect.transform.parent = shootPoint.transform;
    }

    void Update()
    {
        if (Time.timeScale > 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                GameObject shot = Instantiate(beam, shootPoint.position, Quaternion.identity);
                shot.GetComponent<Beam>().Fire(gameObject.transform.localScale.x);
                DontDestroyOnLoad(shot);
                charging = true;
            }

            if (Input.GetButtonUp("Fire1"))
            {
                if (timeCharged >= chargeTime)
                {
                    GameObject shot = Instantiate(chargedBeam, shootPoint.position, Quaternion.identity);
                    shot.GetComponent<Beam>().Fire(gameObject.transform.localScale.x);
                    DontDestroyOnLoad(shot);
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
                    chargedEffect.gameObject.SetActive(true);
                }
            }
        }
        else if (Time.timeScale == 0f && Input.GetButtonUp("Fire1"))
        {
            if (timeCharged > chargeTime)
            {
                GameObject shot = Instantiate(chargedBeam, shootPoint.position, Quaternion.identity);
                shot.GetComponent<Beam>().Fire(gameObject.transform.localScale.x);
                DontDestroyOnLoad(shot);
            }
            timeCharged = 0f;
            charging = false;
            chargedEffect.gameObject.SetActive(false);
        }
    }
}