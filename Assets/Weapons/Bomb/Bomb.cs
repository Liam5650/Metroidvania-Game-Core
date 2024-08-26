using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] float detonateTime;        // Amount of time after placing before detonation
    [SerializeField] float damage;              // Amount of damage to inflict to enemies
    [SerializeField] float explodeRadius;       // Radius that the explosion influences
    [SerializeField] GameObject explodeEffect;  // Particle effect to show the explosion

    public void Drop()
    {   
        // Starts the detonation timer
        Destroy(gameObject, detonateTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Detonate early if an enemy hits the bomb
        if (other.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Initiate explosion process
        AudioManager.instance.PlaySFX("PlayerCombat", 9);
        if (explodeEffect!= null)
        {
            Instantiate(explodeEffect, transform.position, Quaternion.identity);
        }

        // Check all overlapped colliders and interact accordingly
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explodeRadius);
        foreach(Collider2D collider in colliders)
        {
            if (collider.gameObject.tag == "Enemy")
            {
                collider.gameObject.GetComponent<EnemyHealth>().DamageEnemy(damage);
            }
            else if (collider.gameObject.tag == "Destructible")
            {
                Destroy(collider.gameObject);
            }
            else if (collider.gameObject.tag == "Player")
            {
                PlayerCombat.instance.gameObject.GetComponent<PlayerMovement>().BombImpulse();
            }
        }

        // Refresh the number of bombs that are active so the player can place more
        PlayerCombat.instance.DecrementBomb();
    }
}
