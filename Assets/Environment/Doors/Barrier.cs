using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    [SerializeField] private Transform post;        // The position of the post
    [SerializeField] private bool barrierDeployed;  // Keeps track of whether or not the barrier has been deployed
    [SerializeField] float deploySpeed;             // The speed at which the barrier moves when triggered
    private bool deploying, retracting;             // Reference the state the barrier is in if in motion

    private void Start()
    {
        // Set up whether the post starts in a deployed state or not
        if (barrierDeployed) post.localPosition = new Vector3(0f, -2.375f, 0f);
        deploying = false; retracting = false;
    }

    private void Update()
    {
        // Move the post to the correct position depending on state
        if (deploying)
        {
            float currY = post.localPosition.y;
            if (currY > -2.375f)
            {
                post.localPosition = new Vector3(post.localPosition.x, Mathf.MoveTowards(currY, -2.375f, deploySpeed * Time.deltaTime), post.localPosition.z);
            }
            else deploying = false;

        }
        if (retracting)
        {
            float currY = post.localPosition.y;
            if (currY < 1.5f)
            {
                post.localPosition = new Vector3(post.localPosition.x, Mathf.MoveTowards(currY, 1.5f, deploySpeed * Time.deltaTime), post.localPosition.z);
            }
            else retracting = false;
        }
    }

    // Public functions to initiate the deploy or retract processes
    public void Deploy()
    {
        deploying = true;
    }

    public void Retract()
    {
        retracting= true;
    }
}