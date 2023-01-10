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
    [SerializeField] List<GameObject> pickups = new List<GameObject>();
    [SerializeField] int dropChance;

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
        int numPickups = pickups.Count;
        if (numPickups > 0 && Random.Range(1, 100) > (100 - dropChance))
        {
            // Drop pickup at random index
            int index = Random.Range(0, numPickups);
            Instantiate(pickups[index], transform.position, Quaternion.identity);
        }
    }
}
