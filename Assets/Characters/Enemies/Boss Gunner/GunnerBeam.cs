using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunnerBeam : MonoBehaviour
{
    [SerializeField] float beamVelocity;
    [SerializeField] float beamLifetime;
    [SerializeField] float beamDamage;
    [SerializeField] GameObject hitEffect;
    private Rigidbody2D rb;
    [SerializeField] private int hitSFXIndex;

    public void Fire(float direction)
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(beamVelocity * direction, 0f);
        Destroy(gameObject, beamLifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerHealth>().DamagePlayer(beamDamage, gameObject.transform.position);
        }
        Instantiate(hitEffect, transform.position, Quaternion.identity);
        AudioManager.instance.PlayAdjustedSFX("PlayerCombat", hitSFXIndex, 0.05f);
        Destroy(gameObject);
    }
}