using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float maxHealth;
    [SerializeField] float health;
    [SerializeField] Material flashMaterial;
    [SerializeField] float flashDuration;
    [SerializeField] float invincibileTime;
    [SerializeField] float invincibleFlashDuration;
    private bool invincible;
    [SerializeField] GameObject deathEffect;
    private SpriteRenderer spriteRenderer;
    private Material defaultMaterial;
    private Coroutine flashRoutine;
    [SerializeField] HUDController HUD;
    [SerializeField] SaveController saveController;

    void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;
    }

    public void DamagePlayer(float damageAmount, Vector3 hitPosition)
    {
        if (!invincible)
        {
            health -= damageAmount;
            if (health > 0 && flashRoutine == null)
            {
                AudioManager.instance.PlaySFX("PlayerHealth", 0);
                gameObject.GetComponent<PlayerMovement>().launchPlayer(hitPosition);
                flashRoutine = StartCoroutine(flashCoroutine());
            }
            else if (health <= 0)
            {
                AudioManager.instance.PlaySFX("PlayerHealth", 1);
                if (deathEffect!= null)
                {
                    Instantiate(deathEffect, transform.position, Quaternion.identity);
                }
                HUD.UpdateHealth(0f, maxHealth);
                gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator flashCoroutine()
    {
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
        health += healAmount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        HUD.UpdateHealth(health, maxHealth);
    }

    public void UpgradeHealth()
    {
        maxHealth += 100;
        health = maxHealth;
        HUD.UpdateHealth(health, maxHealth);
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public void RefreshState()
    {
        spriteRenderer.material = defaultMaterial;
        spriteRenderer.color = new UnityEngine.Color(1f, 1f, 1f, 1);
        invincible = false;
        flashRoutine = null;

        health = saveController.playerData.currHealth;
        maxHealth = saveController.playerData.maxHealth;
    }
}