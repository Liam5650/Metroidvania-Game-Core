using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    [SerializeField] private Transform post;
    [SerializeField] private bool barrierDeployed;
    [SerializeField] float deploySpeed;
    private bool deploying, retracting;

    private void Start()
    {
        if (barrierDeployed) post.localPosition = new Vector3(0f, -2.375f, 0f);
        deploying = false; retracting = false;
    }

    private void Update()
    {
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

    public void Deploy()
    {
        deploying = true;
    }

    public void Retract()
    {
        retracting= true;
    }
}
