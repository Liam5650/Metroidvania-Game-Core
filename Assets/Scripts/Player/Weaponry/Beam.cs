using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{

    [SerializeField] float beamVelocity;
    [SerializeField] float beamLifetime;
    private Rigidbody2D rb;
    private PlayerMovement player;

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(beamVelocity * player.transform.localScale.x, 0f);
        Destroy(gameObject, beamLifetime);
    }
}