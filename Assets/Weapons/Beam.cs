using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    [SerializeField] float beamVelocity;
    [SerializeField] float beamLifetime;
    [SerializeField] float beamDamage;
    [SerializeField] GameObject hitEffect;
    private Rigidbody2D rb;
    [SerializeField] private int hitSFXIndex;

    public void Fire(float direction)
    {
        // Set up movement
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
        Instantiate(hitEffect, transform.position, Quaternion.identity);
        AudioManager.instance.PlayAdjustedSFX("PlayerCombat", hitSFXIndex, 0.05f);

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