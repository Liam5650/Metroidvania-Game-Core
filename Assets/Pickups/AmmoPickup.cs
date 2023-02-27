using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] int rechargeAmount;
    [SerializeField] float fadeSpeed;
    [SerializeField] float fadeAmount;
    private SpriteRenderer sprite;
    private bool fading;

    void Start()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        fading = true;
    }

    void Update()
    { 
        // Set up fading in and out states
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Restore ammo and destroy
        if (other.tag == "Player")
        {
            AudioManager.instance.PlaySFX("Pickup", 0);
            other.gameObject.GetComponent<PlayerCombat>().RechargeAmmo(rechargeAmount);
            Destroy(gameObject);
        }
    }
}
