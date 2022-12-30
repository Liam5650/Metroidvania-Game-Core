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

    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;
        player = FindObjectOfType<PlayerMovement>();
    }

    public void DamagePlayer(float damageAmount)
    {
        if (!invincible)
        {
            player.launchPlayer();
            invincible = true;
            health -= damageAmount;
            if (flashRoutine == null)
            {
                flashRoutine = StartCoroutine(flashCoroutine());
            }
        }
    }

    private IEnumerator flashCoroutine()
    {
        spriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(flashDuration);
        if (health <= 0)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else
        {   
            spriteRenderer.material = defaultMaterial;
            yield return new WaitForSeconds(invincibleFlashDuration);
            float numInvincibleFlashes = invincibileTime / (invincibleFlashDuration * 2);
            for (int i = 0; i < (int)numInvincibleFlashes; i++)
            {
                spriteRenderer.color = new UnityEngine.Color(1f, 1f, 1f, 0f);
                yield return new WaitForSeconds(invincibleFlashDuration);
                spriteRenderer.color = new UnityEngine.Color(1f, 1f, 1f, 1f);
                yield return new WaitForSeconds(invincibleFlashDuration);
            }
            invincible = false;
            flashRoutine = null;
        }
    }
}