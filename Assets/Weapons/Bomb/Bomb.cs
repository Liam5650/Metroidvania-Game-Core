using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] float detonateTime;
    [SerializeField] float damage;
    [SerializeField] float explodeRadius;
    [SerializeField] GameObject explodeEffect;

    public void Drop()
    {
        Destroy(gameObject, detonateTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyHealth>().DamageEnemy(damage);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        AudioManager.instance.PlaySFX("PlayerCombat", 9);
        if (explodeEffect!= null)
        {
            Instantiate(explodeEffect, transform.position, Quaternion.identity);
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explodeRadius);
        foreach(Collider2D collider in colliders)
        {
            if (collider.gameObject.tag == "Enemy")
            {
                collider.gameObject.GetComponent<EnemyHealth>().DamageEnemy(damage);
            }
        }
    }
}
