using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] float startVelocity;
    [SerializeField] float maxVelocity;
    [SerializeField] float acceleration;
    [SerializeField] float missileLifetime;
    [SerializeField] float missileDamage;
    [SerializeField] GameObject hitEffect;
    private Rigidbody2D rb;
    private float currentVelocity;
    private float fireDirection;

    void Update()
    {
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
        fireDirection = direction;
        gameObject.transform.localScale = new Vector3(fireDirection, 1f, 1f);
        rb = gameObject.GetComponent<Rigidbody2D>();
        currentVelocity = startVelocity;
        rb.velocity = new Vector2(currentVelocity * fireDirection, 0f);
        Destroy(gameObject, missileLifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyHealth>().DamageEnemy(missileDamage);
        }
        if (hitEffect!= null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
