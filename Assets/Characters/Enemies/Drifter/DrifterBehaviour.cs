using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrifterBehaviour : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float accelRate = 1f;
    [SerializeField] float trackingDistance = 8f;
    [SerializeField] float bobSpeed = 2f;
    [SerializeField] float bobStrength = 0.1f;
    private float lastOffset;
    private float currSpeed;
    private float startBobTimeOffset;

    void Start()
    {
        startBobTimeOffset = Random.Range(-1.5f, 1.5f);
        bobSpeed -= Random.Range(0f, 1f);
        bobStrength -= Random.Range(0f, 0.2f);
        currSpeed = 0f;
        lastOffset = bobStrength * Mathf.Sin((Time.time + startBobTimeOffset) * bobSpeed);
        transform.position = transform.position + new Vector3(0f, lastOffset, 0f);
    }

    void Update()
    {

        transform.position = transform.position - new Vector3(0f, lastOffset, 0f);

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

        float newOffset = bobStrength * Mathf.Sin((Time.time + startBobTimeOffset) * bobSpeed);
        transform.position = transform.position + new Vector3(0f, newOffset, 0f);
        lastOffset = newOffset;
    }
}
