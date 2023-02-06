using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] float healAmount;
    [SerializeField] float fadeSpeed;
    [SerializeField] float fadeAmount;
    private SpriteRenderer sprite;
    private float alpha;
    private bool fading;

    void Start()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        fading = true;
    }

    void Update()
    {
        alpha = sprite.color.a;

        if (fading)
        {
            sprite.color = new Color(1f, 1f, 1f, alpha - (fadeSpeed * Time.deltaTime));
            if (alpha < 1f-fadeAmount)
            {
                fading = false;
            }
        }
        else
        {
            sprite.color = new Color(1f, 1f, 1f, alpha + (fadeSpeed * Time.deltaTime));
            if (alpha > 1f)
            {
                fading = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            AudioManager.instance.PlaySFX("Pickup", 0);
            other.gameObject.GetComponent<PlayerHealth>().HealPlayer(healAmount);
            Destroy(gameObject);
        }
    }
}
