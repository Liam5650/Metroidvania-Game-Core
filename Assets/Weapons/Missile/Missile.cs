using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] float startVelocity;       // Start velocity of the missile
    [SerializeField] float maxVelocity;         // Max velocity the missile can reach
    [SerializeField] float acceleration;        // How fast the missile accelerates
    [SerializeField] float missileLifetime;     // How long the missile remains active
    [SerializeField] float missileDamage;       // Damage that the missile inflicts to enemies
    [SerializeField] GameObject hitEffect;      // Missile explosion particle effect
    private Rigidbody2D rb;                     // Reference for handling the velocity changes
    private float currentVelocity;              // Current missile velocity
    private float fireDirection;                // Used to align fire direction with player

    void Update()
    {
        // Accelerate to max velocity
        if (currentVelocity < maxVelocity)
        {
            currentVelocity += (acceleration * Time.deltaTime);
            if (currentVelocity > maxVelocity)
            {
                currentVelocity = maxVelocity;
            }
            rb.velocity = new Vector2(currentVelocity * fireDirection, 0f);
        }
    }

    public void Fire(float direction)
    {
        // Orient and fire missile
        fireDirection = direction;
        gameObject.transform.localScale = new Vector3(fireDirection, 1f, 1f);
        rb = gameObject.GetComponent<Rigidbody2D>();
        currentVelocity = startVelocity;
        rb.velocity = new Vector2(currentVelocity * fireDirection, 0f);
        Destroy(gameObject, missileLifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Handle collisions
        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyHealth>().DamageEnemy(missileDamage);
        }
        if (hitEffect!= null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
        AudioManager.instance.PlayAdjustedSFX("PlayerCombat", 6, 0.05f);
        Destroy(gameObject);
    }
}
