using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomIndicator : MonoBehaviour
{
    [SerializeField] float fadeSpeed;   // Speed at which the indicator fades in and out
    [SerializeField] float fadeAmount;  // Amount the indicator fades
    private SpriteRenderer sprite;      // Reference to alter alpha val of sprite color
    private float alpha;                // The alpha val of the sprite color
    private bool fading;                // Keeps track of current state
    private bool ignorePause;           // Used so that the sprite can continue changing if the game is paused by the map screen rather than pause screen

    void Start()
    {
        // Set up initial values
        sprite = gameObject.GetComponent<SpriteRenderer>();
        fading = true;
        ignorePause= false;
    }

    void Update()
    {
        // Get alpha and update according to game pause state
        alpha = sprite.color.a;

        if (fading)
        {
            if (!ignorePause)
            {
                sprite.color = new Color(1f, 1f, 1f, alpha - (fadeSpeed * Time.deltaTime));
            }
            else
            {
                sprite.color = new Color(1f, 1f, 1f, alpha - (fadeSpeed * Time.unscaledDeltaTime));
            }
            if (alpha < 1f - fadeAmount)
            {
                fading = false;
            }
        }
        else
        {
            if (!ignorePause)
            {
                sprite.color = new Color(1f, 1f, 1f, alpha + (fadeSpeed * Time.deltaTime));
            }
            else
            {
                sprite.color = new Color(1f, 1f, 1f, alpha + (fadeSpeed * Time.unscaledDeltaTime));
            }
            if (alpha > 1f)
            {
                fading = true;
            }
        }
    }

    public void IgnorePause(bool value)
    {
        // Used to make the behavior ignore pausing
        ignorePause = value;
    }
}
