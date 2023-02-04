using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] private string bossID;
    [SerializeField] private Barrier[] barriers;
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject upgrade;
    private bool inRoutine;

    private void Update()
    {
        if (boss == null && !inRoutine)
        {
            inRoutine = true;
            StartCoroutine(SpawnUpgrade());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // also check if savedata shows boss hass not been beaten, then spawn
        if (collision.gameObject.tag == "Player")
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            foreach(Barrier barrier in barriers) barrier.Deploy();
            boss.gameObject.SetActive(true);
        }
    }

    private IEnumerator SpawnUpgrade()
    {
        yield return new WaitForSeconds(2f);

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
        while(upgrade != null)
        {
            yield return null;
        }
        foreach (Barrier barrier in barriers) barrier.Retract();
    }
}