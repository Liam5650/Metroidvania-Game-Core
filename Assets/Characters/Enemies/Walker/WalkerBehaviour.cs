using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerBehaviour : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float waitTime;
    private float timeWaited;
    private List<Vector2> walkPoints = new List<Vector2>();
    private int targetIndex;
    private bool walking;
    private Animator anim;

    void Start()
    {
        // Get all of the child walk point transforms
        foreach (Transform child in transform)
        {
            walkPoints.Add(child.transform.position);
        }
        // Make sure there is at least 1 target point, then start walk
        if (walkPoints.Count >= 1)
        {
            targetIndex = 0;
            walking = true;
            anim = gameObject.GetComponent<Animator>();
            anim.SetBool("isWalking", true);
        }
    }

    void Update()
    {   
        // Handle behaviour when walking
        if (walking)
        {
            // Get the new position based on our current pos, target pos, and a step size
            float step = moveSpeed * Time.deltaTime;
            float newX = Mathf.MoveTowards(transform.position.x, walkPoints[targetIndex].x, step);
            float heading = newX - transform.position.x;
            transform.position = new Vector2(newX, transform.position.y);

            // See if we are moving and set anim accordingly. Otherwise, end walk
            if (heading < 0)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else if (heading > 0)
            {
                transform.localScale = Vector3.one;
            }
            else
            {
                transform.localScale = Vector3.one;
                walking = false;
                anim.SetBool("isWalking", false);
            }
        }
        //Handle behaviour if we are waiting and have points to travel to
        else if (walkPoints.Count > 1)
        {
            // Wait for the specified time at curr point before moving
            if (timeWaited > waitTime)
            {
                // Pick a new point and start walk
                targetIndex = (targetIndex + 1) % walkPoints.Count;
                walking = true;
                anim.SetBool("isWalking", true);
                timeWaited = 0f;
            }
            else
            {
                timeWaited += Time.deltaTime;
            } 
        }
    }
}