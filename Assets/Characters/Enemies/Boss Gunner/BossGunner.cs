using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGunner : MonoBehaviour
{
    private EnemyHealth healthComponent;
    private float initHealth;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        healthComponent = GetComponent<EnemyHealth>();
        initHealth = healthComponent.GetHealth();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float damagePercent = healthComponent.GetHealth() / initHealth;
        if (damagePercent < 0f) damagePercent = 0f;
        spriteRenderer.color = new UnityEngine.Color(1f, 1f - (1f-damagePercent), 1f - (1f-damagePercent), 1f);
    }
}
