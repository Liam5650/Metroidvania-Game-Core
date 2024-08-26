using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrifterBehaviour : MonoBehaviour
{
    [SerializeField] float moveSpeed;               // Max speed the enemy moves
    [SerializeField] float accelRate = 1f;          // Acceleration rate to max speed
    [SerializeField] float trackingDistance = 8f;   // Distance from the player the enemy starts tracking
    [SerializeField] float bobSpeed = 2f;           // Speed the enemy oscillates up and down from flapping wings
    [SerializeField] float bobStrength = 0.1f;      // Distance the enemy oscillates up and down from flapping wings
    private float lastOffset;                       // Reference the last offset position calculated from oscillating
    private float currSpeed;                        // Current speed of the enemy
    private float startBobTimeOffset;               // Changes the start time of the sin movement oscillation to add randomness

    void Start()
    {
        // Set up initial state
        startBobTimeOffset = Random.Range(-1.5f, 1.5f);
        bobSpeed -= Random.Range(0f, 1f);
        bobStrength -= Random.Range(0f, 0.02f);
        currSpeed = 0f;
        lastOffset = bobStrength * Mathf.Sin((Time.time + startBobTimeOffset) * bobSpeed);
        transform.position = transform.position + new Vector3(0f, lastOffset, 0f);
    }

    void Update()
    {
        // Get the true position of the enemy (without the offset)
        transform.position = transform.position - new Vector3(0f, lastOffset, 0f);
        
        // Start moving towards player if tracking
        if ((PlayerCombat.instance.gameObject.transform.position - transform.position).magnitude < trackingDistance)
        {
            currSpeed += Time.deltaTime * accelRate;
            if (currSpeed > moveSpeed) currSpeed = moveSpeed;
            
        }
        else
        {
            currSpeed -= Time.deltaTime * accelRate;
            if (currSpeed < 0f) currSpeed = 0f;
        }

        transform.position = Vector2.MoveTowards(transform.position, PlayerCombat.instance.gameObject.transform.position, currSpeed * Time.deltaTime);

        // Apply the oscillation offset up and down
        float newOffset = bobStrength * Mathf.Sin((Time.time + startBobTimeOffset) * bobSpeed);
        transform.position = transform.position + new Vector3(0f, newOffset, 0f);
        lastOffset = newOffset;
    }
}