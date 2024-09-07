using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    [SerializeField] float beamVelocity;        // The velocity of the projectile
    [SerializeField] float beamLifetime;        // The amount of time the beam remains active
    [SerializeField] float beamDamage;          // How much damage the beam inflicts
    [SerializeField] GameObject hitEffect;      // The effect spawned when the beam hits something
    private Rigidbody2D rb;                     // Used to control the velocity of the beam
    [SerializeField] private int hitSFXIndex;   // SFX to be played upon hit
    private bool hitEffectPlayed = false;       // Make sure the hit effect is only played once if it collides with two objects at the same time

    public void Fire(float direction)
    {
        // Set up movement with direction
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(beamVelocity * direction, 0f);
        Destroy(gameObject, beamLifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Set up collider interactions
        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyHealth>().DamageEnemy(beamDamage);
        }

        if (!hitEffectPlayed)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
            AudioManager.instance.PlayAdjustedSFX("PlayerCombat", hitSFXIndex, 0.05f);
            hitEffectPlayed = true;
        }

        // Detach the particle system trail effect if the beam has one, so we can destroy after the particles have dissappeared
        if (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            transform.DetachChildren();
            var emission = child.GetComponent<ParticleSystem>().emission;
            emission.rateOverTime = 0f;
            Destroy(child.gameObject, 3f);

        }

        Destroy(gameObject);
    }
}