using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] private Barrier[] barriers;    // Barrier objects to close to lock the player in the room
    [SerializeField] private GameObject boss;       // Boss gameobject to enable
    [SerializeField] private GameObject upgrade;    // Upgrade to enable after defeating boss
    private bool inRoutine;                         // Whether or not we have to enter the spawn upgrade coroutine
    [SerializeField] private int eventID;           // Event ID used for saving data

    private void Awake()
    {
        // Destroy if player has already triggered event
        if (SaveController.instance.playerData.events[eventID] == 1) Destroy(gameObject);
    }

    private void Update()
    {
        // Enter the coroutine to spawn the upgrade if the boss has been destroyed and we haven't entered it yet
        if (boss == null && !inRoutine)
        {
            AudioManager.instance.PlaySFX("Boss", 4);
            inRoutine = true;
            StartCoroutine(SpawnUpgrade());
            AudioManager.instance.FadeOutMusic(2f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Initiate the boss fight
        if (collision.gameObject.tag == "Player")
        {
            SaveController.instance.playerData.events[eventID] = 1;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            foreach(Barrier barrier in barriers) barrier.Deploy();
            boss.gameObject.SetActive(true);
        }
    }

    private IEnumerator SpawnUpgrade()
    {
        // This coroutine handles the upgrade spawn sequence after beating the boss
        yield return new WaitForSeconds(2f);

        // Set up the upgrade to fade in
        SpriteRenderer spriteRenderer = upgrade.GetComponent<SpriteRenderer>();
        CircleCollider2D collider = upgrade.GetComponent<CircleCollider2D>();
        collider.enabled = false;
        spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
        upgrade.SetActive(true);

        while (spriteRenderer.color.a < 1f)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a + Time.deltaTime);
            yield return null;
        }
        collider.enabled = true;

        // Wait for the player to collect the upgrade
        while(upgrade != null)
        {
            yield return null;
        }
        foreach (Barrier barrier in barriers) barrier.Retract();
        yield return new WaitForSeconds(1f);
        AudioManager.instance.PlayMusic(1, 0f);
        AudioManager.instance.FadeInMusic(3f);
    }
}