using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float maxHealth;                   // Max player health
    [SerializeField] float health;                      // Current player health
    [SerializeField] Material flashMaterial;            // Used for player taking damage effect
    [SerializeField] float flashDuration;               // Amount of time a single flash takes when taking damage
    [SerializeField] float invincibileTime;             // Amount of time the player is invincible after taking damage
    [SerializeField] float invincibleFlashDuration;     // Total amount of time spent flashing after taking damage
    private bool invincible;                            // Keep track whether or not the player is invincible
    [SerializeField] GameObject deathEffect;            // Player death effect
    private SpriteRenderer spriteRenderer;              // Player sprite renderer to apply flash material
    private Material defaultMaterial;                   // Default player sprite material
    private Coroutine flashRoutine;                     // The coroutine ran when taking damage
    [SerializeField] HUDController HUD;                 // Used to update health values on HUD
    [SerializeField] SaveController saveController;     // Used to save health values to player save data

    void Awake()
    {
        // Get default material
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;
    }

    public void DamagePlayer(float damageAmount, Vector3 hitPosition)
    {
        // Damage the player if they arent invincible from taking damage
        if (!invincible)
        {
            // Start flash coroutine if the player still has health
            health -= damageAmount;
            if (health > 0 && flashRoutine == null)
            {
                AudioManager.instance.PlaySFX("PlayerHealth", 0);
                gameObject.GetComponent<PlayerMovement>().launchPlayer(hitPosition);
                flashRoutine = StartCoroutine(flashCoroutine());
            }
            // Start death coroutine if player health drops to 0
            else if (health <= 0)
            {
                HUD.UpdateHealth(0f, maxHealth);
                StartCoroutine(PlayerDeath());
            }
        }
    }

    private IEnumerator flashCoroutine()
    {
        // Update the hud and make the player sprite flash
        HUD.UpdateHealth(health, maxHealth);
        invincible = true;
        spriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.material = defaultMaterial;
        float numInvincibleFlashes = invincibileTime / (invincibleFlashDuration * 2);
        for (int i = 0; i < (int)numInvincibleFlashes; i++)
        {
            spriteRenderer.color = new UnityEngine.Color(1f, 1f, 1f, 0.75f);
            yield return new WaitForSeconds(invincibleFlashDuration);
            spriteRenderer.color = new UnityEngine.Color(1f, 1f, 1f, 0.25f);
            yield return new WaitForSeconds(invincibleFlashDuration);
        }
        spriteRenderer.color = new UnityEngine.Color(1f, 1f, 1f, 1f);
        invincible = false;
        flashRoutine = null;
    }

    public void HealPlayer(float healAmount)
    {
        // Add health to the player and update HUD
        health += healAmount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        HUD.UpdateHealth(health, maxHealth);
    }

    public void UpgradeHealth()
    {
        // Increase max health and update HUD
        maxHealth += 100;
        health = maxHealth;
        HUD.UpdateHealth(health, maxHealth);
    }

    public float GetHealth()
    {
        // Used by save controller when saving player data
        return health;
    }

    public float GetMaxHealth()
    {
        // Used by save controller when saving player data
        return maxHealth;
    }

    public void RefreshState()
    {
        // Used to load default values from saved player data
        spriteRenderer.material = defaultMaterial;
        spriteRenderer.color = new UnityEngine.Color(1f, 1f, 1f, 1);
        invincible = false;
        flashRoutine = null;

        health = saveController.playerData.currHealth;
        maxHealth = saveController.playerData.maxHealth;
    }

    private IEnumerator PlayerDeath()
    {
        // This coroutine handles the player death sequence

        // Make the player appear above the ui blackscreen for room transitions
        SpriteRenderer playerSprite = gameObject.GetComponent<SpriteRenderer>();
        int sortOrder = playerSprite.sortingOrder;
        playerSprite.sortingOrder = 10000;

        // Handle death sequence
        AudioManager.instance.PlaySFX("PlayerHealth", 0);
        UIController.instance.MarkTransition(true);
        StartCoroutine(UIController.instance.FadeTransition("in", 1f));
        AudioManager.instance.FadeOutMusic(1f);
        for (int i = 0; i < 10; i++)
        {
            spriteRenderer.material = flashMaterial;
            yield return new WaitForSecondsRealtime(0.1f);
            spriteRenderer.material = defaultMaterial;
            yield return new WaitForSecondsRealtime(0.1f);
        }
        AudioManager.instance.PlaySFX("PlayerHealth", 1);
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        playerSprite.sortingOrder = sortOrder;
        yield return new WaitForSecondsRealtime(2f);
        UIController.instance.LoadMenu(false);
    }
}