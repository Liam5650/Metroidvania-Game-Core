using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGunner : MonoBehaviour
{
    private EnemyHealth healthComponent;        // The boss health controller
    private float initHealth;                   // Initial boss health
    private SpriteRenderer spriteRenderer;      // Used to apply damage coloring to boss sprite
    private Transform player;                   // Used to track the player
    private Animator anim;                      // Used to set boss animation
    [SerializeField] private float fadeInSpeed; // Speed at which the boss fades in after triggering the boss event
    private bool inState;                       // Tracks if the boss is within a behavior state
    private float damagePercent;                // The percent of health damage the boss has taken
    [SerializeField] private float trackSpeed, waitTime, chargeDistance, chargeSpeed;   // Various state behavior modifiers
    [SerializeField] GameObject beam, chargedBeam, chargedEffect;                       // Boss weaponry
    [SerializeField] Transform shootPoint;      // Where the boss beam shoots from


    void Start()
    {
        // Initialize the boss
        healthComponent = GetComponent<EnemyHealth>();
        initHealth = healthComponent.GetHealth();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        player = FindObjectOfType<PlayerMovement>().gameObject.transform;
        StartCoroutine(FadeIn());
    }

    void Update()
    {
        // Set damage percent and sprite color to show damage
        damagePercent = healthComponent.GetHealth() / initHealth;
        if (damagePercent < 0f) damagePercent = 0f;
        spriteRenderer.color = new UnityEngine.Color(1f, 1f - (1f-damagePercent), 1f - (1f-damagePercent), spriteRenderer.color.a);

        // Choose boss state
        if (!inState)
        {
            int randState = Random.Range(0, 10);
            if (randState > 3) StartCoroutine(Shoot());
            else StartCoroutine(Charge());
        }
    }

    private IEnumerator FadeIn()
    {
        // Start the boss fight
        inState= true;
        AudioManager.instance.FadeOutMusic(1.5f);
        yield return new WaitForSeconds(2f);

        while (spriteRenderer.color.a < 1f)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a + fadeInSpeed * Time.deltaTime);
            yield return null;
        }

        AudioManager.instance.PlaySFX("Boss", 3);
        yield return new WaitForSeconds(1f);
        AudioManager.instance.PlayMusic(2);
        AudioManager.instance.SetMusicVolume(0.75f);
        gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
        inState= false;
    }

    private IEnumerator Shoot()
    {
        // This state is for when the boss tracks and shoots at the player
        inState= true;

        // Change the speed of the tracking based on damage percent
        float multiplier;
        if (damagePercent >= 0.66f) multiplier = 1f;
        else if (damagePercent < 0.66f && damagePercent >= 0.33f) multiplier = 1.5f;
        else multiplier = 2f;

        // Briefly pause and then start tracking
        yield return new WaitForSeconds(waitTime);
        float trackTime = 2f;
        float timeTracked = 0f;

        while (timeTracked < trackTime)
        {
            timeTracked += Time.deltaTime;
            float distanceSpeedMultiplyer = Mathf.Abs(player.position.y - transform.position.y);
            if (distanceSpeedMultiplyer > 2*multiplier) distanceSpeedMultiplyer = 2*multiplier;
            float newY = Mathf.MoveTowards(transform.position.y, player.position.y, trackSpeed * multiplier * Time.deltaTime * distanceSpeedMultiplyer);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }

        // Briefly pause after tracking
        yield return new WaitForSeconds(waitTime/multiplier);

        // Shoot at the player. There are different actions based on boss damage percent
        if (damagePercent >= 0.66f)
        {
            AudioManager.instance.PlaySFX("Boss", 0);
            Instantiate(beam, shootPoint.position, Quaternion.identity).GetComponent<GunnerBeam>().Fire(gameObject.transform.localScale.x * -1f);
            yield return new WaitForSeconds(0.2f);
            AudioManager.instance.PlaySFX("Boss", 0);
            Instantiate(beam, shootPoint.position, Quaternion.identity).GetComponent<GunnerBeam>().Fire(gameObject.transform.localScale.x * -1f);
            yield return new WaitForSeconds(0.2f);
            AudioManager.instance.PlaySFX("Boss", 0);
            Instantiate(beam, shootPoint.position, Quaternion.identity).GetComponent<GunnerBeam>().Fire(gameObject.transform.localScale.x * -1f);
        }
        else if (damagePercent < 0.66f && damagePercent >= 0.33f)
        {
            chargedEffect.SetActive(true);
            AudioManager.instance.PlaySFX("Boss", 2);
            yield return new WaitForSeconds(1f);
            chargedEffect.SetActive(false);
            AudioManager.instance.PlaySFX("Boss", 1);
            Instantiate(chargedBeam, shootPoint.position, Quaternion.identity).GetComponent<GunnerBeam>().Fire(gameObject.transform.localScale.x * -1f);

        }
        else if (damagePercent < 0.33f)
        {
            AudioManager.instance.PlaySFX("Boss", 0);
            Instantiate(beam, shootPoint.position, Quaternion.identity).GetComponent<GunnerBeam>().Fire(gameObject.transform.localScale.x * -1f);
            yield return new WaitForSeconds(0.1f);
            AudioManager.instance.PlaySFX("Boss", 0);
            Instantiate(beam, shootPoint.position, Quaternion.identity).GetComponent<GunnerBeam>().Fire(gameObject.transform.localScale.x * -1f);
            yield return new WaitForSeconds(0.1f);
            AudioManager.instance.PlaySFX("Boss", 0);
            Instantiate(beam, shootPoint.position, Quaternion.identity).GetComponent<GunnerBeam>().Fire(gameObject.transform.localScale.x * -1f);
            yield return new WaitForSeconds(0.5f);
            chargedEffect.SetActive(true);
            AudioManager.instance.PlaySFX("Boss", 2);
            yield return new WaitForSeconds(0.5f);
            chargedEffect.SetActive(false);
            AudioManager.instance.PlaySFX("Boss", 1);
            Instantiate(chargedBeam, shootPoint.position, Quaternion.identity).GetComponent<GunnerBeam>().Fire(gameObject.transform.localScale.x * -1f);
        }

        // Briefly pause after shooting, and end state
        yield return new WaitForSeconds(waitTime);
        inState= false;
    }

    private IEnumerator Charge()
    {
        // This state is for when the boss changes sides
        inState= true;

        // Change the speed of the charge based on damage percent
        float multiplier;
        if (damagePercent >= 0.66f) multiplier = 1f;
        else if (damagePercent < 0.66f && damagePercent >= 0.33f) multiplier = 1.5f;
        else multiplier = 2f;

        yield return new WaitForSeconds(waitTime);
        anim.speed = 3*multiplier;
        yield return new WaitForSeconds(1f/multiplier);

        // Change sides
        float offset = chargeDistance * gameObject.transform.localScale.x * -1f; //Flip since the sprite is facing backwards
        float targetX = transform.position.x + offset;
        while (transform.position.x != targetX)
        {
            transform.position = new Vector3(Mathf.MoveTowards(transform.position.x, targetX, chargeSpeed * multiplier * Time.deltaTime), transform.position.y, transform.position.z);
            yield return null;
        }
        gameObject.transform.localScale = new Vector3(-1f * gameObject.transform.localScale.x, 1f, 1f);
        anim.speed = 1;

        // Pause after charging and end state
        yield return new WaitForSeconds(waitTime);
        inState= false;
    }
}