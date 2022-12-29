using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] GameObject beam;
    [SerializeField] GameObject chargedBeam;
    [SerializeField] Transform shootPoint;
    [SerializeField] float chargeTime;
    private float timeCharged;
    private bool charging;

    void Start()
    {
        timeCharged = 0f;
        charging = false;
    }

    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Instantiate(beam, shootPoint.position, Quaternion.identity);
            charging = true;
        }

        if(Input.GetButtonUp("Fire1"))
        {
            if(timeCharged >= chargeTime)
            {
                Instantiate(chargedBeam, shootPoint.position, Quaternion.identity);

            }
            timeCharged = 0f;
            charging = false;
        }

        if(charging)
        {
            timeCharged += Time.deltaTime;
        }
    }
}
