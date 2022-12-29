using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float health;
    [SerializeField] Material flashMaterial;
    [SerializeField] float flashDuration;
    [SerializeField] GameObject deathEffect;
    private SpriteRenderer spriteRenderer;
    private Material defaultMaterial;
    private Coroutine flashRoutine;

    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;
    }

    public void DamageEnemy(float damageAmount)
    {
        health -= damageAmount;
        if (flashRoutine == null)
        {
            flashRoutine = StartCoroutine(flashCoroutine());
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
            flashRoutine = null;
        }
    }
}
