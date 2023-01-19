using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomIndicator : MonoBehaviour
{
    [SerializeField] float fadeSpeed;
    [SerializeField] float fadeAmount;
    private SpriteRenderer sprite;
    private float alpha;
    private bool fading;
    private bool ignorePause;

    void Start()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        fading = true;
        ignorePause= false;
    }

    void Update()
    {
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

    public void IgnorePause(bool value )
    {
        ignorePause = value;
    }
}
