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
    private PlayerMovement player;

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(beamVelocity * player.transform.localScale.x, 0f);
        Destroy(gameObject, beamLifetime);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyHealth>().DamageEnemy(beamDamage);
        }
        Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}