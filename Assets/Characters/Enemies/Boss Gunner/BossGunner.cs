using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGunner : MonoBehaviour
{
    private EnemyHealth healthComponent;
    private float initHealth;
    private SpriteRenderer spriteRenderer;
    private Transform player;
    private Animator anim;
    [SerializeField] private float fadeInSpeed;
    private bool inState;
    private float damagePercent;
    [SerializeField] private float trackSpeed, waitTime, chargeDistance, chargeSpeed;

    [SerializeField] GameObject beam, chargedBeam, chargedEffect;
    [SerializeField] Transform shootPoint;


    // Start is called before the first frame update
    void Start()
    {
        healthComponent = GetComponent<EnemyHealth>();
        initHealth = healthComponent.GetHealth();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        player = FindObjectOfType<PlayerMovement>().gameObject.transform;
        StartCoroutine(FadeIn());
    }

    // Update is called once per frame
    void Update()
    {
        damagePercent = healthComponent.GetHealth() / initHealth;
        if (damagePercent < 0f) damagePercent = 0f;
        spriteRenderer.color = new UnityEngine.Color(1f, 1f - (1f-damagePercent), 1f - (1f-damagePercent), spriteRenderer.color.a);

        // Boss states
        if (!inState)
        {
            int randState = Random.Range(0, 10);
            if (randState > 3) StartCoroutine(Shoot());
            else StartCoroutine(Charge());
        }
    }

    private IEnumerator FadeIn()
    {
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
        inState= true;

        float multiplier;
        if (damagePercent >= 0.66f) multiplier = 1f;
        else if (damagePercent < 0.66f && damagePercent >= 0.33f) multiplier = 1.5f;
        else multiplier = 2f;


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

        yield return new WaitForSeconds(waitTime/multiplier);

        // Boss sub states
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

        yield return new WaitForSeconds(waitTime);
        inState= false;
    }

    private IEnumerator Charge()
    {
        inState= true;

        float multiplier;
        if (damagePercent >= 0.66f) multiplier = 1f;
        else if (damagePercent < 0.66f && damagePercent >= 0.33f) multiplier = 1.5f;
        else multiplier = 2f;

        yield return new WaitForSeconds(waitTime);
        anim.speed = 3*multiplier;
        yield return new WaitForSeconds(1f/multiplier);


        float offset = chargeDistance * gameObject.transform.localScale.x * -1f; //Flip since the sprite is facing backwards
        float targetX = transform.position.x + offset;
        while (transform.position.x != targetX)
        {
            transform.position = new Vector3(Mathf.MoveTowards(transform.position.x, targetX, chargeSpeed * multiplier * Time.deltaTime), transform.position.y, transform.position.z);
            yield return null;
        }
        gameObject.transform.localScale = new Vector3(-1f * gameObject.transform.localScale.x, 1f, 1f);
        anim.speed = 1;
        yield return new WaitForSeconds(waitTime);
        inState= false;
    }
}