using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    [SerializeField] float beamVelocity;
    [SerializeField] float beamLifetime;
    [SerializeField] float beamDamage;
    [SerializeField] GameObject hitEffect;
    private Rigidbody2D rb;
    [SerializeField] AudioClip sfx;

    public void Fire(float direction)
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(beamVelocity * direction, 0f);
        Destroy(gameObject, beamLifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyHealth>().DamageEnemy(beamDamage);
        }
        Instantiate(hitEffect, transform.position, Quaternion.identity);
        //AudioManager.instance.PlaySFX(sfx);
        Destroy(gameObject);
    }
}