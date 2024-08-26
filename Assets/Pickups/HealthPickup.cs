using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] float healAmount;      // Amount of health to add
    [SerializeField] float fadeSpeed;       // Speed at which the pickup effect flashes
    [SerializeField] float fadeAmount;      // Amount the pickup fades back and forth to create a flashing effect
    private SpriteRenderer sprite;          // Reference to change the color values of the sprite
    private bool fading;                    // Indicates if we are fading in or out

    void Start()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        fading = true;
    }

    void Update()
    {
        // Handle actions and transitions of fading in and out states
        if (fading)
        {
            if (sprite.color.a < 1f - fadeAmount) fading = false;
            else sprite.color = new Color(1f, 1f, 1f, sprite.color.a - (fadeSpeed * Time.deltaTime));
        }
        else
        {
            if (sprite.color.a > 1f) fading = true;
            else sprite.color = new Color(1f, 1f, 1f, sprite.color.a + (fadeSpeed * Time.deltaTime));
        }

        // Clamp the alpha values to their respective bounds
        sprite.color = new Color(1f, 1f, 1f, Mathf.Clamp(sprite.color.a, 0.999f - fadeAmount, 1.001f));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Restore health and destroy if player touches pickup
        if (other.tag == "Player")
        {
            AudioManager.instance.PlaySFX("Pickup", 0);
            other.gameObject.GetComponent<PlayerHealth>().HealPlayer(healAmount);
            Destroy(gameObject);
        }
    }
}
