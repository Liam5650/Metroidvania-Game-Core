using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
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
    private PlayerMovement player;
    private HUDController HUD;

    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;
        player = FindObjectOfType<PlayerMovement>();
        HUD = FindObjectOfType<HUDController>();
        HUD.UpdateHealth(health);
    }

    public void DamagePlayer(float damageAmount)
    {
        if (!invincible)
        {
            health -= damageAmount;
            if (health > 0 && flashRoutine == null)
            {
                flashRoutine = StartCoroutine(flashCoroutine());
            }
            else if (health <= 0)
            {
                if (deathEffect!= null)
                {
                    Instantiate(deathEffect, transform.position, Quaternion.identity);
                }
                HUD.UpdateHealth(0f);
                gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator flashCoroutine()
    {
        HUD.UpdateHealth(health);
        invincible = true;
        player.launchPlayer();
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
}