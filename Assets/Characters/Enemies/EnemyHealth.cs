using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float health;                  // Enemy total health
    [SerializeField] Material flashMaterial;        // Enemy material for flashing hit effect
    [SerializeField] float flashDuration;           // Duration of flash hit effect
    [SerializeField] GameObject deathEffect;        // Particle effect for enemy death
    private SpriteRenderer spriteRenderer;          // Used to change the sprite material between default and flash
    private Material defaultMaterial;               // Default sprite material
    private Coroutine flashRoutine;                 // Coroutine for flashing effect after taking damage
    [SerializeField] List<GameObject> pickups = new List<GameObject>();    // Types of pickups the enemy can drop upon death
    [SerializeField] int dropChance;                // Chance of dropping a pickup on death
    [SerializeField] bool playDeathSound;           // Indicates when to play the enemy death sound
    [SerializeField] private int deathSoundIndex;   // The index of the death sound in the AudioManager enemy sfx list

    void Start()
    {
        // Get the sprite default material
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;
    }

    public void DamageEnemy(float damageAmount)
    {
        // Damage the enemy and enter the flashing effect coroutine
        health -= damageAmount;
        AudioManager.instance.PlaySFX("PlayerCombat", 10);
        if (flashRoutine == null)
        {
            flashRoutine = StartCoroutine(flashCoroutine());
        }
    }

    private IEnumerator flashCoroutine()
    {
        // Start the flashing
        spriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(flashDuration);

        // If the enemy has no health, initiate death, otherwise keep flashing
        if (health <= 0)
        {
            if (playDeathSound == true) AudioManager.instance.PlaySFX("Enemy", deathSoundIndex);
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            DropPickup();
            Destroy(gameObject);
        }
        else
        {
            spriteRenderer.material = defaultMaterial;
            flashRoutine = null;
        }
    }

    private void DropPickup()
    {
        // Randomly drop a pickup
        int numPickups = pickups.Count;
        if (numPickups > 0 && Random.Range(1, 100) > (100 - dropChance))
        {
            // Drop pickup at random index
            int index = Random.Range(0, numPickups);
            Instantiate(pickups[index], transform.position, Quaternion.identity);
        }
    }

    public float GetHealth()
    {
        // Returns the enemy's current health
        return health;
    }
}